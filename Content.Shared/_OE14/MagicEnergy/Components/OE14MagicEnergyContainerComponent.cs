using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;

namespace Content.Shared._OE14.MagicEnergy.Components;

/// <summary>
/// Allows an item to store magical energy within itself.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(OE14SharedMagicEnergySystem))]
public sealed partial class OE14MagicEnergyContainerComponent : Component
{
    [DataField, AutoNetworkedField]
    public FixedPoint2 Energy = 0;

    [DataField, AutoNetworkedField]
    public FixedPoint2 MaxEnergy = 100;

    [DataField, AutoNetworkedField]
    public ProtoId<AlertPrototype>? MagicAlert;

    /// <summary>
    /// Does this container support unsafe energy manipulation?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool UnsafeSupport;
}
