namespace Content.Shared._OE14.Actions.Components;

[RegisterComponent]
public sealed partial class OE14ActionEmotingComponent : Component
{
    [DataField]
    public string StartEmote = string.Empty; //Not LocId!

    [DataField]
    public string EndEmote = string.Empty; //Not LocId!
}
