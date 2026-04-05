using Content.Shared.Stealth;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellRevealStealthUser : OE14SpellEffect
{
    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.User is null)
            return;

        var stealth = entManager.System<SharedStealthSystem>();

        stealth.SetVisibility(args.User.Value, 1);
    }
}
