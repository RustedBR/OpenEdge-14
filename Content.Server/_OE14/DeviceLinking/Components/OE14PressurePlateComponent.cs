using Content.Shared.DeviceLinking;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.DeviceLinking.Components;

/// <summary>
/// This component allows the facility to register the weight of objects above it and provide signals to devices
/// </summary>
[RegisterComponent, Access(typeof(OE14PressurePlateSystem))]
public sealed partial class OE14PressurePlateComponent : Component
{
    [DataField]
    public bool IsPressed;

    /// <summary>
    /// The required weight of an object that happens to be above the slab to activate.
    /// </summary>
    [DataField]
    public float WeightRequired = 100f;

    [DataField]
    public float CurrentWeight;

    [DataField]
    public ProtoId<SourcePortPrototype> PressedPort = "OE14Pressed";

    [DataField]
    public ProtoId<SourcePortPrototype> StatusPort = "Status";

    [DataField]
    public ProtoId<SourcePortPrototype> ReleasedPort = "OE14Released";

    [DataField]
    public SoundSpecifier PressedSound = new SoundPathSpecifier("/Audio/Machines/button.ogg");

    [DataField]
    public SoundSpecifier ReleasedSound = new SoundPathSpecifier("/Audio/Machines/button.ogg");
}
