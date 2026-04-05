using Content.Shared._OE14.MagicSpell;

namespace Content.Shared._OE14.Actions.Components;

/// <summary>
/// Restricts the use of this action, by spending stamina.
/// </summary>
[RegisterComponent]
public sealed partial class OE14ActionStaminaCostComponent : Component
{
    [DataField]
    public float Stamina = 0f;
}
