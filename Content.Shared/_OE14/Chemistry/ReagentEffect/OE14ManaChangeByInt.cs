using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared._OE14.MagicEnergy;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Chemistry.ReagentEffect;

/// <summary>
/// Mana regeneration effect that scales with Intelligence stat.
/// Regenerates: baseAmount + (Intelligence - 5)
/// Example: INT 10 → 2 + 5 = 7 mana per tick
/// </summary>
[UsedImplicitly]
[DataDefinition]
public sealed partial class OE14ManaChangeByInt : EntityEffect
{
    [DataField(required: true)]
    public float BaseAmount = 2;

    [DataField]
    public bool Safe;

    [DataField]
    public bool ScaleByQuantity;

    protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("oe14-reagent-effect-guidebook-mana-add",
            ("chance", Probability),
            ("amount", BaseAmount));
    }

    public override void Effect(EntityEffectBaseArgs args)
    {
        var entityManager = args.EntityManager;
        var scale = FixedPoint2.New(1);

        if (args is EntityEffectReagentArgs reagentArgs)
            scale = ScaleByQuantity ? reagentArgs.Quantity * reagentArgs.Scale : reagentArgs.Scale;

        // Calculate mana delta based on INT: baseAmount + (INT - 5)
        var manaDelta = BaseAmount;
        if (entityManager.TryGetComponent<OE14CharacterStatsComponent>(args.TargetEntity, out var stats))
        {
            var intBonus = stats.Intelligence - 5;
            manaDelta += intBonus;
        }

        var magicSystem = entityManager.System<OE14SharedMagicEnergySystem>();
        magicSystem.ChangeEnergy(args.TargetEntity, manaDelta * scale, out var changed, out var overload, safe: Safe);

        scale -= FixedPoint2.Abs(changed + overload);

        if (!args.EntityManager.TryGetComponent<OE14MagicEnergyCrystalSlotComponent>(args.TargetEntity, out var crystalSlot))
            return;

        var slotSystem = entityManager.System<SharedOE14MagicEnergyCrystalSlotSystem>();
        slotSystem.TryChangeEnergy((args.TargetEntity, crystalSlot), manaDelta * scale);
    }
}
