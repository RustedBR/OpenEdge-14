using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared._OE14.CharacterStats;
using Robust.Shared.GameStates;

namespace Content.Shared._OE14.CharacterStats.Systems;

/// <summary>
/// System that manages character stats and calculates their bonuses.
/// </summary>
public abstract partial class OE14SharedCharacterStatsSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14CharacterStatsComponent, ComponentInit>(OnStatsInit);
        SubscribeLocalEvent<OE14CharacterStatsComponent, ComponentStartup>(OnStatsStartup);
    }

    private void OnStatsInit(Entity<OE14CharacterStatsComponent> ent, ref ComponentInit args)
    {
        UpdateStatsCalculations(ent);
    }

    private void OnStatsStartup(Entity<OE14CharacterStatsComponent> ent, ref ComponentStartup args)
    {
        UpdateStatsCalculations(ent);
        OnStartupApply(ent);
    }

    /// <summary>
    /// Called after UpdateStatsCalculations on startup.
    /// Override in server to apply bonuses to actual game systems (health, stamina, mana).
    /// </summary>
    protected virtual void OnStartupApply(Entity<OE14CharacterStatsComponent> ent) { }

    /// <summary>
    /// Called after a stat modifier is changed via AddStatModifier.
    /// Override in server to re-apply threshold/mana/stamina changes.
    /// </summary>
    protected virtual void OnStatModifierChanged(Entity<OE14CharacterStatsComponent> ent) { }

    /// <summary>
    /// Recalculates all stat bonuses based on current stat values plus any active modifiers.
    /// Called whenever stats or modifiers change.
    /// </summary>
    public void UpdateStatsCalculations(Entity<OE14CharacterStatsComponent> ent)
    {
        var stats = ent.Comp;

        // Effective stat = base stat + modifier, clamped to valid range [1, MaxStatValue].
        var effStr = Math.Clamp(stats.Strength + stats.StrengthModifier, 1, stats.MaxStatValue);
        var effVit = Math.Clamp(stats.Vitality + stats.VitalityModifier, 1, stats.MaxStatValue);
        var effDex = Math.Clamp(stats.Dexterity + stats.DexterityModifier, 1, stats.MaxStatValue);
        var effInt = Math.Clamp(stats.Intelligence + stats.IntelligenceModifier, 1, stats.MaxStatValue);

        // Strength: asymmetric so all three anchor points hit exactly.
        // Stat 5 = 1.0x (0%), Stat 10 = 1.5x (+50%), Stat 1 = 0.5x (-50%)
        stats.DamageMultiplier = effStr >= 5
            ? 1.0f + (effStr - 5) * 0.10f
            : 1.0f + (effStr - 5) * 0.125f;

        // Vitality: 20 HP per point from neutral (5).
        // Stat 5 = 0 bonus, Stat 10 = +100 (max 200 HP critical / 400 HP death), Stat 1 = -80
        stats.HealthBonus = (effVit - 5) * 20f;

        // Dexterity: 20% stamina multiplier per point from neutral (5).
        // Stat 5 = 1.0x (100 stamina), Stat 10 = 2.0x (200 stamina max), Stat 1 = 0.2x (20 stamina, minimum)
        stats.StaminaMultiplier = 1.0f + (effDex - 5) * 0.20f;

        // Intelligence: 20 mana per point from neutral (5).
        // Stat 5 = 0 bonus (100 mana), Stat 10 = +100 (200 mana max), Stat 1 = -100 (0 mana minimum)
        stats.ManaBonus = (effInt - 5) * 20f;

        // Mark for network update
        Dirty(ent.Owner, stats);

        // Notify other systems (e.g. spell description updater)
        RaiseLocalEvent(ent.Owner, new OE14StatsUpdatedEvent());
    }

    /// <summary>
    /// Sets a stat and recalculates bonuses.
    /// </summary>
    public void SetStat(EntityUid uid, string statName, int value, OE14CharacterStatsComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        switch (statName.ToLowerInvariant())
        {
            case "strength":
                component.Strength = value;
                break;
            case "vitality":
                component.Vitality = value;
                break;
            case "dexterity":
                component.Dexterity = value;
                break;
            case "intelligence":
                component.Intelligence = value;
                break;
            default:
                return;
        }

        UpdateStatsCalculations(new Entity<OE14CharacterStatsComponent>(uid, component));
    }

    /// <summary>
    /// Gets the damage multiplier from Strength stat.
    /// </summary>
    public float GetDamageMultiplier(EntityUid uid, OE14CharacterStatsComponent? component = null)
    {
        if (!Resolve(uid, ref component, false))
            return 1.0f;

        return component.DamageMultiplier;
    }

    /// <summary>
    /// Gets the health bonus from Vitality stat.
    /// </summary>
    public float GetHealthBonus(EntityUid uid, OE14CharacterStatsComponent? component = null)
    {
        if (!Resolve(uid, ref component, false))
            return 0;

        return component.HealthBonus;
    }

    /// <summary>
    /// Gets the stamina multiplier from Dexterity stat.
    /// </summary>
    public float GetStaminaMultiplier(EntityUid uid, OE14CharacterStatsComponent? component = null)
    {
        if (!Resolve(uid, ref component, false))
            return 1.0f;

        return component.StaminaMultiplier;
    }

    /// <summary>
    /// Gets the mana bonus from Intelligence stat.
    /// </summary>
    public float GetManaBonus(EntityUid uid, OE14CharacterStatsComponent? component = null)
    {
        if (!Resolve(uid, ref component, false))
            return 0;

        return component.ManaBonus;
    }

    /// <summary>
    /// Adds delta (+1 or -1) to a stat modifier granted by a skill or spell.
    ///
    /// Adding rule: if base + currentModifier + delta would exceed MaxStatValue,
    /// the modifier is still incremented (for symmetrical removal later) but a free
    /// AvailablePoint is refunded to the player instead of being "wasted".
    ///
    /// Removing rule: if base + currentModifier currently exceeds MaxStatValue,
    /// that means a point was refunded when this modifier was added — reclaim it.
    /// </summary>
    /// <summary>
    /// Add/remove a stat modifier from a specific source (Spell, Spend, Item, Buff).
    /// </summary>
    public void AddStatModifier(EntityUid uid, string stat, int delta, ModifierSource source = ModifierSource.Spell, OE14CharacterStatsComponent? component = null)
    {
        if (!Resolve(uid, ref component, false))
            return;

        var baseVal = stat.ToLowerInvariant() switch
        {
            "strength"     => component.Strength,
            "vitality"     => component.Vitality,
            "dexterity"    => component.Dexterity,
            "intelligence" => component.Intelligence,
            _              => -1
        };
        if (baseVal < 0)
            return;

        var modifiers = stat.ToLowerInvariant() switch
        {
            "strength"     => component.StrengthModifiers,
            "vitality"     => component.VitalityModifiers,
            "dexterity"    => component.DexterityModifiers,
            "intelligence" => component.IntelligenceModifiers,
            _              => null
        };
        if (modifiers == null)
            return;

        var modVal = component.GetTotalModifier(modifiers);

        if (delta > 0 && baseVal + modVal + delta > component.MaxStatValue)
            component.AvailablePoints++;
        else if (delta < 0 && baseVal + modVal > component.MaxStatValue)
            component.AvailablePoints = Math.Max(0, component.AvailablePoints - 1);

        // Add delta to the specific source
        if (!modifiers.ContainsKey(source))
            modifiers[source] = 0;
        modifiers[source] += delta;

        UpdateStatsCalculations(new Entity<OE14CharacterStatsComponent>(uid, component));
        OnStatModifierChanged(new Entity<OE14CharacterStatsComponent>(uid, component));
    }
}
