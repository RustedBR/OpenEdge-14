using System.Linq;
using System.Numerics;
using Content.Server._OE14.GameTicking.Rules.Components;
using Content.Server._OE14.Procedural;
using Content.Server.GameTicking.Rules;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Systems;
using Content.Shared.GameTicking.Components;
using Content.Shared.Station.Components;
using Robust.Shared.Map;

namespace Content.Server._OE14.GameTicking.Rules;

public sealed class OE14ExpeditionToWindlandsRule : GameRuleSystem<OE14ExpeditionToWindlandsRuleComponent>
{
    [Dependency] private readonly ShuttleSystem _shuttles = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly ILogManager _logManager = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly OE14LocationGenerationSystem _generation = default!;

    private ISawmill _sawmill = default!;

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _logManager.GetSawmill("oe14_expedition_to_windlands_rule");
    }

    protected override void Started(EntityUid uid,
        OE14ExpeditionToWindlandsRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        var station = _station.GetStations().First();
        var largestStationGrid = _station.GetLargestGrid(station);

        if (largestStationGrid is null)
        {
            _sawmill.Error($"Station {station} does not have a grid.");
            return;
        }

        EnsureComp<ShuttleComponent>(largestStationGrid.Value, out var shuttleComp);

        var windlands = _mapSystem.CreateMap(out var mapId, runMapInit: true);

        _generation.GenerateLocation(windlands, mapId, component.Location, component.Modifiers);
        _shuttles.FTLToCoordinates(largestStationGrid.Value, shuttleComp, new EntityCoordinates(windlands, Vector2.Zero), 0f, 0f, component.FloatingTime);
    }
}
