using Content.Shared._OE14.Religion.Components;
using Content.Shared._OE14.Religion.Systems;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellGodRenounce : OE14SpellEffect
{
    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        if (!entManager.TryGetComponent<OE14ReligionEntityComponent>(args.User, out var god) || god.Religion is null)
            return;

        if (!entManager.TryGetComponent<OE14ReligionFollowerComponent>(args.Target.Value, out var follower) || follower.Religion != god.Religion)
            return;

        var religionSys = entManager.System<OE14SharedReligionGodSystem>();

        religionSys.ToDisbelieve(args.Target.Value);
    }
}
