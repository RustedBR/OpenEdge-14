using Content.Shared._OE14.Dash;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellDash : OE14SpellEffect
{
    [DataField]
    public float Speed = 10f;

    [DataField]
    public float Range = 3.5f;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.User is null)
            return;
        if (args.Position is null)
            return;

        var dashSys = entManager.System<OE14DashSystem>();

        dashSys.PerformDash(args.User.Value, args.Position.Value, Speed, Range);
    }
}
