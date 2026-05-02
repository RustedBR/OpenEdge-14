using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared.StatusEffectNew;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellApplyStatusEffect : OE14SpellEffect
{
    [DataField(required: true)]
    public EntProtoId StatusEffect = default;

    [DataField(required: true)]
    public TimeSpan Duration = TimeSpan.FromSeconds(1f);

    [DataField]
    public bool Refresh = true;

    /// <summary>
    /// Adds (effInt * IntDurationScale) seconds to the base Duration.
    /// INT 5 = +5s per 1.0 scale; INT 10 = +10s per 1.0 scale.
    /// Leave at 0 to disable INT scaling.
    /// </summary>
    [DataField]
    public float IntDurationScale = 0f;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        var effectiveDuration = Duration;

        if (IntDurationScale > 0f &&
            args.User is not null &&
            entManager.TryGetComponent<OE14CharacterStatsComponent>(args.User.Value, out var stats))
        {
            var effInt = Math.Clamp(stats.Intelligence + stats.IntelligenceModifier, 1, stats.MaxStatValue);
            effectiveDuration += TimeSpan.FromSeconds(effInt * IntDurationScale);
        }

        var effectSys = entManager.System<StatusEffectsSystem>();

        if (!Refresh)
            effectSys.TryAddStatusEffectDuration(args.Target.Value, StatusEffect, effectiveDuration);
        else
            effectSys.TrySetStatusEffectDuration(args.Target.Value, StatusEffect, effectiveDuration);
    }
}
