using Robust.Shared.GameStates;

namespace Content.Shared._OE14.MagicWeakness;

/// <summary>
/// trigger entity on unsafe magic energy damage
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(OE14SharedMagicWeaknessSystem))]
public sealed partial class OE14MagicUnsafeTriggerComponent : Component
{
}
