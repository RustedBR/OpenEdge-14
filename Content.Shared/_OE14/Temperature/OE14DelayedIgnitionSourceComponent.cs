using Robust.Shared.GameStates;

namespace Content.Shared._OE14.Temperature;

/// <summary>
/// Allows you to fire entities through interacting with them after a delay.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(OE14SharedFireSpreadSystem))]
public sealed partial class OE14DelayedIgnitionSourceComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool Enabled;

    [DataField]
    public TimeSpan Delay = TimeSpan.FromSeconds(3f);
}
