using Content.Shared._OE14.Farming.Components;
using Content.Shared._OE14.Farming.Prototypes;
using Content.Shared.FixedPoint;

namespace Content.Shared._OE14.Farming.Metabolizer;

public sealed partial class AffectPlantValues : OE14MetabolizerEffect
{
    [DataField]
    public float Energy = 0f;
    [DataField]
    public float Resource = 0f;
    [DataField]
    public float Growth = 0f;

    public override void Effect(Entity<OE14PlantComponent> plant, FixedPoint2 amount, EntityManager entityManager)
    {
        var farmingSystem = entityManager.System<OE14SharedFarmingSystem>();

        farmingSystem.AffectEnergy(plant, Energy * (float)amount);
        farmingSystem.AffectResource(plant,Resource * (float)amount);
        farmingSystem.AffectGrowth(plant, Growth * (float)amount);
    }
}
