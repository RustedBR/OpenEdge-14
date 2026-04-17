using Content.Shared._OE14.Drunk;
using Content.Shared.Damage;
using Content.Shared.Drunk;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.StatusEffectNew;

namespace Content.Server._OE14.Drunk;

/// <summary>
/// System for dwarven alcohol mechanics:
/// - Dwarves resist drunkenness (shorter duration)
/// - Dwarves heal while drunk (passive HP regeneration)
/// </summary>
public sealed class OE14DwarfAlcoholSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    // Track last heal time per dwarf
    private Dictionary<EntityUid, float> _lastHealTime = new();
    private const float HealInterval = 1f; // Heal every 1 second
    private const float HealAmount = 2f; // HP per heal tick

    private float _resistanceCheckTimer = 0f;
    private const float ResistanceCheckInterval = 2f; // Check and reduce drunk time every 2 seconds

    public override void Initialize()
    {
        // No events needed, everything is handled in Update
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        // Check for dwarves with active drunk status and heal them
        var query = EntityQueryEnumerator<OE14DwarfAlcoholResistanceComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            // Check if entity is alive
            if (!TryComp<MobStateComponent>(uid, out var mobState) || !_mobState.IsAlive(uid, mobState))
            {
                _lastHealTime.Remove(uid);
                continue;
            }

            // Check if they have the Drunk status effect
            if (!HasComp<DrunkStatusEffectComponent>(uid))
            {
                _lastHealTime.Remove(uid);
                continue;
            }

            // Track healing timing - heal every 1 second while drunk
            if (!_lastHealTime.TryGetValue(uid, out var lastHeal))
                lastHeal = 0f;

            lastHeal += frameTime;

            if (lastHeal >= HealInterval)
            {
                // Heal the dwarf while drunk: 5 HP per second
                var damage = new DamageSpecifier();
                damage.DamageDict["Brute"] = FixedPoint2.New(-HealAmount); // Negative = healing

                _damageable.TryChangeDamage(uid, damage, true);
                lastHeal = 0f;
            }

            _lastHealTime[uid] = lastHeal;
        }
    }
}
