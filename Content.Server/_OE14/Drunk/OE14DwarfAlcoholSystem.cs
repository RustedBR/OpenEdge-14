using Content.Shared._OE14.Drunk;
using Content.Shared.Body.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.Drunk;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.StatusEffectNew;
using Content.Shared.StatusEffectNew.Components;
using Robust.Shared.Containers;
using Robust.Shared.Log;

namespace Content.Server._OE14.Drunk;

/// <summary>
/// System for dwarven alcohol mechanics:
/// - Dwarves resist drunkenness (shorter duration)
/// - Dwarves heal while they have alcohol in their bloodstream (passive HP regeneration)
/// </summary>
public sealed class OE14DwarfAlcoholSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainerSystem = default!;

    // Track last heal time per dwarf
    private Dictionary<EntityUid, float> _lastHealTime = new();
    private const float HealInterval = 0.5f; // Heal every 0.5 seconds
    private const float HealAmount = 5f; // HP per heal tick

    // Debug tracking to avoid spam
    private HashSet<EntityUid> _debuggedDwarves = new();

    public override void Initialize()
    {
        // No events needed, everything is handled in Update
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        // Check for dwarves with alcohol in their bloodstream and heal them
        var query = EntityQueryEnumerator<OE14DwarfAlcoholResistanceComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            // Check if entity is alive
            if (!TryComp<MobStateComponent>(uid, out var mobState) || !_mobState.IsAlive(uid, mobState))
            {
                _lastHealTime.Remove(uid);
                continue;
            }

            // Check if they have alcohol in their bloodstream
            var hasAlcoholInBloodstream = false;

            if (TryComp<BloodstreamComponent>(uid, out var bloodstream))
            {
                // Try to get the chemical solution (where alcohol/reagents accumulate)
                if (_solutionContainerSystem.ResolveSolution(uid, bloodstream.ChemicalSolutionName,
                    ref bloodstream.ChemicalSolution, out var chemicalSolution))
                {
                    // If there are any reagents in the chemical solution, the dwarf has alcohol in their bloodstream
                    hasAlcoholInBloodstream = chemicalSolution.Contents.Count > 0;

                    // Debug logging - once per dwarf when alcohol detected
                    if (hasAlcoholInBloodstream && !_debuggedDwarves.Contains(uid))
                    {
                        Logger.InfoS("dwarf-alcohol", $"Dwarf {uid} has {chemicalSolution.Contents.Count} reagents in bloodstream, healing started");
                        _debuggedDwarves.Add(uid);
                    }
                }
                else if (!_debuggedDwarves.Contains(uid))
                {
                    Logger.WarningS("dwarf-alcohol", $"Could not resolve chemical solution for dwarf {uid}");
                    _debuggedDwarves.Add(uid);
                }
            }
            else if (!_debuggedDwarves.Contains(uid))
            {
                Logger.WarningS("dwarf-alcohol", $"Dwarf {uid} has no BloodstreamComponent");
                _debuggedDwarves.Add(uid);
            }

            if (!hasAlcoholInBloodstream)
            {
                _lastHealTime.Remove(uid);
                _debuggedDwarves.Remove(uid); // Reset debug tracking when alcohol is gone
                continue;
            }

            // Track healing timing - heal every 1 second while drunk
            if (!_lastHealTime.TryGetValue(uid, out var lastHeal))
                lastHeal = 0f;

            lastHeal += frameTime;

            if (lastHeal >= HealInterval)
            {
                // Heal the dwarf: 5 HP per heal tick (0.5 seconds) = 10 HP/s total
                // Heals all physical damage types (Brute, Slash, Pierce, etc.)
                var damage = new DamageSpecifier();
                damage.DamageDict["Brute"] = FixedPoint2.New(-HealAmount); // Negative = healing
                damage.DamageDict["Slash"] = FixedPoint2.New(-HealAmount);
                damage.DamageDict["Pierce"] = FixedPoint2.New(-HealAmount);

                _damageable.TryChangeDamage(uid, damage, true);
                lastHeal = 0f;
            }

            _lastHealTime[uid] = lastHeal;
        }
    }
}
