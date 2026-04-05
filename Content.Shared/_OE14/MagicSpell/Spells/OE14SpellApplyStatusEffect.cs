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

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        var effectSys = entManager.System<StatusEffectsSystem>();

        if (!Refresh)
            effectSys.TryAddStatusEffectDuration(args.Target.Value, StatusEffect, Duration);
        else
            effectSys.TrySetStatusEffectDuration(args.Target.Value, StatusEffect, Duration);
    }
}
