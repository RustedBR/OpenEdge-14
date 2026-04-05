using Robust.Shared.Audio;

namespace Content.Shared._OE14.LockKey.Components;

/// <summary>
/// Allows, when interacting with keys, to mill different teeth, changing the shape of the key
/// </summary>
[RegisterComponent]
public sealed partial class OE14LockEditerComponent : Component
{
    /// <summary>
    /// sound when used
    /// </summary>
    [DataField]
    public SoundSpecifier UseSound =
        new SoundCollectionSpecifier("Screwdriver")
        {
            Params = AudioParams.Default.WithVariation(0.02f),
        };
}

//Ed: maybe this component should be removed, and logic be attached to "Screwing" tool?
//OE14KeyFileComponent too, but with different tool prototype

//    ／l、         meow
//  （ﾟ､ ｡ ７
//    l  ~ヽ
//    じしf_,)ノ
