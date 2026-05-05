using Content.Server._OE14.Objectives.Components;
using Content.Server.Cargo.Systems;
using Content.Shared._OE14.Currency;
using Content.Shared._OE14.MagicEnergy;
using Content.Shared.FixedPoint;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Objectives.Components;
using Content.Shared.Objectives.Systems;

namespace Content.Server._OE14.Objectives.Systems;

public sealed class OE14CurrencyCollectConditionSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly SharedObjectivesSystem _objectives = default!;
    [Dependency] private readonly OE14SharedCurrencySystem _currency = default!;
    [Dependency] private readonly PricingSystem _price = default!;
    [Dependency] private readonly OE14SharedMagicEnergySystem _magicEnergy = default!;

    private Dictionary<EntityUid, (EntityUid MindId, int LastRewardCount)> _objectiveTracking = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14CurrencyCollectConditionComponent, ObjectiveAfterAssignEvent>(OnCollectAfterAssign);
        SubscribeLocalEvent<OE14CurrencyCollectConditionComponent, ObjectiveGetProgressEvent>(OnCollectGetProgress);
    }

    private void OnCollectAfterAssign(Entity<OE14CurrencyCollectConditionComponent> condition, ref ObjectiveAfterAssignEvent args)
    {
        _metaData.SetEntityName(condition.Owner, Loc.GetString(condition.Comp.ObjectiveText, ("coins", _currency.GetCurrencyPrettyString(condition.Comp.Currency))), args.Meta);
        _metaData.SetEntityDescription(condition.Owner, Loc.GetString(condition.Comp.ObjectiveDescription, ("coins", _currency.GetCurrencyPrettyString(condition.Comp.Currency))), args.Meta);
        _objectives.SetIcon(condition.Owner, condition.Comp.ObjectiveSprite);

        // Track this objective with the mind ID for Memory rewards
        if (args.MindId is { } mindId)
        {
            _objectiveTracking[condition.Owner] = (mindId, 0);
        }
    }

    private void OnCollectGetProgress(Entity<OE14CurrencyCollectConditionComponent> condition, ref ObjectiveGetProgressEvent args)
    {
        args.Progress = GetProgress(args.MindId, args.Mind, condition);
    }

    private float GetProgress(EntityUid mindId, MindComponent mind, OE14CurrencyCollectConditionComponent condition)
    {
        double count = 0;

        if (mind.OwnedEntity is null)
            return 0;

        count += _price.GetPrice(mind.OwnedEntity.Value);
        count -= _price.GetPrice(mind.OwnedEntity.Value, false);

        // Check if we should reward Memory (only if RewardEvery > 0, meaning this objective supports Memory)
        if (condition.RewardEvery > 0 && condition.MemoryReward > 0)
        {
            var currentCount = (int)count;
            var killsSinceLastReward = currentCount - condition.LastRewardCount;

            if (killsSinceLastReward >= condition.RewardEvery)
            {
                // Reward Memory to the player
                if (mind.OwnedEntity is { } playerEntity)
                {
                    _magicEnergy.ChangeEnergy(playerEntity,
                        (FixedPoint2)condition.MemoryReward,
                        out var _, out var _, safe: true);
                }

                condition.LastRewardCount = currentCount;
                Dirty(condition.Owner, condition);
            }
        }

        var result = count / (float)condition.Currency;
        result = Math.Clamp(result, 0, 1);
        return (float)result;
    }
}
