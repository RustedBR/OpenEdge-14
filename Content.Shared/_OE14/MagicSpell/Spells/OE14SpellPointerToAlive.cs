using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellPointerToAlive : OE14SpellEffect
{
    [DataField(required: true)]
    public EntProtoId PointerEntity;

    [DataField]
    public float SearchRange = 30f;

    /// <summary>
    /// Added to SearchRange per point of effective INT. Final range = SearchRange + effInt * IntRangeBonus.
    /// </summary>
    [DataField]
    public float IntRangeBonus = 0f;

    /// <summary>
    /// When true, limits the number of pointers spawned to the caster's effective INT value.
    /// Entities are sorted by distance (closest first).
    /// </summary>
    [DataField]
    public bool MaxTargetsFromInt = false;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        var net = IoCManager.Resolve<INetManager>();
        if (net.IsClient)
            return;

        if (args.User is not { } user)
            return;

        var lookup = entManager.System<EntityLookupSystem>();
        var mobStateSys = entManager.System<MobStateSystem>();
        var transform = entManager.System<SharedTransformSystem>();

        var originPosition = transform.GetWorldPosition(user);
        var originEntPosition = transform.GetMoverCoordinates(user);

        var effectiveRange = SearchRange;
        var maxTargets = int.MaxValue;

        if (entManager.TryGetComponent<OE14CharacterStatsComponent>(user, out var stats))
        {
            var effInt = Math.Clamp(stats.Intelligence + stats.IntelligenceModifier, 1, stats.MaxStatValue);
            effectiveRange += effInt * IntRangeBonus;
            if (MaxTargetsFromInt)
                maxTargets = (int) effInt;
        }

        var entitiesInRange = lookup.GetEntitiesInRange<MobStateComponent>(originEntPosition, effectiveRange);

        var targets = new List<(EntityUid Owner, float Distance)>();
        foreach (var ent in entitiesInRange)
        {
            if (ent.Owner == user)
                continue;

            if (mobStateSys.IsDead(ent))
                continue;

            var targetPosition = transform.GetWorldPosition(ent);
            var distance = (targetPosition - originPosition).Length();
            targets.Add((ent.Owner, distance));
        }

        targets.Sort((a, b) => a.Distance.CompareTo(b.Distance));

        for (var i = 0; i < Math.Min(targets.Count, maxTargets); i++)
        {
            var targetPosition = transform.GetWorldPosition(targets[i].Owner);
            var angle = new Angle(targetPosition - originPosition);
            var pointer = entManager.Spawn(PointerEntity, new MapCoordinates(originPosition, transform.GetMapId(originEntPosition)));
            transform.SetWorldRotation(pointer, angle + Angle.FromDegrees(90));
        }
    }
}
