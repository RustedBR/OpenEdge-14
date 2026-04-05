using Content.Shared.StatusEffectNew;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellToggleStatusEffect : OE14SpellEffect
{
    [DataField(required: true)]
    public EntProtoId StatusEffect = default;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        var effectSys = entManager.System<StatusEffectsSystem>();

        if (!effectSys.HasStatusEffect(args.Target.Value, StatusEffect))
            effectSys.TrySetStatusEffectDuration(args.Target.Value, StatusEffect);
        else
            effectSys.TryRemoveStatusEffect(args.Target.Value, StatusEffect);

    }
}
