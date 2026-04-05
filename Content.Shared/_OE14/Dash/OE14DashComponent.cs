using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Dash;

/// <summary>
/// This component marks entities that are currently in the dash
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(OE14DashSystem))]
public sealed partial class OE14DashComponent : Component
{
    [DataField]
    public EntProtoId DashEffect = "OE14DustEffect";

    [DataField]
    public SoundSpecifier DashSound = new SoundPathSpecifier("/Audio/_OE14/Effects/dash.ogg")
    {
        Params = AudioParams.Default.WithVariation(0.05f)
    };
}
