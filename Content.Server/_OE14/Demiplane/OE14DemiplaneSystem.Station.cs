using System.Linq;
using System.Numerics;
using Content.Server._OE14.Demiplane.Components;
using Content.Server.Station.Systems;
using Content.Shared._OE14.Demiplane;
using Content.Shared._OE14.Demiplane.Components;
using Content.Shared._OE14.Procedural.Prototypes;
using Content.Shared.UserInterface;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._OE14.Demiplane;

public sealed partial class OE14DemiplaneSystem
{
    [Dependency] private readonly UserInterfaceSystem _userInterface = default!;
    [Dependency] private readonly StationSystem _station = default!;

    private readonly ProtoId<OE14ProceduralModifierPrototype> _firstPointProto = "DemiplaneArcSingle";
    private readonly ProtoId<OE14ProceduralModifierPrototype> _secondPointProto = "OE14DemiplanEnterRoom";

    private void InitializeStation()
    {
        SubscribeLocalEvent<OE14StationDemiplaneMapComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<OE14DemiplaneNavigationMapComponent, BeforeActivatableUIOpenEvent>(
            OnBeforeActivatableUiOpen);
    }

    private void OnMapInit(Entity<OE14StationDemiplaneMapComponent> ent, ref MapInitEvent args)
    {
        GenerateDemiplaneMap(ent);
    }

    private void OnBeforeActivatableUiOpen(Entity<OE14DemiplaneNavigationMapComponent> ent,
        ref BeforeActivatableUIOpenEvent args)
    {
        var station = _station.GetOwningStation(ent, Transform(ent));

        if (!TryComp<OE14StationDemiplaneMapComponent>(station, out var stationMap))
            return;

        UpdateNodesStatus((station.Value, stationMap));

        _userInterface.SetUiState(ent.Owner,
            OE14DemiplaneMapUiKey.Key,
            new OE14DemiplaneMapUiState(stationMap.Nodes, stationMap.Edges));
    }

    private void UpdateNodesStatus(Entity<OE14StationDemiplaneMapComponent> ent)
    {
        var openedMaps = new List<Vector2i>();

        var query = EntityQueryEnumerator<OE14DemiplaneMapComponent>();
        while (query.MoveNext(out var uid, out var demiplane))
        {
            openedMaps.Add(demiplane.Position);
        }

        foreach (var node in ent.Comp.Nodes)
        {
            node.Value.Opened = openedMaps.Contains(node.Key);
        }
    }

    private void GenerateDemiplaneMap(Entity<OE14StationDemiplaneMapComponent> ent)
    {
        ent.Comp.Nodes.Clear();
        ent.Comp.Edges.Clear();

        var directions = new[] { new Vector2i(1, 0), new Vector2i(-1, 0), new Vector2i(0, 1), new Vector2i(0, -1) };

        // Generate village node
        var villageNode = new OE14DemiplaneMapNode(Vector2.Zero, null, null)
        {
            Start = true,
        };

        ent.Comp.Nodes.Add(Vector2i.Zero, villageNode);

        // Generate first node
        var firstNodePosition = _random.Pick(directions);
        var location = SelectLocation(0);
        var modifiers = SelectModifiers(0, location, GetLimits(0));
        var firstNode = new OE14DemiplaneMapNode(firstNodePosition, location, modifiers)
        {
            Opened = true
        };
        firstNode.Modifiers.Add(_secondPointProto);

        ent.Comp.Nodes.Add(firstNodePosition, firstNode);
        ent.Comp.Edges.Add((Vector2i.Zero, firstNodePosition));

        //In a loop, take a random existing node, find an empty spot next to it on the grid, add a new room there, and connect them.
        while (ent.Comp.Nodes.Count < ent.Comp.TotalCount)
        {
            //Get random existing node
            var randomNode = _random.Pick(ent.Comp.Nodes);

            if (randomNode.Value.Start)
                continue;

            var randomNodePosition = randomNode.Key;

            // Find a random empty adjacent position
            var emptyPositions = directions
                .Select(dir => randomNodePosition + dir)
                .Where(pos => !ent.Comp.Nodes.ContainsKey(pos))
                .ToList();

            if (emptyPositions.Count == 0)
                continue;

            var newPosition = emptyPositions[_random.Next(emptyPositions.Count)];

            // Add the new node and connect it with an edge
            var lvl = randomNode.Value.Level + 1;
            location = SelectLocation(lvl);
            modifiers = SelectModifiers(lvl, location, GetLimits(lvl));
            var newNode = new OE14DemiplaneMapNode(newPosition, location, modifiers)
            {
                Level = lvl,
            };

            ent.Comp.Nodes[newPosition] = newNode;
            ent.Comp.Edges.Add((randomNodePosition, newPosition));

            randomNode.Value.Modifiers.Add(_firstPointProto);
            newNode.Modifiers.Add(_secondPointProto);
        }

        foreach (var (position, node) in ent.Comp.Nodes)
        {
            //Random minor UI offset
            node.UiPosition += new Vector2(
                _random.NextFloat(-0.2f, 0.2f),
                _random.NextFloat(-0.2f, 0.2f));
        }
    }

    private Dictionary<ProtoId<OE14ProceduralModifierCategoryPrototype>, float> GetLimits(int level)
    {
        return new Dictionary<ProtoId<OE14ProceduralModifierCategoryPrototype>, float>
        {
            { "Danger", Math.Max(level * 0.2f, 0.5f) },
            { "GhostRoleDanger", 1f },
            { "Reward", Math.Max(level * 0.3f, 0.5f) },
            { "Ore", Math.Max(level * 0.5f, 1f) },
            { "Fun", 1f },
            { "Weather", 1f },
            { "MapLight", 1f },
        };
    }

    private Vector2i? GetRandomNeighbourNotGeneratedMap(Entity<OE14StationDemiplaneMapComponent> ent, Vector2i origin)
    {
        if (!ent.Comp.Nodes.ContainsKey(origin))
            return null;

        // First try adjacent nodes (direct neighbors)
        var paths = new List<Vector2i>();
        foreach (var edge in ent.Comp.Edges)
        {
            if (edge.Item1 == origin && !ent.Comp.GeneratedNodes.Contains(edge.Item2))
                paths.Add(edge.Item2);
        }

        if (paths.Count > 0)
            return _random.Pick(paths);

        // Fallback: BFS to find any reachable non-generated node
        var visited = new HashSet<Vector2i> { origin };
        var queue = new Queue<Vector2i>();
        queue.Enqueue(origin);
        var reachable = new List<Vector2i>();

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var edge in ent.Comp.Edges)
            {
                if (edge.Item1 != current)
                    continue;

                var next = edge.Item2;
                if (visited.Contains(next))
                    continue;

                visited.Add(next);

                if (!ent.Comp.GeneratedNodes.Contains(next))
                    reachable.Add(next);
                else
                    queue.Enqueue(next);
            }
        }

        if (reachable.Count == 0)
            return null;

        return _random.Pick(reachable);
    }
}
