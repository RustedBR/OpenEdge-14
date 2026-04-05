using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._OE14.MagicEssence;

/// <summary>
///
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class OE14MagicEssenceComponent : Component
{
    [DataField]
    public string Solution = "essence";

    [DataField]
    public SoundSpecifier ConsumeSound = new SoundPathSpecifier("/Audio/_OE14/Effects/essence_consume.ogg")
    {
        Params = AudioParams.Default.WithVolume(-2f).WithVariation(0.2f),
    };
}
