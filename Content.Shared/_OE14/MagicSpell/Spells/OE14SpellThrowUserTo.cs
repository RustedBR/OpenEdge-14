using Content.Shared.Throwing;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellThrowUserTo : OE14SpellEffect
{
    [DataField]
    public float ThrowPower = 10f;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Position is null || args.User is null)
            return;

        var throwing = entManager.System<ThrowingSystem>();

        throwing.TryThrow(args.User.Value, args.Position.Value, ThrowPower);
    }
}
