using Content.Server._OE14.MagicEnergy;
using Content.Shared._OE14.CharacterStats;
using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared._OE14.CharacterStats.Systems;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared._OE14.ModularCraft.Components;
using Content.Shared.Damage.Components;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.Hands;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;

namespace Content.Server._OE14.CharacterStats;

/// <summary>
/// Server-side extension of the character stats system.
/// Handles:
/// - Applying stat bonuses to actual game systems (health, stamina, mana) at startup and after changes.
/// - Processing client requests to spend stat points.
/// - Applying/removing stat bonuses from equipped items.
/// </summary>
public sealed partial class OE14CharacterStatsSystem : OE14SharedCharacterStatsSystem
{
    [Dependency] private readonly MobThresholdSystem _thresholds = default!;
    [Dependency] private readonly OE14MagicEnergySystem _magicEnergy = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeed = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<OE14SpendStatPointMessage>(OnSpendStatPoint);
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawnComplete);
        SubscribeLocalEvent<OE14CharacterStatsComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovementSpeed);

        // Component-directed subscriptions: fired via EntDispatch, independent of broadcast flag.
        // GotEquipped/GotUnequipped are raised on the ITEM entity — filter by OE14StatBonusComponent on the item.
        // This is the correct SS14 pattern (not broadcast handlers which require broadcast=true).
        SubscribeLocalEvent<OE14StatBonusComponent, GotEquippedEvent>(OnItemEquipped);
        SubscribeLocalEvent<OE14StatBonusComponent, GotUnequippedEvent>(OnItemUnequipped);
        SubscribeLocalEvent<OE14StatBonusComponent, GotEquippedHandEvent>(OnItemEquippedHand);
        SubscribeLocalEvent<OE14StatBonusComponent, GotUnequippedHandEvent>(OnItemUnequippedHand);
    }

    /// <summary>
    /// Applies character-creation stat bonuses from the player's profile at spawn.
    /// If player didn't spend any creation points and has no available runtime points,
    /// grants 3 points to spend in-game as a fallback.
    /// </summary>
    private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent ev)
    {
        if (!TryComp<OE14CharacterStatsComponent>(ev.Mob, out var stats))
            return;

        var profile = ev.Profile;
        var hasSpending = profile.OE14StatBonusStrength != 0 ||
                          profile.OE14StatBonusVitality != 0 ||
                          profile.OE14StatBonusDexterity != 0 ||
                          profile.OE14StatBonusIntelligence != 0;

        if (hasSpending)
        {
            if (profile.OE14StatBonusStrength != 0)
                AddStatModifier(ev.Mob, "strength", profile.OE14StatBonusStrength, ModifierSource.Spend, stats);
            if (profile.OE14StatBonusVitality != 0)
                AddStatModifier(ev.Mob, "vitality", profile.OE14StatBonusVitality, ModifierSource.Spend, stats);
            if (profile.OE14StatBonusDexterity != 0)
                AddStatModifier(ev.Mob, "dexterity", profile.OE14StatBonusDexterity, ModifierSource.Spend, stats);
            if (profile.OE14StatBonusIntelligence != 0)
                AddStatModifier(ev.Mob, "intelligence", profile.OE14StatBonusIntelligence, ModifierSource.Spend, stats);

            var creationPointsAvailable = 3;
            if (TryComp<HumanoidAppearanceComponent>(ev.Mob, out var appearance) && appearance.Species == "OE14Human")
                creationPointsAvailable = 5;

            var totalSpent = profile.OE14StatBonusStrength + profile.OE14StatBonusVitality +
                             profile.OE14StatBonusDexterity + profile.OE14StatBonusIntelligence;
            var pointsLeft = creationPointsAvailable - totalSpent;
            if (pointsLeft > 0)
                stats.AvailablePoints += pointsLeft;

            UpdateStatsCalculations(new Entity<OE14CharacterStatsComponent>(ev.Mob, stats));
            ApplyStatsToGameSystems(ev.Mob, stats);
        }
        else if (stats.AvailablePoints == 0)
        {
            var fallbackPoints = 3;
            if (TryComp<HumanoidAppearanceComponent>(ev.Mob, out var appearance) && appearance.Species == "OE14Human")
                fallbackPoints = 5;
            stats.AvailablePoints = fallbackPoints;
            Dirty(ev.Mob, stats);
        }
    }

    /// <summary>
    /// Called after UpdateStatsCalculations on startup.
    /// Applies bonuses to health, stamina, and mana systems.
    /// </summary>
    protected override void OnStartupApply(Entity<OE14CharacterStatsComponent> ent)
    {
        ApplyStatsToGameSystems(ent.Owner, ent.Comp);
    }

    /// <summary>
    /// Called when a stat modifier changes (e.g., from a skill or spell).
    /// Re-applies thresholds so health, stamina, and mana update immediately.
    /// </summary>
    protected override void OnStatModifierChanged(Entity<OE14CharacterStatsComponent> ent)
    {
        ApplyStatsToGameSystems(ent.Owner, ent.Comp);
    }

    /// <summary>
    /// Handle client request to spend one available stat point.
    /// Validates that the stat is not already at MaxStatValue before allowing the spend.
    /// </summary>
    private void OnSpendStatPoint(OE14SpendStatPointMessage ev, EntitySessionEventArgs args)
    {
        var entity = GetEntity(ev.Entity);

        if (args.SenderSession.AttachedEntity != entity)
            return;

        if (!TryComp<OE14CharacterStatsComponent>(entity, out var stats))
            return;

        if (stats.AvailablePoints <= 0)
            return;

        var (statValue, statModifier) = ev.StatName.ToLowerInvariant() switch
        {
            "strength"     => (stats.Strength, stats.StrengthModifier),
            "vitality"     => (stats.Vitality, stats.VitalityModifier),
            "dexterity"    => (stats.Dexterity, stats.DexterityModifier),
            "intelligence" => (stats.Intelligence, stats.IntelligenceModifier),
            _              => (-1, 0)
        };

        if (statValue < 0)
            return;

        var effectiveValue = statValue + statModifier;
        if (effectiveValue >= stats.MaxStatValue)
            return;

        stats.AvailablePoints--;
        AddStatModifier(entity, ev.StatName, +1, ModifierSource.Spend, stats);
    }

    /// <summary>
    /// Applies calculated stat bonuses to the actual gameplay components.
    /// Called at startup and whenever a stat changes.
    /// </summary>
    public void ApplyStatsToGameSystems(EntityUid uid, OE14CharacterStatsComponent stats)
    {
        // Health thresholds from Vitality
        var critThreshold = (FixedPoint2)(100f + stats.HealthBonus);
        var deadThreshold = critThreshold * 2;

        _thresholds.SetMobStateThreshold(uid, critThreshold, MobState.Critical);
        _thresholds.SetMobStateThreshold(uid, deadThreshold, MobState.Dead);

        // Stamina from Dexterity
        if (TryComp<StaminaComponent>(uid, out var stamina))
        {
            var newStatThreshold = 100f * stats.StaminaMultiplier;

            float finalThreshold;
            if (stats.LastAppliedStaminaThreshold < 0)
            {
                finalThreshold = newStatThreshold;
            }
            else
            {
                var externalBonus = stamina.CritThreshold - stats.LastAppliedStaminaThreshold;
                finalThreshold = newStatThreshold + externalBonus;
            }
            stats.LastAppliedStaminaThreshold = newStatThreshold;
            stamina.CritThreshold = finalThreshold;

            var stunThresholds = new Dictionary<FixedPoint2, float>();
            stunThresholds.TryAdd(0f, 1.00f);
            stunThresholds.TryAdd((FixedPoint2)(finalThreshold * 0.25f), 0.85f);
            stunThresholds.TryAdd((FixedPoint2)(finalThreshold * 0.50f), 0.70f);
            stunThresholds.TryAdd((FixedPoint2)(finalThreshold * 0.75f), 0.50f);
            stamina.StunModifierThresholds = stunThresholds;
            stamina.AnimationThreshold = finalThreshold * 0.25f;

            Dirty(uid, stamina);
        }

        // Mana from Intelligence
        if (_magicEnergy.GetMaxEnergy(uid) is { } currentMax)
        {
            var newStatMax = (FixedPoint2)Math.Max(0f, 100f + stats.ManaBonus);

            var lastApplied = stats.LastAppliedManaMax < 0
                ? currentMax
                : (FixedPoint2)stats.LastAppliedManaMax;

            var delta = newStatMax - lastApplied;
            stats.LastAppliedManaMax = newStatMax.Float();

            if (delta != 0)
                _magicEnergy.ChangeMaximumEnergy(uid, delta);

            var totalMax = currentMax + delta;
            var regenPerTick = (FixedPoint2)Math.Max(0f, totalMax.Float() / 100f);
            _magicEnergy.SetDrawRate(uid, regenPerTick, 3f);
        }

        // Refresh movement speed based on Dexterity
        if (TryComp<MovementSpeedModifierComponent>(uid, out _))
        {
            _movementSpeed.RefreshMovementSpeedModifiers(uid);
        }
    }

    /// <summary>
    /// Apply movement speed modifier based on Dexterity stat.
    /// DEX 5 = 100%, DEX 10 = 125%, DEX 1 = 75%
    /// Formula: 1.0 + (DEX - 5) * 0.0625
    /// </summary>
    private void OnRefreshMovementSpeed(Entity<OE14CharacterStatsComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
    {
        var dexModifier = 1.0f + (ent.Comp.Dexterity - 5) * 0.0625f;
        args.ModifySpeed(dexModifier, dexModifier);
    }

    // ── Item equip/unequip handlers ──────────────────────────────────────────
    // These use component-directed subscriptions (SubscribeLocalEvent<TComp, TEvent>)
    // which fire via EntDispatch regardless of the broadcast flag.
    // Broadcast-only subscriptions (SubscribeLocalEvent<TEvent>) do NOT work here
    // because hand events are raised with broadcast=false.

    private void OnItemEquipped(EntityUid uid, OE14StatBonusComponent bonuses, GotEquippedEvent ev)
    {
        ApplyStatBonusFromItem(ev.Equipee, bonuses);
    }

    private void OnItemUnequipped(EntityUid uid, OE14StatBonusComponent bonuses, GotUnequippedEvent ev)
    {
        RemoveStatBonusFromItem(ev.Equipee, bonuses);
    }

    private void OnItemEquippedHand(EntityUid uid, OE14StatBonusComponent bonuses, GotEquippedHandEvent ev)
    {
        ApplyStatBonusFromItem(ev.User, bonuses);
    }

    private void OnItemUnequippedHand(EntityUid uid, OE14StatBonusComponent bonuses, GotUnequippedHandEvent ev)
    {
        RemoveStatBonusFromItem(ev.User, bonuses);
    }

    // ── Stat bonus apply/remove helpers ─────────────────────────────────────

    public void ApplyStatBonusFromItem(EntityUid wearer, OE14StatBonusComponent bonuses)
    {
        if (!TryComp<OE14CharacterStatsComponent>(wearer, out var stats))
            return;

        var strBonus = bonuses.StatBonuses.TryGetValue("strength", out var str) ? str : 0;
        var vitBonus = bonuses.StatBonuses.TryGetValue("vitality", out var vit) ? vit : 0;
        var dexBonus = bonuses.StatBonuses.TryGetValue("dexterity", out var dex) ? dex : 0;
        var intBonus = bonuses.StatBonuses.TryGetValue("intelligence", out var intel) ? intel : 0;

        if (strBonus != 0)
            AddStatModifier(wearer, "strength", strBonus, ModifierSource.Item, stats);
        if (vitBonus != 0)
            AddStatModifier(wearer, "vitality", vitBonus, ModifierSource.Item, stats);
        if (dexBonus != 0)
            AddStatModifier(wearer, "dexterity", dexBonus, ModifierSource.Item, stats);
        if (intBonus != 0)
            AddStatModifier(wearer, "intelligence", intBonus, ModifierSource.Item, stats);
    }

    private void RemoveStatBonusFromItem(EntityUid wearer, OE14StatBonusComponent bonuses)
    {
        if (!TryComp<OE14CharacterStatsComponent>(wearer, out var stats))
            return;

        var strBonus = bonuses.StatBonuses.TryGetValue("strength", out var str) ? str : 0;
        var vitBonus = bonuses.StatBonuses.TryGetValue("vitality", out var vit) ? vit : 0;
        var dexBonus = bonuses.StatBonuses.TryGetValue("dexterity", out var dex) ? dex : 0;
        var intBonus = bonuses.StatBonuses.TryGetValue("intelligence", out var intel) ? intel : 0;

        if (strBonus != 0)
            AddStatModifier(wearer, "strength", -strBonus, ModifierSource.Item, stats);
        if (vitBonus != 0)
            AddStatModifier(wearer, "vitality", -vitBonus, ModifierSource.Item, stats);
        if (dexBonus != 0)
            AddStatModifier(wearer, "dexterity", -dexBonus, ModifierSource.Item, stats);
        if (intBonus != 0)
            AddStatModifier(wearer, "intelligence", -intBonus, ModifierSource.Item, stats);
    }
}
