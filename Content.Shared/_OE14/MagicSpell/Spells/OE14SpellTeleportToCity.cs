using System.Numerics;
using Content.Shared._OE14.Demiplane.Components;
using Content.Shared.Teleportation.Systems;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Map;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellTeleportToCity : OE14SpellEffect
{
    [DataField]
    public EntProtoId PortalProto = "OE14TempPortalToCity";
    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Position is null)
            return;

        var net = IoCManager.Resolve<INetManager>();

        if (net.IsClient)
            return;

        var random = IoCManager.Resolve<IRobustRandom>();
        var linkSys = entManager.System<LinkedEntitySystem>();

        var first = entManager.SpawnAtPosition(PortalProto, args.Position.Value);

        // Try to find a navigation map first (for demiplane maps with OE14DemiplaneNavigationMapComponent)
        var query = entManager.EntityQueryEnumerator<OE14DemiplaneNavigationMapComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var map, out var xform))
        {
            var randomOffset = new Vector2(random.Next(-1, 1), random.Next(-1, 1));
            var portalLinked = entManager.SpawnAtPosition(PortalProto, xform.Coordinates.Offset(randomOffset));

            linkSys.TryLink(first, portalLinked, true);
            return;
        }

        // Fallback: Find any valid map position (station or main map)
        // This allows the spell to work even without a navigation crystal
        EntityCoordinates? spawnCoords = null;
        
        var transformQuery = entManager.EntityQueryEnumerator<TransformComponent>();
        while (transformQuery.MoveNext(out var xform))
        {
            // Find coordinates that are valid and on a real map (not null space)
            if (xform.MapUid != null && xform.MapUid != EntityUid.Invalid && xform.Coordinates.IsValid(entManager))
            {
                // Get the map position to spawn at
                spawnCoords = xform.Coordinates;
                break;
            }
        }

        // If still no valid position, spawn near the original spell target
        if (spawnCoords is null)
        {
            spawnCoords = args.Position.Value.Offset(new Vector2(random.Next(-3, 3), random.Next(-3, 3)));
        }

        var portalFallback = entManager.SpawnAtPosition(PortalProto, spawnCoords.Value);
        linkSys.TryLink(first, portalFallback, true);
    }
}
