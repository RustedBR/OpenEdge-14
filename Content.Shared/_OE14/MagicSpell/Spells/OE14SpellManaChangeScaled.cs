using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared._OE14.MagicEnergy;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared.FixedPoint;

namespace Content.Shared._OE14.MagicSpell.Spells;

/// <summary>
///     Adds or removes mana from the target, optionally scaled by the caster's INT.
///     Use positive ManaDelta to restore mana, negative to drain.
/// </summary>
public sealed partial class OE14SpellManaChangeScaled : OE14SpellEffect
{
    [DataField(required: true)]
    public FixedPoint2 ManaDelta = 0;

    /// <summary>
    ///     When true, unsafe mana changes (e.g. negative = burnout) are allowed.
    ///     Set to false to clamp at 0.
    /// </summary>
    [DataField]
    public bool Safe = true;

    /// <summary>
    ///     When true, multiplies ManaDelta by the caster's INT multiplier.
    ///     INT 5 = 1.0x, INT 10 = 1.5x, INT 1 = 0.5x.
    /// </summary>
    [DataField]
    public bool ScaleWithCasterInt = false;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        if (!entManager.HasComponent<OE14MagicEnergyContainerComponent>(args.Target.Value))
            return;

        var effectiveDelta = ManaDelta;

        if (ScaleWithCasterInt &&
            args.User is not null &&
            entManager.TryGetComponent<OE14CharacterStatsComponent>(args.User.Value, out var stats))
        {
            var effInt = Math.Clamp(stats.Intelligence + stats.IntelligenceModifier, 1, stats.MaxStatValue);
            var multiplier = effInt >= 5
                ? 1.0f + (effInt - 5) * 0.10f
                : 1.0f + (effInt - 5) * 0.125f;
            effectiveDelta = FixedPoint2.New((float)ManaDelta * multiplier);
        }

        var magicEnergy = entManager.System<OE14SharedMagicEnergySystem>();
        magicEnergy.ChangeEnergy(args.Target.Value, effectiveDelta, out _, out _, Safe);
    }
}
