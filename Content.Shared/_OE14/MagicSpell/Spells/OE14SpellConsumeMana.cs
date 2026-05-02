using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared._OE14.MagicEnergy;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared.FixedPoint;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellConsumeManaEffect : OE14SpellEffect
{
    [DataField]
    public FixedPoint2 Mana = 0;

    [DataField]
    public bool Safe = false;

    /// <summary>
    /// When true, multiplies Mana transferred by the caster's INT multiplier.
    /// INT 5 = 1.0x, INT 10 = 1.5x, INT 1 = 0.5x.
    /// </summary>
    [DataField]
    public bool ScaleWithCasterInt = false;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        var targetEntity = args.Target.Value;

        if (!entManager.HasComponent<OE14MagicEnergyContainerComponent>(targetEntity))
            return;

        var magicEnergy = entManager.System<OE14SharedMagicEnergySystem>();

        var effectiveMana = Mana;

        if (ScaleWithCasterInt &&
            args.User is not null &&
            entManager.TryGetComponent<OE14CharacterStatsComponent>(args.User.Value, out var stats))
        {
            var effInt = Math.Clamp(stats.Intelligence + stats.IntelligenceModifier, 1, stats.MaxStatValue);
            var multiplier = effInt >= 5
                ? 1.0f + (effInt - 5) * 0.10f
                : 1.0f + (effInt - 5) * 0.125f;
            effectiveMana = FixedPoint2.New((float)Mana * multiplier);
        }

        //First - used object
        if (args.Used is not null)
        {
            magicEnergy.TransferEnergy(targetEntity,
                args.Used.Value,
                effectiveMana,
                out _,
                out _,
                safe: Safe);
            return;
        }

        //Second - player
        if (args.User is not null)
        {
            magicEnergy.TransferEnergy(targetEntity,
                args.User.Value,
                effectiveMana,
                out _,
                out _,
                safe: Safe);
            return;
        }
    }
}
