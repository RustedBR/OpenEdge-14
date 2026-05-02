using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Map;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellArea : OE14SpellEffect
{
    [DataField(required: true)]
    public List<OE14SpellEffect> Effects { get; set; } = new();

    [DataField]
    public EntityWhitelist? Whitelist;

    /// <summary>
    /// How many entities can be subject to EntityEffect? Leave 0 to remove the restriction.
    /// </summary>
    [DataField]
    public int MaxTargets = 0;

    [DataField(required: true)]
    public float Range = 1f;

    [DataField]
    public bool AffectCaster = false;

    /// <summary>
    /// Adds (effInt * IntRangeBonus) to the base Range.
    /// INT 5 = +0.5 tiles per 0.1 bonus; INT 10 = +1.0 tile per 0.1 bonus.
    /// </summary>
    [DataField]
    public float IntRangeBonus = 0f;

    /// <summary>
    /// Adds floor(effInt * IntMaxTargetsBonus) to MaxTargets.
    /// Only applied when MaxTargets > 0.
    /// </summary>
    [DataField]
    public float IntMaxTargetsBonus = 0f;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        EntityCoordinates? targetPoint = null;

        if (args.Target is not null &&
            entManager.TryGetComponent<TransformComponent>(args.Target.Value, out var transformComponent))
            targetPoint = transformComponent.Coordinates;
        else if (args.Position is not null)
            targetPoint = args.Position;

        if (targetPoint is null)
            return;

        var effectiveRange = Range;
        var effectiveMaxTargets = MaxTargets;

        if ((IntRangeBonus > 0f || IntMaxTargetsBonus > 0f) &&
            args.User is not null &&
            entManager.TryGetComponent<OE14CharacterStatsComponent>(args.User.Value, out var stats))
        {
            var effInt = Math.Clamp(stats.Intelligence + stats.IntelligenceModifier, 1, stats.MaxStatValue);
            effectiveRange += effInt * IntRangeBonus;
            if (MaxTargets > 0)
                effectiveMaxTargets += (int)(effInt * IntMaxTargetsBonus);
        }

        var lookup = entManager.System<EntityLookupSystem>();
        var whitelist = entManager.System<EntityWhitelistSystem>();

        var entitiesAround = lookup.GetEntitiesInRange(targetPoint.Value, effectiveRange, LookupFlags.Uncontained);

        var count = 0;
        foreach (var entity in entitiesAround)
        {
            if (entity == args.User && !AffectCaster)
                continue;

            if (Whitelist is not null && !whitelist.IsValid(Whitelist, entity))
                continue;

            foreach (var effect in Effects)
            {
                effect.Effect(entManager, new OE14SpellEffectBaseArgs(args.User, null, entity, targetPoint));
            }

            count++;

            if (effectiveMaxTargets > 0 && count >= effectiveMaxTargets)
                break;
        }
    }
}
