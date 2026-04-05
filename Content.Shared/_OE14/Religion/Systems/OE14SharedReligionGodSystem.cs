using Content.Shared._OE14.Religion.Components;
using Content.Shared._OE14.Religion.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Religion.Systems;

public abstract partial class OE14SharedReligionGodSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();
        InitializeObservation();
        InitializeFollowers();
        InitializeAltars();
    }

    public HashSet<Entity<OE14ReligionEntityComponent>> GetGods(ProtoId<OE14ReligionPrototype> religion)
    {
        HashSet<Entity<OE14ReligionEntityComponent>> gods = new();

        var query = EntityQueryEnumerator<OE14ReligionEntityComponent>();
        while (query.MoveNext(out var uid, out var god))
        {
            if (god.Religion != religion)
                continue;

            gods.Add(new Entity<OE14ReligionEntityComponent>(uid, god));
        }

        return gods;
    }

    public abstract void SendMessageToGods(ProtoId<OE14ReligionPrototype> religion, string msg, EntityUid source);
}

/// <summary>
/// It is invoked on altars and followers when they change their religion.
/// </summary>
public sealed class OE14ReligionChangedEvent(ProtoId<OE14ReligionPrototype>? oldRel, ProtoId<OE14ReligionPrototype>? newRel) : EntityEventArgs
{
    public ProtoId<OE14ReligionPrototype>? OldReligion = oldRel;
    public ProtoId<OE14ReligionPrototype>? NewReligion = newRel;
}
