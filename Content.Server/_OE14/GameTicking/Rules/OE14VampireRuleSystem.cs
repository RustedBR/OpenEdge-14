using System.Linq;
using Content.Server._OE14.GameTicking.Rules.Components;
using Content.Server._OE14.Objectives.Systems;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Shared._OE14.Vampire;
using Content.Shared._OE14.Vampire.Components;
using Content.Shared.GameTicking.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.GameTicking.Rules;

public sealed class OE14VampireRuleSystem : GameRuleSystem<OE14VampireRuleComponent>
{
    [Dependency] private readonly OE14VampireObjectiveConditionsSystem _condition = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    protected override void AppendRoundEndText(EntityUid uid,
        OE14VampireRuleComponent component,
        GameRuleComponent gameRule,
        ref RoundEndTextAppendEvent args)
    {
        base.AppendRoundEndText(uid, component, gameRule, ref args);

        //Alive percentage
        var alivePercentage = _condition.CalculateAlivePlayersPercentage();

        var aliveFactions = new HashSet<ProtoId<OE14VampireFactionPrototype>>();

        var query = EntityQueryEnumerator<OE14VampireClanHeartComponent>();
        while (query.MoveNext(out _, out var heart))
        {
            if (heart.Faction is null)
                continue;

            if (heart.Level < _condition.RequiredHeartLevel)
                continue;

            aliveFactions.Add(heart.Faction.Value);
        }

        args.AddLine($"[head=2][color=#ab1b3d]{Loc.GetString("oe14-vampire-clans-battle")}[/color][/head]");

        if (alivePercentage > _condition.RequiredAlivePercentage)
        {
            if (aliveFactions.Count == 0)
            {
                //City win
                args.AddLine($"[head=3][color=#7d112b]{Loc.GetString("oe14-vampire-clans-battle-clan-city-win")}[/color][/head]");
                args.AddLine(Loc.GetString("oe14-vampire-clans-battle-clan-city-win-desc"));
            }

            if (aliveFactions.Count == 1)
            {
                var faction = aliveFactions.First();

                if (_proto.TryIndex(faction, out var indexedFaction))
                    args.AddLine($"[head=3][color=#7d112b]{Loc.GetString("oe14-vampire-clans-battle-clan-win", ("name", Loc.GetString(indexedFaction.Name)))}[/color][/head]");
                args.AddLine(Loc.GetString("oe14-vampire-clans-battle-clan-win-desc"));
            }

            if (aliveFactions.Count == 2)
            {
                var factions = aliveFactions.ToArray();

                if (_proto.TryIndex(factions[0], out var indexedFaction1) && _proto.TryIndex(factions[1], out var indexedFaction2))
                    args.AddLine($"[head=3][color=#7d112b]{Loc.GetString("oe14-vampire-clans-battle-clan-tie-2", ("name1", Loc.GetString(indexedFaction1.Name)), ("name2", Loc.GetString(indexedFaction2.Name)))}[/color][/head]");
                args.AddLine(Loc.GetString("oe14-vampire-clans-battle-clan-tie-2-desc"));
            }

            if (aliveFactions.Count == 3)
            {
                args.AddLine($"[head=3][color=#7d112b]{Loc.GetString("oe14-vampire-clans-battle-clan-tie-3")}[/color][/head]");
                args.AddLine(Loc.GetString("oe14-vampire-clans-battle-clan-tie-3-desc"));
            }
        }
        else
        {
            args.AddLine($"[head=3][color=#7d112b]{Loc.GetString("oe14-vampire-clans-battle-clan-lose")}[/color][/head]");
            args.AddLine(Loc.GetString("oe14-vampire-clans-battle-clan-lose-desc"));
        }

        args.AddLine(Loc.GetString("oe14-vampire-clans-battle-alive-people", ("percent", MathF.Round(alivePercentage * 100))));
    }
}
