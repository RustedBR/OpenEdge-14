using System.Numerics;
using Content.Shared._OE14.UniqueLoot;
using Content.Shared._OE14.Vampire.Components;
using Content.Shared.Teleportation.Systems;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellTeleportToVampireSingleton : OE14SpellEffect
{
    [DataField]
    public EntProtoId PortalProto = "OE14TempPortalRed";

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Position is null)
            return;
        if (args.User is null)
            return;

        if (!entManager.TryGetComponent<OE14VampireComponent>(args.User.Value, out var vampireComponent))
            return;

        var net = IoCManager.Resolve<INetManager>();

        if (net.IsClient)
            return;

        var protoMan = IoCManager.Resolve<IPrototypeManager>();
        var random = IoCManager.Resolve<IRobustRandom>();
        var linkSys = entManager.System<LinkedEntitySystem>();
        var query = entManager.EntityQueryEnumerator<OE14SingletonComponent, TransformComponent>();

        if (!protoMan.TryIndex(vampireComponent.Faction, out var indexedVampireFaction))
            return;

        var first = entManager.SpawnAtPosition(PortalProto, args.Position.Value);

        while (query.MoveNext(out var uid, out var singleton, out var xform))
        {

            if (singleton.Key != indexedVampireFaction.SingletonTeleportKey)
                continue;

            var second = entManager.SpawnAtPosition(PortalProto, xform.Coordinates);

            linkSys.TryLink(first, second, true);
            return;
        }
    }
}
