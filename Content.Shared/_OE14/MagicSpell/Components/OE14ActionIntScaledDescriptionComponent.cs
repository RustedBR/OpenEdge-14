using Robust.Shared.Serialization;

namespace Content.Shared._OE14.MagicSpell.Components;

/// <summary>
/// A single value entry shown dynamically in an action's description.
/// </summary>
[DataDefinition]
public sealed partial class OE14IntScaleEntry
{
    /// <summary>
    /// Label shown before the value. E.g. "Dano", "Cura", "Duração".
    /// </summary>
    [DataField(required: true)]
    public string Label = "";

    /// <summary>
    /// Base value at INT 5 for multiplicative scaling, or flat base for additive.
    /// </summary>
    [DataField(required: true)]
    public float BaseValue = 0f;

    /// <summary>
    /// Unit string appended after the value. E.g. " HP", "s", " mana", " tiles".
    /// </summary>
    [DataField]
    public string Unit = "";

    /// <summary>
    /// Number of decimal places to display.
    /// </summary>
    [DataField]
    public int Decimals = 1;

    /// <summary>
    /// When true: value = BaseValue + effInt * IntBonus (additive, e.g. range/duration).
    /// When false (default): value = BaseValue * intMultiplier (multiplicative, e.g. damage).
    /// </summary>
    [DataField]
    public bool IsAdditive = false;

    /// <summary>
    /// Bonus per INT point for additive scaling. Only used when IsAdditive = true.
    /// </summary>
    [DataField]
    public float IntBonus = 0f;
}

/// <summary>
/// Adds a dynamic line to the action description showing current INT-scaled values.
/// The system regenerates the description every time the owning player's Intelligence changes.
/// </summary>
[RegisterComponent]
public sealed partial class OE14ActionIntScaledDescriptionComponent : Component
{
    /// <summary>
    /// Base description in PT-BR (shown as-is when no player owns this action).
    /// The dynamic value line is appended after this text.
    /// </summary>
    [DataField(required: true)]
    public string BaseDescription = "";

    /// <summary>
    /// One or more scaled value entries to show in the description.
    /// Multiple entries are separated by " | " on a single line.
    /// </summary>
    [DataField]
    public List<OE14IntScaleEntry> ScaleEntries { get; set; } = new();
}
