using Content.Shared._OE14.Religion.Components;
using Content.Shared._OE14.Religion.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.Verbs;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._OE14.Religion.Systems;

public abstract partial class OE14SharedReligionGodSystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;

    private void InitializeAltars()
    {
        SubscribeLocalEvent<OE14ReligionAltarComponent, GetVerbsEvent<AlternativeVerb>>(GetAltVerb);
    }

    private void GetAltVerb(Entity<OE14ReligionAltarComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (ent.Comp.Religion is null)
            return;

        var disabled = !CanBecomeFollower(args.User, ent.Comp.Religion.Value);

        if (!disabled && TryComp<OE14ReligionPendingFollowerComponent>(args.User, out var pendingFollower))
        {
            if (pendingFollower.Religion is not null)
                disabled = true;
        }

        if (disabled)
            return;

        var user = args.User;
        args.Verbs.Add(new AlternativeVerb()
        {
            Text = Loc.GetString("oe14-altar-become-follower"),
            Message = Loc.GetString("oe14-altar-become-follower-desc"),
            Act = () =>
            {
                var doAfterArgs = new DoAfterArgs(EntityManager, user, 5f, new OE14AltarOfferDoAfter(), ent, used: ent)
                {
                    BreakOnDamage = true,
                    BreakOnMove = true,
                };
                _doAfter.TryStartDoAfter(doAfterArgs);
            },
        });
    }
}

[Serializable, NetSerializable]
public sealed partial class OE14AltarOfferDoAfter : SimpleDoAfterEvent
{
}
