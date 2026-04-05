using Content.Shared.FixedPoint;
using Content.Shared._OE14.CharacterStats.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using System.Collections.Generic;

namespace Content.Shared._OE14.CharacterStats.Components;

/// <summary>
/// Component that stores character stats (attributes) that influence gameplay.
/// Stats provide bonuses to damage, health, stamina, and mana.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class OE14CharacterStatsComponent : Component
{
    /// <summary>
    /// Strength: Increases physical damage and reduces slowdown
    /// </summary>
    [DataField, AutoNetworkedField]
    public int Strength = 10;

    /// <summary>
    /// Vitality: Increases maximum health
    /// </summary>
    [DataField, AutoNetworkedField]
    public int Vitality = 10;

    /// <summary>
    /// Dexterity: Increases stamina
    /// </summary>
    [DataField, AutoNetworkedField]
    public int Dexterity = 10;

    /// <summary>
    /// Intelligence: Increases maximum mana
    /// </summary>
    [DataField, AutoNetworkedField]
    public int Intelligence = 10;

    /// <summary>
    /// Calculated damage multiplier from Strength (1.0 + Strength * 0.10)
    /// </summary>
    [DataField, AutoNetworkedField]
    public float DamageMultiplier = 1.0f;

    /// <summary>
    /// Calculated health bonus from Vitality (12.5 HP per point from neutral 5)
    /// </summary>
    [DataField, AutoNetworkedField]
    public float HealthBonus = 0;

    /// <summary>
    /// Calculated stamina multiplier from Dexterity (18.75% per point from neutral 5)
    /// </summary>
    [DataField, AutoNetworkedField]
    public float StaminaMultiplier = 1.0f;

    /// <summary>
    /// Calculated mana bonus from Intelligence (25 mana per point from neutral 5)
    /// </summary>
    [DataField, AutoNetworkedField]
    public float ManaBonus = 0;

    // ── Stat modifiers ───────────────────────────────────────────────────────
    // Temporary bonuses/penalties from equipment, spells, or other effects.
    // Tracked by source (Spell, Spend, Item, Buff) for UI display.
    // Effective stat = base stat + total modifiers (clamped to 1..MaxStatValue).
    // Call OE14CharacterStatsSystem.ApplyStatModifier() to add/remove them.

    [DataField, AutoNetworkedField]
    public Dictionary<ModifierSource, int> StrengthModifiers = new();

    [DataField, AutoNetworkedField]
    public Dictionary<ModifierSource, int> VitalityModifiers = new();

    [DataField, AutoNetworkedField]
    public Dictionary<ModifierSource, int> DexterityModifiers = new();

    [DataField, AutoNetworkedField]
    public Dictionary<ModifierSource, int> IntelligenceModifiers = new();

    /// <summary>
    /// Helper method: Get total modifier value for a stat (sum of all sources).
    /// </summary>
    public int GetTotalModifier(Dictionary<ModifierSource, int> modifiers)
    {
        if (modifiers == null || modifiers.Count == 0)
            return 0;

        var total = 0;
        foreach (var mod in modifiers.Values)
            total += mod;
        return total;
    }

    /// <summary>
    /// Compatibility properties: Return total modifier (sum of all sources).
    /// Setter modifies the Spend source (for character creation bonuses).
    /// </summary>
    public int StrengthModifier
    {
        get => GetTotalModifier(StrengthModifiers);
        set
        {
            if (!StrengthModifiers.ContainsKey(ModifierSource.Spend))
                StrengthModifiers[ModifierSource.Spend] = 0;
            StrengthModifiers[ModifierSource.Spend] = value;
        }
    }

    public int VitalityModifier
    {
        get => GetTotalModifier(VitalityModifiers);
        set
        {
            if (!VitalityModifiers.ContainsKey(ModifierSource.Spend))
                VitalityModifiers[ModifierSource.Spend] = 0;
            VitalityModifiers[ModifierSource.Spend] = value;
        }
    }

    public int DexterityModifier
    {
        get => GetTotalModifier(DexterityModifiers);
        set
        {
            if (!DexterityModifiers.ContainsKey(ModifierSource.Spend))
                DexterityModifiers[ModifierSource.Spend] = 0;
            DexterityModifiers[ModifierSource.Spend] = value;
        }
    }

    public int IntelligenceModifier
    {
        get => GetTotalModifier(IntelligenceModifiers);
        set
        {
            if (!IntelligenceModifiers.ContainsKey(ModifierSource.Spend))
                IntelligenceModifiers[ModifierSource.Spend] = 0;
            IntelligenceModifiers[ModifierSource.Spend] = value;
        }
    }

    // ── Point spending ────────────────────────────────────────────────────────

    /// <summary>
    /// Unspent stat points the player can allocate.
    /// Awarded by the server (e.g., on level-up or admin grant).
    /// </summary>
    [DataField, AutoNetworkedField]
    public int AvailablePoints = 0;

    /// <summary>
    /// Maximum value any single stat can reach (including modifiers).
    /// </summary>
    [DataField]
    public int MaxStatValue = 10;

    // ── Last-applied tracking (server-only) ───────────────────────────────────
    // Used to apply stat contributions as deltas so external bonuses
    // from spells or equipment are preserved across recalculations.
    // -1 means "not yet applied" — triggers an absolute set on first call.

    [DataField]
    public float LastAppliedManaMax = -1f;

    [DataField]
    public float LastAppliedStaminaThreshold = -1f;
}
