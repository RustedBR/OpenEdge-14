namespace Content.Shared._OE14.CharacterStats;

/// <summary>
/// Tracks the origin/source of stat modifiers.
/// Used to display in UI which modifiers come from spells, spending, items, or buffs.
/// </summary>
public enum ModifierSource
{
    /// <summary>Modifiers from learned spells/skills (e.g., Metamagic T1/T2)</summary>
    Spell,

    /// <summary>Modifiers from stat points spent during character creation</summary>
    Spend,

    /// <summary>Modifiers from equipped items (future use)</summary>
    Item,

    /// <summary>Modifiers from temporary buffs/debuffs (future use)</summary>
    Buff
}
