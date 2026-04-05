using Content.Server._OE14.GameTicking.Rules.Components;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking.Rules;
using Content.Server.Station.Components;
using Content.Server.StationEvents.Events;
using Content.Shared._OE14.DayCycle;
using Content.Shared.GameTicking.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;

namespace Content.Server._OE14.GameTicking.Rules;

public sealed class OE14BloodMoonRule : GameRuleSystem<OE14BloodMoonRuleComponent>
{
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14StartNightEvent>(OnStartNight);
    }

    protected override void Started(EntityUid uid,
        OE14BloodMoonRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        Filter allPlayersInGame = Filter.Empty().AddWhere(GameTicker.UserHasJoinedGame);
        _chatSystem.DispatchFilteredAnnouncement(
            allPlayersInGame,
            message: Loc.GetString(component.StartAnnouncement),
            colorOverride: component.AnnouncementColor);

        _audio.PlayGlobal(component.AnnounceSound, allPlayersInGame, true);
    }

    private void OnStartNight(OE14StartNightEvent ev)
    {
        if (!HasComp<BecomesStationComponent>(ev.Map))
            return;

        var query = QueryActiveRules();
        while (query.MoveNext(out var uid, out _, out var comp, out _))
        {
            var ruleEnt = GameTicker.AddGameRule(comp.CurseRule);
            GameTicker.StartGameRule(ruleEnt);
            ForceEndSelf(uid);
        }
    }
}
