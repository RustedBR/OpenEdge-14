namespace Content.Shared._OE14.Drunk;

/// <summary>
/// Component for dwarves that grants alcohol healing while drunk.
/// When a dwarf has this component and is drunk, they passively regenerate HP.
///
/// Alcohol resistance is handled by adding LightweightDrunkComponent to the same entity.
/// </summary>
[RegisterComponent]
public sealed partial class OE14DwarfAlcoholResistanceComponent : Component
{
    // This component marks a dwarf for special alcohol healing behavior
    // The actual healing is handled by OE14DwarfAlcoholSystem
}
