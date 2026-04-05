using Content.Shared._OE14.MagicSpell.Spells;
using Content.Shared._OE14.Religion.Components;
using Content.Shared._OE14.Religion.Prototypes;
using Content.Shared.Actions;
using Content.Shared.Alert;
using Content.Shared.Follower;
using Content.Shared.Mind;
using Content.Shared.Mobs;
using Content.Shared.Verbs;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Religion.Systems;

public abstract partial class OE14SharedReligionGodSystem
{
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] protected readonly SharedMindSystem Mind = default!;
    [Dependency] private readonly FollowerSystem _follower = default!;

    [ValidatePrototypeId<AlertPrototype>]
    public const string AlertProto = "OE14DivineOffer";

    private void InitializeFollowers()
    {
        SubscribeLocalEvent<OE14ReligionPendingFollowerComponent, MapInitEvent>(OnPendingFollowerInit);
        SubscribeLocalEvent<OE14ReligionPendingFollowerComponent, ComponentShutdown>(OnPendingFollowerShutdown);
        SubscribeLocalEvent<OE14ReligionPendingFollowerComponent, OE14BreakDivineOfferEvent>(OnBreakDivineOffer);
        SubscribeLocalEvent<OE14ReligionPendingFollowerComponent, OE14GodTouchEvent>(OnGodTouch);

        SubscribeLocalEvent<OE14ReligionAltarComponent, OE14AltarOfferDoAfter>(OnOfferDoAfter);

        SubscribeLocalEvent<OE14ReligionFollowerComponent, OE14RenounceFromGodEvent>(OnRenounceFromGod);
        SubscribeLocalEvent<OE14ReligionFollowerComponent, GetVerbsEvent<AlternativeVerb>>(OnGetAltVerbs);
        SubscribeLocalEvent<OE14ReligionFollowerComponent, MobStateChangedEvent>(OnFollowerStateChange);
    }

    private void OnFollowerStateChange(Entity<OE14ReligionFollowerComponent> ent, ref MobStateChangedEvent args)
    {
        if (ent.Comp.Religion is null)
            return;

        EnsureComp<OE14ReligionObserverComponent>(ent, out var observer);

        if (!_proto.TryIndex(observer.Religion, out var indexedReligion))
            return;

        var baseObservation = indexedReligion.FollowerObservationRadius;
        switch (args.NewMobState)
        {
            case MobState.Alive:
                observer.Radius = baseObservation;
                break;

            case MobState.Critical:
                SendMessageToGods(ent.Comp.Religion.Value, Loc.GetString("oe14-critical-follower-message", ("name", MetaData(ent).EntityName)), ent);
                observer.Radius = baseObservation * 0.25f;
                break;

            case MobState.Dead:
                SendMessageToGods(ent.Comp.Religion.Value, Loc.GetString("oe14-dead-follower-message", ("name", MetaData(ent).EntityName)), ent);
                observer.Radius = 0f;
                break;
        }

        Dirty(ent.Owner, observer);
    }

    private void OnGetAltVerbs(EntityUid uid, OE14ReligionFollowerComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!TryComp<OE14ReligionEntityComponent>(args.User, out var god))
            return;

        if (god.Religion != component.Religion)
            return;

        args.Verbs.Add(new AlternativeVerb
        {
            Text = Loc.GetString("admin-player-actions-follow"),
            Act = () =>
            {
                _follower.StartFollowingEntity(args.User, uid);
            },
        });
    }

    private void OnRenounceFromGod(Entity<OE14ReligionFollowerComponent> ent, ref OE14RenounceFromGodEvent args)
    {
        ToDisbelieve(ent);
    }

    private void OnOfferDoAfter(Entity<OE14ReligionAltarComponent> ent, ref OE14AltarOfferDoAfter args)
    {
        if (args.Handled || args.Cancelled)
            return;

        if (ent.Comp.Religion is null)
            return;

        TryAddPendingFollower(args.User, ent.Comp.Religion.Value);

        args.Handled = true;
    }

    private void OnGodTouch(Entity<OE14ReligionPendingFollowerComponent> ent, ref OE14GodTouchEvent args)
    {
        if (args.Religion != ent.Comp.Religion)
            return;

        TryToBelieve(ent);
    }

    private void OnBreakDivineOffer(Entity<OE14ReligionPendingFollowerComponent> ent, ref OE14BreakDivineOfferEvent args)
    {
        RemCompDeferred<OE14ReligionPendingFollowerComponent>(ent);

        if (ent.Comp.Religion is null)
            return;

        SendMessageToGods(ent.Comp.Religion.Value, Loc.GetString("oe14-unoffer-soul-god-message", ("name", MetaData(ent).EntityName)), ent);
    }

    private void OnPendingFollowerInit(Entity<OE14ReligionPendingFollowerComponent> ent, ref MapInitEvent args)
    {
        _alerts.ShowAlert(ent.Owner, AlertProto);
    }

    private void OnPendingFollowerShutdown(Entity<OE14ReligionPendingFollowerComponent> ent, ref ComponentShutdown args)
    {
        _alerts.ClearAlert(ent.Owner, AlertProto);
    }

    private bool CanBecomeFollower(EntityUid target, ProtoId<OE14ReligionPrototype> religion)
    {
        if (HasComp<OE14ReligionEntityComponent>(target))
            return false;

        EnsureComp<OE14ReligionFollowerComponent>(target, out var follower);

        if (follower.Religion is not null)
            return false;

        return !follower.RejectedReligions.Contains(religion);
    }

    private void TryAddPendingFollower(EntityUid target, ProtoId<OE14ReligionPrototype> religion)
    {
        if (!CanBecomeFollower(target, religion))
            return;

        EnsureComp<OE14ReligionPendingFollowerComponent>(target, out var pendingFollower);
        pendingFollower.Religion = religion;

        SendMessageToGods(religion, Loc.GetString("oe14-offer-soul-god-message", ("name", MetaData(target).EntityName)), target);
    }

    private bool TryToBelieve(Entity<OE14ReligionPendingFollowerComponent> pending)
    {
        if (pending.Comp.Religion is null)
            return false;

        if (!_proto.TryIndex(pending.Comp.Religion, out var indexedReligion))
            return false;

        if (!CanBecomeFollower(pending, pending.Comp.Religion.Value))
            return false;

        EnsureComp<OE14ReligionFollowerComponent>(pending, out var follower);
        EnsureComp<OE14ReligionObserverComponent>(pending, out var observer);
        observer.Religion = indexedReligion;
        observer.Radius = indexedReligion.FollowerObservationRadius;

        var oldReligion = follower.Religion;
        follower.Religion = pending.Comp.Religion;
        Dirty(pending, follower);
        Dirty(pending, observer);

        var ev = new OE14ReligionChangedEvent(oldReligion, pending.Comp.Religion);
        RaiseLocalEvent(pending, ev);

        RemCompDeferred<OE14ReligionPendingFollowerComponent>(pending);
        SendMessageToGods(pending.Comp.Religion.Value, Loc.GetString("oe14-become-follower-message", ("name", MetaData(pending).EntityName)), pending);

        _actions.AddAction(pending, ref follower.RenounceAction, follower.RenounceActionProto);
        _actions.AddAction(pending, ref follower.AppealAction, follower.AppealToGofProto);
        return true;
    }

    public void ToDisbelieve(EntityUid target)
    {
        if (!TryComp<OE14ReligionFollowerComponent>(target, out var follower))
            return;

        if (follower.Religion is null)
            return;

        RemCompDeferred<OE14ReligionObserverComponent>(target);

        SendMessageToGods(follower.Religion.Value, Loc.GetString("oe14-remove-follower-message", ("name", MetaData(target).EntityName)), target);

        var oldReligion = follower.Religion;
        follower.Religion = null;
        if (oldReligion is not null)
            follower.RejectedReligions.Add(oldReligion.Value);

        var ev = new OE14ReligionChangedEvent(oldReligion, null);
        RaiseLocalEvent(target, ev);

        Dirty(target, follower);

        _actions.RemoveAction(target, follower.RenounceAction);
        _actions.RemoveAction(target, follower.AppealAction);
    }
}

public sealed partial class OE14BreakDivineOfferEvent : BaseAlertEvent;

public sealed partial class OE14RenounceFromGodEvent : InstantActionEvent;
