using Content.Shared._OE14.Farming;
using Content.Shared._OE14.Farming.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._OE14.Farming;

public sealed partial class OE14FarmingSystem : OE14SharedFarmingSystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainer = default!;

    public override void Initialize()
    {
        base.Initialize();

        InitializeResources();

        SubscribeLocalEvent<OE14PlantComponent, MapInitEvent>(OnMapInit);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<OE14PlantComponent>();
        while (query.MoveNext(out var uid, out var plant))
        {
            if (_timing.CurTime <= plant.NextUpdateTime)
                continue;

            var newTime = _random.NextFloat(plant.UpdateFrequency);
            plant.NextUpdateTime = _timing.CurTime + TimeSpan.FromSeconds(newTime);

            var ev = new OE14PlantUpdateEvent((uid, plant));
            RaiseLocalEvent(uid, ev);

            AffectResource((uid, plant), ev.ResourceDelta);
            AffectEnergy((uid, plant), ev.EnergyDelta);

            var ev2 = new OE14AfterPlantUpdateEvent((uid, plant));
            RaiseLocalEvent(uid, ev2);

            Dirty(uid, plant);
        }
    }

    private void OnMapInit(Entity<OE14PlantComponent> plant, ref MapInitEvent args)
    {
        var newTime = _random.NextFloat(plant.Comp.UpdateFrequency);
        plant.Comp.NextUpdateTime = _timing.CurTime + TimeSpan.FromSeconds(newTime);
    }
}
