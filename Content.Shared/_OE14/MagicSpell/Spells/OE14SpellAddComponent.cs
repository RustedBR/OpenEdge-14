using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellAddComponent : OE14SpellEffect
{
    [DataField]
    public ComponentRegistry Components = new();

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        entManager.AddComponents(args.Target.Value, Components);
    }
}
