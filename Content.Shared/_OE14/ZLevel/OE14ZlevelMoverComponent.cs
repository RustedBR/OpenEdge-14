using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.ZLevel;

/// <summary>
/// component that allows you to quickly move between Z levels
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class OE14ZLevelMoverComponent : Component
{
    [DataField]
    public EntProtoId UpActionProto = "OE14ActionZLevelUp";

    [DataField, AutoNetworkedField]
    public EntityUid? OE14ZLevelUpActionEntity;

    [DataField]
    public EntProtoId DownActionProto = "OE14ActionZLevelDown";

    [DataField, AutoNetworkedField]
    public EntityUid? OE14ZLevelDownActionEntity;
}
