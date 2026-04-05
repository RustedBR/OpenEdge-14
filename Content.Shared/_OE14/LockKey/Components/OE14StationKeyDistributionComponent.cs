using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.LockKey.Components;

/// <summary>
///
/// </summary>
[RegisterComponent]
public sealed partial class OE14StationKeyDistributionComponent : Component
{
    [DataField]
    public List<ProtoId<OE14LockTypePrototype>> Keys = new();
}
