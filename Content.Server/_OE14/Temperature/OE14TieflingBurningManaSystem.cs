using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared._OE14.MagicEnergy;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared.Damage;
using Content.Shared.Humanoid;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;

namespace Content.Server._OE14.Temperature;

/// <summary>
/// System for Tiefling burning mana regeneration:
/// - Tieflings regenerate mana while burning (taking Heat damage)
/// - Regen scales with Intelligence: baseAmount + (INT - 5)
/// - This replaces OE14MagicEnergyFromDamage for Tieflings
/// </summary>
public sealed class OE14TieflingBurningManaSystem : EntitySystem
{
    [Dependency] private readonly OE14SharedMagicEnergySystem _magicEnergy = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    // Track last mana regen time per tiefling
    private Dictionary<EntityUid, float> _lastManaRegenTime = new();
    private const float RegenInterval = 1f; // Regen every 1 second while burning
    private const float BaseRegenAmount = 2f; // Base mana per regen tick

    public override void Initialize()
    {
        // No events needed, everything is handled in Update
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        // Check for Tieflings who are taking Heat damage (burning) and regenerate mana
        var query = EntityQueryEnumerator<DamageableComponent, OE14CharacterStatsComponent>();
        while (query.MoveNext(out var uid, out var damageable, out var stats))
        {
            // Only apply to Tieflings
            if (!TryComp<HumanoidAppearanceComponent>(uid, out var humanoid) ||
                humanoid.Species != "OE14Tiefling")
                continue;

            // Check if entity is alive
            if (!TryComp<MobStateComponent>(uid, out var mobState) || !_mobState.IsAlive(uid, mobState))
            {
                _lastManaRegenTime.Remove(uid);
                continue;
            }

            // Check if they have damage (indicating they're burning/taking Heat damage)
            if (damageable.TotalDamage == 0)
            {
                _lastManaRegenTime.Remove(uid);
                continue;
            }

            // Track regen timing - regen every 1 second while burning
            if (!_lastManaRegenTime.TryGetValue(uid, out var lastRegen))
                lastRegen = 0f;

            lastRegen += frameTime;

            if (lastRegen >= RegenInterval)
            {
                // Calculate mana regen: BaseRegenAmount + (INT - 5)
                var intBonus = stats.Intelligence - 5;
                var manaRegen = BaseRegenAmount + intBonus;

                _magicEnergy.ChangeEnergy(uid, manaRegen, out var changed, out var overload, safe: true);
                lastRegen = 0f;
            }

            _lastManaRegenTime[uid] = lastRegen;
        }
    }
}
