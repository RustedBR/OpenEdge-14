using Content.Server._OE14.Objectives.Systems;
using Robust.Shared.Utility;

namespace Content.Server._OE14.Objectives.Components;

[RegisterComponent, Access(typeof(OE14CurrencyCollectConditionSystem))]
public sealed partial class OE14CurrencyCollectConditionComponent : Component
{
    [DataField]
    public int Currency = 1000;

    [DataField(required: true)]
    public LocId ObjectiveText;

    [DataField(required: true)]
    public LocId ObjectiveDescription;

    [DataField(required: true)]
    public SpriteSpecifier ObjectiveSprite;

    /// <summary>
    /// Amount of Memory to reward per milestone
    /// </summary>
    [DataField]
    public float MemoryReward = 0f;

    /// <summary>
    /// Currency amount between memory rewards
    /// </summary>
    [DataField]
    public int RewardEvery = 0;

    /// <summary>
    /// Last currency amount when a reward was given
    /// </summary>
    [DataField]
    public int LastRewardCount = 0;
}
