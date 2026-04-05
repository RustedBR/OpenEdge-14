using Content.Shared._OE14.Workbench;

namespace Content.Shared._OE14.Actions.Components;

/// <summary>
/// Requires the caster to hold a specific resource in their hand, which will be spent to use the spell.
/// </summary>
[RegisterComponent]
public sealed partial class OE14ActionMaterialCostComponent : Component
{
    [DataField(required: true)]
    public OE14WorkbenchCraftRequirement? Requirement;
}
