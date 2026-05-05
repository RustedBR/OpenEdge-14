using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;

namespace Content.Server._OE14.Objectives.Components;

[RegisterComponent]
public sealed partial class OE14DefeatEnemiesConditionComponent : Component
{
    /// <summary>
    /// Number of enemies to defeat
    /// </summary>
    [DataField]
    public int TargetCount = 5;

    /// <summary>
    /// Current number of enemies defeated
    /// </summary>
    [DataField]
    public int CurrentCount = 0;

    [DataField]
    public string ObjectiveText = "oe14-objective-personal-defeat-enemies-title";

    [DataField]
    public string ObjectiveDescription = "oe14-objective-personal-defeat-enemies-desc";

    [DataField]
    public SpriteSpecifier ObjectiveSprite = new SpriteSpecifier.EntityPrototype("MobXenoQueen");

    /// <summary>
    /// Amount of Memory to reward per milestone (every RewardEvery kills)
    /// </summary>
    [DataField]
    public float MemoryReward = 0.5f;

    /// <summary>
    /// Number of kills between memory rewards
    /// </summary>
    [DataField]
    public int RewardEvery = 5;

    /// <summary>
    /// Last kill count when a reward was given
    /// </summary>
    [DataField]
    public int LastRewardCount = 0;
}
