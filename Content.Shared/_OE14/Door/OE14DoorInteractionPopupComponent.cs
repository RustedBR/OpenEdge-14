using Robust.Shared.Audio;

namespace Content.Shared._OE14.Door;

[RegisterComponent, Access(typeof(OE14DoorInteractionPopupSystem))]
public sealed partial class OE14DoorInteractionPopupComponent : Component
{
    /// <summary>
    /// Time delay between interactions to avoid spam.
    /// </summary>
    [DataField("interactDelay")]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan InteractDelay = TimeSpan.FromSeconds(1.0);

    [DataField("interactString")]
    public string InteractString = "oe14-closed-door-interact-popup";

    [DataField("interactSound")]
    public SoundSpecifier? InteractSound;

    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan LastInteractTime = TimeSpan.Zero;

}
