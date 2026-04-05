using System.Linq;
using Content.Shared._OE14.Farming.Components;
using Content.Shared.DoAfter;
using Content.Shared.EntityTable;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Timing;

namespace Content.Shared._OE14.Farming;

public abstract partial class OE14SharedFarmingSystem
{
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly EntityTableSystem _entityTable = default!;
    [Dependency] private readonly TurfSystem _turf = default!;

    private void InitializeInteractions()
    {
        SubscribeLocalEvent<OE14SeedComponent, AfterInteractEvent>(OnSeedInteract);
        SubscribeLocalEvent<OE14PlantGatherableComponent, InteractUsingEvent>(OnActivate);
        SubscribeLocalEvent<OE14PlantGatherableComponent, OE14PlantGatherDoAfterEvent>(OnGatherDoAfter);

        SubscribeLocalEvent<OE14SeedComponent, OE14PlantSeedDoAfterEvent>(OnSeedPlantedDoAfter);
    }

    private void OnActivate(Entity<OE14PlantGatherableComponent> gatherable, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (_whitelist.IsWhitelistFailOrNull(gatherable.Comp.ToolWhitelist, args.Used))
            return;

        TryHarvestPlant(gatherable, args.Used, args.User);
        args.Handled = true;
    }

    private bool CanHarvestPlant(Entity<OE14PlantGatherableComponent> gatherable)
    {
        if (PlantQuery.TryComp(gatherable, out var plant))
        {
            if (plant.GrowthLevel < gatherable.Comp.GrowthLevelToHarvest)
                return false;
        }

        if (gatherable.Comp.Loot == null)
            return false;

        return true;
    }

    private bool TryHarvestPlant(Entity<OE14PlantGatherableComponent> gatherable,
        EntityUid used,
        EntityUid user)
    {
        if (!CanHarvestPlant(gatherable))
            return false;

        _audio.PlayPvs(gatherable.Comp.GatherSound, Transform(gatherable).Coordinates);

        var doAfterArgs =
            new DoAfterArgs(EntityManager,
                user,
                gatherable.Comp.GatherDelay,
                new OE14PlantGatherDoAfterEvent(),
                gatherable,
                used: used)
            {
                BreakOnDamage = true,
                BlockDuplicate = false,
                CancelDuplicate = false,
                BreakOnMove = true,
                BreakOnHandChange = true,
            };

        _doAfter.TryStartDoAfter(doAfterArgs);

        return true;
    }

    private void OnGatherDoAfter(Entity<OE14PlantGatherableComponent> gatherable, ref OE14PlantGatherDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        if (!CanHarvestPlant(gatherable))
            return;

        args.Handled = true;

        HarvestPlant(gatherable, out _, args.Used);
    }

    public void HarvestPlant(Entity<OE14PlantGatherableComponent> gatherable,
        out HashSet<EntityUid> result,
        EntityUid? used)
    {
        result = new();

        if (_net.IsClient)
            return;

        if (gatherable.Comp.Loot == null)
            return;

        var pos = _transform.GetMapCoordinates(gatherable);

        foreach (var (tag, table) in gatherable.Comp.Loot)
        {
            if (tag != "All")
            {
                if (used != null && !_tag.HasTag(used.Value, tag))
                    continue;
            }

            var spawnLoot = _entityTable.GetSpawns(table);
            foreach (var loot in spawnLoot)
            {
                var spawnPos = pos.Offset(_random.NextVector2(gatherable.Comp.GatherOffset));
                result.Add(Spawn(loot, spawnPos));
            }
        }

        _audio.PlayPvs(gatherable.Comp.GatherSound, Transform(gatherable).Coordinates);

        if (gatherable.Comp.DeleteAfterHarvest)
            _destructible.DestroyEntity(gatherable);
        else
        {
            if (PlantQuery.TryComp(gatherable, out var plant))
            {
                AffectGrowth((gatherable, plant), -gatherable.Comp.GrowthCostHarvest);
            }
        }
    }

    private void OnSeedInteract(Entity<OE14SeedComponent> seed, ref AfterInteractEvent args)
    {
        if (args.Handled || !args.CanReach)
            return;

        if (!CanPlantSeed(seed, args.ClickLocation, args.User))
            return;

        var doAfterArgs =
            new DoAfterArgs(EntityManager,
                args.User,
                seed.Comp.PlantingTime,
                new OE14PlantSeedDoAfterEvent(GetNetCoordinates(args.ClickLocation)),
                seed)
            {
                BreakOnDamage = true,
                BlockDuplicate = false,
                CancelDuplicate = false,
                BreakOnMove = true,
                BreakOnHandChange = true,
            };
        _doAfter.TryStartDoAfter(doAfterArgs);

        args.Handled = true;
    }

    private void OnSeedPlantedDoAfter(Entity<OE14SeedComponent> ent, ref OE14PlantSeedDoAfterEvent args)
    {
        if (_net.IsClient || args.Handled || args.Cancelled)
            return;

        var position = GetCoordinates(args.Coordinates);
        if (!CanPlantSeed(ent, position, args.User))
            return;

        args.Handled = true;

        Spawn(ent.Comp.PlantProto, position);
        QueueDel(ent);
    }

    public bool CanPlantSeed(Entity<OE14SeedComponent> seed, EntityCoordinates position, EntityUid? user)
    {
        var map = _transform.GetMap(position);
        if (!TryComp<MapGridComponent>(map, out var gridComp))
            return false;

        var tileRef = _map.GetTileRef(map.Value, gridComp, position);

        var tile = _turf.GetContentTileDefinition(tileRef);

        if (!seed.Comp.SoilTile.Contains(tile))
        {
            if (user is not null && _timing.IsFirstTimePredicted && _net.IsClient)
            {
                _popup.PopupEntity(Loc.GetString("oe14-farming-soil-wrong", ("seed", MetaData(seed).EntityName)),
                    user.Value,
                    user.Value);
            }

            return false;
        }

        foreach (var anchored in _map.GetAnchoredEntities((map.Value, gridComp), position))
        {
            if (PlantQuery.TryComp(anchored, out var plant))
            {
                if (user is not null && _timing.IsFirstTimePredicted && _net.IsClient)
                    _popup.PopupEntity(Loc.GetString("oe14-farming-soil-occupied"), user.Value, user.Value);
                return false;
            }
        }

        return true;
    }
}

