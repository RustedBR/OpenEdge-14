using Robust.Shared.GameStates;

namespace Content.Shared._OE14.MagicEnergy.Components;

/// <summary>
/// Allows you to examine how much energy is in that object.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(OE14SharedMagicEnergySystem))]
public sealed partial class OE14MagicEnergyExaminableComponent : Component;
