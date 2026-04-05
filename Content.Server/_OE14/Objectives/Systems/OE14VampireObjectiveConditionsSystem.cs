using Content.Server._OE14.Objectives.Components;
using Content.Server.Station.Components;
using Content.Shared._OE14.Vampire.Components;
using Content.Shared.Mind;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Objectives.Components;
using Content.Shared.Objectives.Systems;
using Content.Shared.Roles.Jobs;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.Objectives.Systems;

public sealed class OE14VampireObjectiveConditionsSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _meta = default!;
    [Dependency] private readonly SharedObjectivesSystem _objectives = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedJobSystem _jobs = default!;

    public readonly float RequiredAlivePercentage = 0.5f;
    public readonly int RequiredHeartLevel = 3;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14VampireBloodPurityConditionComponent, ObjectiveAfterAssignEvent>(OnBloodPurityAfterAssign);
        SubscribeLocalEvent<OE14VampireBloodPurityConditionComponent, ObjectiveGetProgressEvent>(OnBloodPurityGetProgress);

        SubscribeLocalEvent<OE14VampireDefenceVillageConditionComponent, ObjectiveAfterAssignEvent>(OnDefenceAfterAssign);
        SubscribeLocalEvent<OE14VampireDefenceVillageConditionComponent, ObjectiveGetProgressEvent>(OnDefenceGetProgress);
    }

    private void OnDefenceAfterAssign(Entity<OE14VampireDefenceVillageConditionComponent> ent, ref ObjectiveAfterAssignEvent args)
    {
        _meta.SetEntityName(ent, Loc.GetString("oe14-objective-vampire-defence-settlement-title"));
        _meta.SetEntityDescription(ent, Loc.GetString("oe14-objective-vampire-defence-settlement-desc", ("count", RequiredAlivePercentage * 100)));
        _objectives.SetIcon(ent, ent.Comp.Icon);
    }

    public float CalculateAlivePlayersPercentage()
    {
        var query = EntityQueryEnumerator<StationJobsComponent>();

        var totalPlayers = 0f;
        var alivePlayers = 0f;
        while (query.MoveNext(out var uid, out var jobs))
        {
            totalPlayers += jobs.PlayerJobs.Count;

            foreach (var (netUserId, jobsList) in jobs.PlayerJobs)
            {
                if (!_mind.TryGetMind(netUserId, out var mind))
                    continue;

                if (!_jobs.MindTryGetJob(mind, out var jobRole))
                    continue;

                var firstMindEntity = GetEntity(mind.Value.Comp.OriginalOwnedEntity);

                if (firstMindEntity is null)
                    continue;

                if (!_mobState.IsDead(firstMindEntity.Value))
                    alivePlayers++;
            }
        }

        return totalPlayers > 0 ? (alivePlayers / totalPlayers) : 0f;
    }

    private void OnDefenceGetProgress(Entity<OE14VampireDefenceVillageConditionComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        args.Progress = CalculateAlivePlayersPercentage() > RequiredAlivePercentage ? 1 : 0;
    }

    private void OnBloodPurityAfterAssign(Entity<OE14VampireBloodPurityConditionComponent> ent, ref ObjectiveAfterAssignEvent args)
    {
         if (!TryComp<OE14VampireComponent>(args.Mind?.OwnedEntity, out var vampireComp))
             return;

         ent.Comp.Faction = vampireComp.Faction;

         _meta.SetEntityName(ent, Loc.GetString("oe14-objective-vampire-pure-blood-title"));
         _meta.SetEntityDescription(ent, Loc.GetString("oe14-objective-vampire-pure-blood-desc"));
         _objectives.SetIcon(ent, ent.Comp.Icon);
    }

    private void OnBloodPurityGetProgress(Entity<OE14VampireBloodPurityConditionComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        var query = EntityQueryEnumerator<OE14VampireClanHeartComponent>();

        var ourHeartReady = false;
        var othersHeartsExist = false;
        while (query.MoveNext(out var uid, out var vampire))
        {
            if (vampire.Faction == ent.Comp.Faction && vampire.Level >= RequiredHeartLevel)
                ourHeartReady = true;

            if (vampire.Faction != ent.Comp.Faction && vampire.Level >= RequiredHeartLevel)
            {
                othersHeartsExist = true;
                break;
            }
        }

        var progress = 0f;

        if (ourHeartReady)
            progress += 0.5f;

        if (!othersHeartsExist)
            progress += 0.5f;

        args.Progress = progress;
    }
}
