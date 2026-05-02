using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared.Damage;

namespace Content.Shared._OE14.MagicSpell.Spells;

/// <summary>
/// Applies damage or healing to the spell target, scaled by the caster's Intelligence stat.
/// Use this instead of OE14SpellApplyEntityEffect + HealthChange for spells that should scale with INT.
///
/// Formula (same asymmetric curve as Strength → DamageMultiplier):
///   INT 1  = 0.50x  (-50%)
///   INT 5  = 1.00x  (neutral baseline)
///   INT 10 = 1.50x  (+50%)
///
/// If the caster has no OE14CharacterStatsComponent, the damage is applied unscaled.
/// </summary>
public sealed partial class OE14SpellApplyDamageScaled : OE14SpellEffect
{
    [DataField(required: true)]
    public DamageSpecifier Damage = default!;

    [DataField]
    public bool IgnoreResistances = false;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        var scaled = new DamageSpecifier(Damage);

        if (args.User is not null &&
            entManager.TryGetComponent<OE14CharacterStatsComponent>(args.User.Value, out var stats))
        {
            scaled *= GetIntMultiplier(stats);
        }

        entManager.System<DamageableSystem>()
            .TryChangeDamage(args.Target.Value, scaled, IgnoreResistances, interruptsDoAfters: false);
    }

    private static float GetIntMultiplier(OE14CharacterStatsComponent stats)
    {
        var effInt = Math.Clamp(
            stats.Intelligence + stats.IntelligenceModifier,
            1,
            stats.MaxStatValue);

        return effInt >= 5
            ? 1.0f + (effInt - 5) * 0.10f
            : 1.0f + (effInt - 5) * 0.125f;
    }
}
