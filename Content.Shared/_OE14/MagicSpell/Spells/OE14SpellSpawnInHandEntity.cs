using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Melee;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellSpawnInHandEntity : OE14SpellEffect
{
    [DataField]
    public List<EntProtoId> Spawns = new();

    [DataField]
    public bool DeleteIfCantPickup = false;

    /// <summary>
    /// Adds (INT * IntBonusSolutionQuantity) units of each reagent already in the solution.
    /// Used for beer/water creation to scale liquid amount with Intelligence.
    /// </summary>
    [DataField]
    public float IntBonusSolutionQuantity = 0f;

    /// <summary>
    /// When true, scales MeleeWeapon, Projectile and DamageOtherOnHit damage by INT multiplier.
    /// INT 5 = 1.0x, INT 10 = 1.5x, INT 1 = 0.5x.
    /// </summary>
    [DataField]
    public bool ScaleWeaponDamage = false;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        if (!entManager.TryGetComponent<TransformComponent>(args.Target.Value, out var transformComponent))
            return;

        var handSystem = entManager.System<SharedHandsSystem>();

        float intMultiplier = 1f;
        int effInt = 5;
        if (args.User is not null &&
            entManager.TryGetComponent<OE14CharacterStatsComponent>(args.User.Value, out var stats))
        {
            effInt = Math.Clamp(stats.Intelligence + stats.IntelligenceModifier, 1, stats.MaxStatValue);
            intMultiplier = effInt >= 5
                ? 1.0f + (effInt - 5) * 0.10f
                : 1.0f + (effInt - 5) * 0.125f;
        }

        foreach (var spawn in Spawns)
        {
            var item = entManager.PredictedSpawnAtPosition(spawn, transformComponent.Coordinates);

            if (IntBonusSolutionQuantity > 0f)
                ScaleSolution(entManager, item, effInt, IntBonusSolutionQuantity);

            if (ScaleWeaponDamage)
                ScaleDamageComponents(entManager, item, intMultiplier);

            if (!handSystem.TryPickupAnyHand(args.Target.Value, item) && DeleteIfCantPickup)
                entManager.QueueDeleteEntity(item);
        }
    }

    private static void ScaleSolution(EntityManager entManager, EntityUid item, int effInt, float bonusPerInt)
    {
        if (!entManager.TryGetComponent<SolutionContainerManagerComponent>(item, out var manager))
            return;

        var solutionSystem = entManager.System<SharedSolutionContainerSystem>();
        var bonus = FixedPoint2.New(effInt * bonusPerInt);
        if (bonus <= FixedPoint2.Zero)
            return;

        foreach (var (_, sol) in solutionSystem.EnumerateSolutions((item, manager)))
        {
            solutionSystem.SetCapacity(sol, sol.Comp.Solution.MaxVolume + bonus);

            foreach (var reagent in sol.Comp.Solution.Contents.ToArray())
            {
                solutionSystem.TryAddReagent(sol, reagent.Reagent, bonus, out _);
            }
        }
    }

    private static void ScaleDamageComponents(EntityManager entManager, EntityUid item, float multiplier)
    {
        if (entManager.TryGetComponent<MeleeWeaponComponent>(item, out var melee))
            melee.Damage *= multiplier;

        if (entManager.TryGetComponent<ProjectileComponent>(item, out var proj))
            proj.Damage *= multiplier;
    }
}
