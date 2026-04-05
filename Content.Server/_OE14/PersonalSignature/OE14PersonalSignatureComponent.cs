using Robust.Shared.Audio;

namespace Content.Server._OE14.PersonalSignature;

[RegisterComponent]
public sealed partial class OE14PersonalSignatureComponent : Component
{
    [DataField]
    public SoundSpecifier? SignSound;
}
