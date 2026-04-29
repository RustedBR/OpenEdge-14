using Content.Server.Station.Systems;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Roles;
using Content.Shared.Station.Components;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.Jobs;

/// <summary>
/// Gives a keyring to the player at spawn if a specific job has no slots remaining on the station.
/// Used so that Apprentices receive the keys of full artisan shops.
/// </summary>
[DataDefinition]
public sealed partial class OE14GiveKeysIfJobMissingSpecial : JobSpecial
{
    /// <summary>
    /// Map of job ID → keyring entity prototype to spawn if that job is full or absent.
    /// </summary>
    [DataField(required: true)]
    public Dictionary<ProtoId<JobPrototype>, EntProtoId> JobToKeyRing { get; private set; } = new();

    public override void AfterEquip(EntityUid mob)
    {
        var entMan = IoCManager.Resolve<IEntityManager>();
        var sysMan = IoCManager.Resolve<IEntitySystemManager>();

        var transform = entMan.GetComponent<TransformComponent>(mob);
        var grid = transform.GridUid;

        if (grid is null)
            return;

        if (!entMan.TryGetComponent<StationMemberComponent>(grid.Value, out var member))
            return;

        var stationJobs = sysMan.GetEntitySystem<StationJobsSystem>();
        var hands = sysMan.GetEntitySystem<SharedHandsSystem>();
        var inventory = sysMan.GetEntitySystem<InventorySystem>();

        foreach (var (jobId, keyRingId) in JobToKeyRing)
        {
            // Job absent from this station — skip.
            if (!stationJobs.TryGetJobSlot(member.Station, jobId, out var slots))
                continue;

            // null means unlimited slots — job is available, no need to give keys.
            if (slots is null || slots > 0)
                continue;

            var keyRing = entMan.SpawnEntity(keyRingId, transform.Coordinates);

            if (!hands.TryPickupAnyHand(mob, keyRing))
                inventory.TryEquip(mob, keyRing, "pocket1", checkDoafter: false, force: true);
        }
    }
}
