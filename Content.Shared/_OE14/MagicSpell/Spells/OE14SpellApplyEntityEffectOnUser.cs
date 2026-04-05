using Content.Shared.EntityEffects;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellApplyEntityEffectOnUser : OE14SpellEffect
{
    [DataField(required: true, serverOnly: true)]
    public List<EntityEffect> Effects = new();

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.User == null)
            return;

        foreach (var effect in Effects)
        {
            effect.Effect(new EntityEffectBaseArgs(args.User.Value, entManager));
        }
    }
}
