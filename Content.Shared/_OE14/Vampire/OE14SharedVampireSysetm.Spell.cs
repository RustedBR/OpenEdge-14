using Content.Shared._OE14.MagicSpell.Events;
using Content.Shared._OE14.Vampire.Components;
using Content.Shared.Actions.Events;
using Content.Shared.Examine;
using Content.Shared.Mobs.Systems;
using Content.Shared.SSDIndicator;

namespace Content.Shared._OE14.Vampire;

public abstract partial class OE14SharedVampireSystem
{
    private void InitializeSpell()
    {
        SubscribeLocalEvent<OE14MagicEffectVampireComponent, ActionAttemptEvent>(OnVampireCastAttempt);
        SubscribeLocalEvent<OE14MagicEffectVampireComponent, ExaminedEvent>(OnVampireCastExamine);
    }

    private void OnVampireCastAttempt(Entity<OE14MagicEffectVampireComponent> ent, ref ActionAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        //If we are not vampires in principle, we certainly should not have this ability,
        //but then we will not limit its use to a valid vampire form that is unavailable to us.

        if (!HasComp<OE14VampireComponent>(args.User))
            return;

        if (!HasComp<OE14VampireVisualsComponent>(args.User))
        {
            _popup.PopupClient(Loc.GetString("oe14-magic-spell-need-vampire-valid"), args.User, args.User);
            args.Cancelled = true;
        }
    }

    private void OnVampireCastExamine(Entity<OE14MagicEffectVampireComponent> ent, ref ExaminedEvent args)
    {
        args.PushMarkup($"{Loc.GetString("oe14-magic-spell-need-vampire-valid")}");
    }
}
