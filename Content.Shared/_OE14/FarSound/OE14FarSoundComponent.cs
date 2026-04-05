using Robust.Shared.Audio;

namespace Content.Shared._OE14.FarSound;

[RegisterComponent]
public sealed partial class OE14FarSoundComponent : Component
{
    [DataField]
    public SoundSpecifier? CloseSound;

    [DataField]
    public SoundSpecifier? FarSound;

    [DataField]
    public float FarRange = 50f;
}
