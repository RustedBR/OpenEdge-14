using Robust.Shared.GameStates;

namespace Content.Shared._OE14.Actions.Components;

/// <summary>
/// apply slowdown effect from casting spells
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(OE14SharedActionSystem))]
public sealed partial class OE14SlowdownFromActionsComponent : Component
{
    [DataField, AutoNetworkedField]
    public Dictionary<NetEntity, float> SpeedAffectors = new();
}
