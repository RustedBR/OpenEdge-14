using Content.Server._OE14.Objectives.Components;
using Content.Shared._OE14.MagicEnergy;
using Content.Shared.FixedPoint;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Objectives.Components;
using Content.Shared.Objectives.Systems;

namespace Content.Server._OE14.Objectives.Systems;

public sealed class OE14DefeatEnemiesConditionSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly SharedObjectivesSystem _objectives = default!;
    [Dependency] private readonly OE14SharedMagicEnergySystem _magicEnergy = default!;

    private Dictionary<EntityUid, (EntityUid MindId, int CurrentCount, int LastRewardCount)> _objectiveTracking = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14DefeatEnemiesConditionComponent, ObjectiveAfterAssignEvent>(OnDefeatAfterAssign);
        SubscribeLocalEvent<OE14DefeatEnemiesConditionComponent, ObjectiveGetProgressEvent>(OnDefeatGetProgress);
        SubscribeLocalEvent<MobStateChangedEvent>(OnMobStateChanged);
    }

    private void OnDefeatAfterAssign(Entity<OE14DefeatEnemiesConditionComponent> condition, ref ObjectiveAfterAssignEvent args)
    {
        _metaData.SetEntityName(condition.Owner,
            Loc.GetString(condition.Comp.ObjectiveText, ("count", condition.Comp.TargetCount)),
            args.Meta);
        _metaData.SetEntityDescription(condition.Owner,
            Loc.GetString(condition.Comp.ObjectiveDescription, ("count", condition.Comp.TargetCount)),
            args.Meta);
        _objectives.SetIcon(condition.Owner, condition.Comp.ObjectiveSprite);

        // Track this objective with the mind ID
        if (args.MindId is { } mindId)
        {
            _objectiveTracking[condition.Owner] = (mindId, 0, 0);
        }
    }

    private void OnDefeatGetProgress(Entity<OE14DefeatEnemiesConditionComponent> condition, ref ObjectiveGetProgressEvent args)
    {
        var progress = condition.Comp.CurrentCount / (float)condition.Comp.TargetCount;
        args.Progress = Math.Clamp(progress, 0, 1);
    }

    private void OnMobStateChanged(ref MobStateChangedEvent ev)
    {
        // Only care about entities becoming dead
        if (ev.NewMobState != MobState.Dead)
            return;

        // Only care if there's an origin (the attacker)
        if (ev.Origin is not { } origin)
            return;

        // Find the mind of the attacker via MindContainerComponent
        if (!TryComp<MindContainerComponent>(origin, out var mindContainer) || mindContainer.Mind is not { } attackerMindId)
            return;

        // Find all objectives for this attacker
        var query = EntityQueryEnumerator<OE14DefeatEnemiesConditionComponent>();
        while (query.MoveNext(out var objEnt, out var defeatCondition))
        {
            // Check if this objective belongs to this attacker
            if (!_objectiveTracking.TryGetValue(objEnt, out var tracking))
                continue;

            if (tracking.MindId != attackerMindId)
                continue;

            // Increment kill count
            defeatCondition.CurrentCount++;

            // Check if we should reward Memory
            var killsSinceLastReward = defeatCondition.CurrentCount - tracking.LastRewardCount;
            if (killsSinceLastReward >= defeatCondition.RewardEvery)
            {
                // Try to give the player Memory reward
                _magicEnergy.ChangeEnergy(origin,
                    (FixedPoint2)defeatCondition.MemoryReward,
                    out var _, out var _, safe: true);

                // Update tracking
                tracking = (tracking.MindId, defeatCondition.CurrentCount, defeatCondition.CurrentCount);
                _objectiveTracking[objEnt] = tracking;
            }
            else
            {
                // Just update the current count in tracking
                tracking = (tracking.MindId, defeatCondition.CurrentCount, tracking.LastRewardCount);
                _objectiveTracking[objEnt] = tracking;
            }

            Dirty(objEnt, defeatCondition);
        }
    }
}
