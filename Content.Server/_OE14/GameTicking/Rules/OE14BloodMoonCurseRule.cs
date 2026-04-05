using System.Linq;
using Content.Server._OE14.GameTicking.Rules.Components;
using Content.Server.Antag;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking.Rules;
using Content.Server.Popups;
using Content.Server.Station.Components;
using Content.Server.Stunnable;
using Content.Shared._OE14.BloodMoon;
using Content.Shared._OE14.DayCycle;
using Content.Shared.Actions;
using Content.Shared.Examine;
using Content.Shared.GameTicking.Components;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;

namespace Content.Server._OE14.GameTicking.Rules;

public sealed class OE14BloodMoonCurseRule : GameRuleSystem<OE14BloodMoonCurseRuleComponent>
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly StunSystem _stun = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedActionsSystem _action = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14StartDayEvent>(OnStartDay);
        SubscribeLocalEvent<OE14BloodMoonCurseRuleComponent, AfterAntagEntitySelectedEvent>(AfterAntagEntitySelected);
        SubscribeLocalEvent<OE14BloodMoonCurseComponent, ExaminedEvent>(CurseExamined);
    }

    private void CurseExamined(Entity<OE14BloodMoonCurseComponent> ent, ref ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString("oe14-bloodmoon-curse-examined"));
    }

    private void AfterAntagEntitySelected(Entity<OE14BloodMoonCurseRuleComponent> ent, ref AfterAntagEntitySelectedEvent args)
    {
        SpawnAttachedTo(ent.Comp.CurseEffect, Transform(args.EntityUid).Coordinates);
        var curseComp = EnsureComp<OE14BloodMoonCurseComponent>(args.EntityUid);
        var effect = SpawnAttachedTo(curseComp.CurseEffect, Transform(args.EntityUid).Coordinates);
        curseComp.SpawnedEffect = effect;
        curseComp.CurseRule = ent;
        _transform.SetParent(effect, args.EntityUid);
        _action.AddAction(args.EntityUid, ref curseComp.ActionEntity, curseComp.Action);
    }

    protected override void Started(EntityUid uid,
        OE14BloodMoonCurseRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleStartedEvent args)
    {
        Filter allPlayersInGame = Filter.Empty().AddWhere(GameTicker.UserHasJoinedGame);
        _chatSystem.DispatchFilteredAnnouncement(allPlayersInGame, Loc.GetString(component.StartAnnouncement), colorOverride: component.AnnouncementColor);

        _audio.PlayGlobal(component.GlobalSound, allPlayersInGame, true);
    }

    protected override void Ended(EntityUid uid,
        OE14BloodMoonCurseRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleEndedEvent args)
    {
        Filter allPlayersInGame = Filter.Empty().AddWhere(GameTicker.UserHasJoinedGame);
        _chatSystem.DispatchFilteredAnnouncement(allPlayersInGame, Loc.GetString(component.EndAnnouncement), colorOverride: component.AnnouncementColor);

        var aliveAntags = _antag.GetAliveAntags(uid);

        foreach (var antag in aliveAntags)
        {
            SpawnAttachedTo(component.CurseEffect, Transform(antag).Coordinates);
            ClearCurse(antag);
        }

        GameTicker.EndRound();
    }

    private void OnStartDay(OE14StartDayEvent ev)
    {
        if (!HasComp<BecomesStationComponent>(ev.Map))
            return;

        var query = QueryActiveRules();
        while (query.MoveNext(out var uid, out _, out var comp, out _))
        {
            ForceEndSelf(uid);
        }
    }

    private void ClearCurse(Entity<OE14BloodMoonCurseComponent?> ent)
    {
        if (!Resolve(ent.Owner, ref ent.Comp, false))
            return;

        _stun.TryUpdateParalyzeDuration(ent, ent.Comp.EndStunDuration);
        _popup.PopupEntity(Loc.GetString("oe14-bloodmoon-curse-removed"), ent, PopupType.SmallCaution);
        if (TryComp<OE14BloodMoonCurseComponent>(ent, out var curseComp))
        {
            QueueDel(curseComp.SpawnedEffect);
            RemCompDeferred<OE14BloodMoonCurseComponent>(ent);
        }

        _action.RemoveAction(ent.Comp.ActionEntity);
    }
}
