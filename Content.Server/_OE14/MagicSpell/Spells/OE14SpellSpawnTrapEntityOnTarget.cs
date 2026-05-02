using Content.Shared._OE14.MagicSpell.Spells;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.MagicSpell.Spells;

/// <summary>
/// Spawns a trap entity at the target position and assigns the spell caster as the owner
/// on OE14TrapCollideComponent. Owner immunity is handled by OE14MagicSystem.OnTrapCollide.
/// </summary>
public sealed partial class OE14SpellSpawnTrapEntityOnTarget : OE14SpellEffect
{
    [DataField(required: true)]
    public EntProtoId Trap = default!;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        EntityCoordinates? targetPoint = null;
        if (args.Position is not null)
            targetPoint = args.Position.Value;
        if (args.Target is not null && entManager.TryGetComponent<TransformComponent>(args.Target.Value, out var xform))
            targetPoint = xform.Coordinates;

        if (targetPoint is null)
            return;

        var netMan = IoCManager.Resolve<INetManager>();
        if (netMan.IsClient)
            return;

        var trap = entManager.SpawnAtPosition(Trap, targetPoint.Value);

        if (entManager.TryGetComponent<OE14TrapCollideComponent>(trap, out var trapComp))
            trapComp.Owner = args.User;
    }
}
