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
    private const float HealInterval = 1.0f; // Heal every 1 second
    private const float HealAmount = 5f; // 5 HP per heal tick = 5 HP/s


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
                }
            }

            if (!hasAlcoholInBloodstream)
            {
                _lastHealTime.Remove(uid);
                continue;
            }

            // Track healing timing - heal periodically while alcohol is in bloodstream
            if (!_lastHealTime.TryGetValue(uid, out var lastHeal))
                lastHeal = 0f;

            lastHeal += frameTime;

            if (lastHeal >= HealInterval)
            {
                // Heal the dwarf: 5 HP per heal tick (1 second) = 5 HP/s total
                // Heals ALL damage types while alcohol is in bloodstream
                var damage = new DamageSpecifier();
                damage.DamageDict["Brute"] = FixedPoint2.New(-HealAmount); // Negative = healing
                damage.DamageDict["Slash"] = FixedPoint2.New(-HealAmount);
                damage.DamageDict["Pierce"] = FixedPoint2.New(-HealAmount);
                damage.DamageDict["Burn"] = FixedPoint2.New(-HealAmount);
                damage.DamageDict["Radiation"] = FixedPoint2.New(-HealAmount);
                damage.DamageDict["Toxin"] = FixedPoint2.New(-HealAmount);
                damage.DamageDict["Heat"] = FixedPoint2.New(-HealAmount);
                damage.DamageDict["Cold"] = FixedPoint2.New(-HealAmount);

                _damageable.TryChangeDamage(uid, damage, true);

                // TODO: Also reduce bleeding (bloodloss) at a slower rate than HP healing
                // This requires write-access to BloodstreamComponent, which needs a separate system query

                lastHeal = 0f;
            }

            _lastHealTime[uid] = lastHeal;
        }
    }
}
