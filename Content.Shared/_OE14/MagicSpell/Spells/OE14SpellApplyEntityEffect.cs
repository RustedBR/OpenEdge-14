using Content.Shared.EntityEffects;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellApplyEntityEffect : OE14SpellEffect
{
    [DataField(required: true, serverOnly: true)]
    public List<EntityEffect> Effects = new();

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        var targetEntity = args.Target.Value;

        foreach (var effect in Effects)
        {
            effect.Effect(new EntityEffectBaseArgs(targetEntity, entManager));
        }
    }
}
