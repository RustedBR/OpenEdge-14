namespace Content.Shared._OE14.Actions.Components;

/// <summary>
/// Requires the user to have at least one free hand to use this spell
/// </summary>
[RegisterComponent]
public sealed partial class OE14ActionFreeHandsRequiredComponent : Component
{
    [DataField]
    public int FreeHandRequired = 1;
}
