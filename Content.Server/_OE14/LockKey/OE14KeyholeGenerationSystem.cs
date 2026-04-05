using System.Linq;
using Content.Server.Labels;
using Content.Shared._OE14.LockKey;
using Content.Shared._OE14.LockKey.Components;
using Content.Shared.GameTicking;
using Content.Shared.Labels.EntitySystems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._OE14.LockKey;

public sealed partial class OE14KeyholeGenerationSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly LabelSystem _label = default!;

    private Dictionary<ProtoId<OE14LockTypePrototype>, List<int>> _roundKeyData = new(); //TODO: it won't survive saving and loading. This data must be stored in some component.

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundEnd);

        SubscribeLocalEvent<OE14LockComponent, MapInitEvent>(OnLockInit);
        SubscribeLocalEvent<OE14KeyComponent, MapInitEvent>(OnKeyInit);
    }

    #region Init
    private void OnRoundEnd(RoundRestartCleanupEvent ev)
    {
        _roundKeyData = new();
    }

    private void OnKeyInit(Entity<OE14KeyComponent> keyEnt, ref MapInitEvent args)
    {
        if (keyEnt.Comp.AutoGenerateShape != null)
        {
            SetShape(keyEnt, keyEnt.Comp.AutoGenerateShape.Value);
        }
    }

    private void OnLockInit(Entity<OE14LockComponent> lockEnt, ref MapInitEvent args)
    {
        if (lockEnt.Comp.AutoGenerateShape != null)
        {
            SetShape(lockEnt, lockEnt.Comp.AutoGenerateShape.Value);
        }
        else if (lockEnt.Comp.AutoGenerateRandomShape != null)
        {
            SetRandomShape(lockEnt, lockEnt.Comp.AutoGenerateRandomShape.Value);
        }
    }
    #endregion

    private List<int> GetKeyLockData(ProtoId<OE14LockTypePrototype> category)
    {
        if (_roundKeyData.ContainsKey(category))
            return new List<int>(_roundKeyData[category]);

        var newData = GenerateNewUniqueLockData(category);
        _roundKeyData[category] = newData;
        return newData;
    }

    public void SetShape(Entity<OE14KeyComponent> keyEnt, ProtoId<OE14LockTypePrototype> type)
    {
        keyEnt.Comp.LockShape = GetKeyLockData(type);
        DirtyField(keyEnt, keyEnt.Comp, nameof(OE14KeyComponent.LockShape));

        var indexedType = _proto.Index(type);
        if (indexedType.Name is not null)
            _label.Label(keyEnt, Loc.GetString(indexedType.Name.Value));
    }

    public void SetShape(Entity<OE14LockComponent> lockEnt, ProtoId<OE14LockTypePrototype> type)
    {
        lockEnt.Comp.LockShape = GetKeyLockData(type);

        var indexedType = _proto.Index(type);
        if (indexedType.Name is not null)
            _label.Label(lockEnt, Loc.GetString(indexedType.Name.Value));

        DirtyField(lockEnt, lockEnt.Comp, nameof(OE14LockComponent.LockShape));
    }

    public void SetRandomShape(Entity<OE14LockComponent> lockEnt, int complexity)
    {
        lockEnt.Comp.LockShape = new List<int>();
        for (var i = 0; i < complexity; i++)
        {
            lockEnt.Comp.LockShape.Add(_random.Next(-SharedOE14LockKeySystem.DepthComplexity, SharedOE14LockKeySystem.DepthComplexity));
        }

        DirtyField(lockEnt, lockEnt.Comp, nameof(OE14LockComponent.LockShape));
    }

    private List<int> GenerateNewUniqueLockData(ProtoId<OE14LockTypePrototype> category)
    {
        List<int> newKeyData = new();
        var categoryData = _proto.Index(category);
        var iteration = 0;

        while (true)
        {
            //Generate try
            newKeyData = new List<int>();
            for (var i = 0; i < categoryData.Complexity; i++)
            {
                newKeyData.Add(_random.Next(-SharedOE14LockKeySystem.DepthComplexity, SharedOE14LockKeySystem.DepthComplexity));
            }

            // Identity Check shit code
            // It is currently trying to generate a unique code. If it fails to generate a unique code 100 times, it will output the last generated non-unique code.
            var unique = true;
            foreach (var pair in _roundKeyData)
            {
                if (newKeyData.SequenceEqual(pair.Value))
                {
                    unique = false;
                    break;
                }
            }
            if (unique)
                return newKeyData;
            else
                iteration++;

            if (iteration > 100)
            {
                break;
            }
        }
        Log.Error("The unique key for CPLockSystem could not be generated!");
        return newKeyData; //FUCK
    }
}
