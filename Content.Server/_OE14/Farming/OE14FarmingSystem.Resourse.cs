using Content.Shared._OE14.DayCycle;
using Content.Shared._OE14.Farming.Components;
using Content.Shared.Chemistry.Components.SolutionManager;

namespace Content.Server._OE14.Farming;

public sealed partial class OE14FarmingSystem
{
    [Dependency] private readonly OE14DayCycleSystem _dayCycle = default!;
    private void InitializeResources()
    {
        SubscribeLocalEvent<OE14PlantEnergyFromLightComponent, OE14PlantUpdateEvent>(OnTakeEnergyFromLight);
        SubscribeLocalEvent<OE14PlantMetabolizerComponent, OE14PlantUpdateEvent>(OnPlantMetabolizing);

        SubscribeLocalEvent<OE14PlantGrowingComponent, OE14AfterPlantUpdateEvent>(OnPlantGrowing);
    }

    private void OnTakeEnergyFromLight(Entity<OE14PlantEnergyFromLightComponent> regeneration, ref OE14PlantUpdateEvent args)
    {
        var gainEnergy = false;
        var daylight = _dayCycle.UnderSunlight(regeneration);

        if (regeneration.Comp.Daytime && daylight)
            gainEnergy = true;

        if (regeneration.Comp.Nighttime && !daylight)
            gainEnergy = true;

        if (gainEnergy)
            args.EnergyDelta += regeneration.Comp.Energy;
    }

    private void OnPlantGrowing(Entity<OE14PlantGrowingComponent> growing, ref OE14AfterPlantUpdateEvent args)
    {
        if (args.Plant.Comp.Energy < growing.Comp.EnergyCost)
            return;

        if (args.Plant.Comp.Resource < growing.Comp.ResourceCost)
            return;

        if (args.Plant.Comp.GrowthLevel >= 1)
            return;

        AffectEnergy(args.Plant, -growing.Comp.EnergyCost);
        AffectResource(args.Plant, -growing.Comp.ResourceCost);

        AffectGrowth(args.Plant, growing.Comp.GrowthPerUpdate);
    }

    private void OnPlantMetabolizing(Entity<OE14PlantMetabolizerComponent> ent, ref OE14PlantUpdateEvent args)
    {
        if (!PlantQuery.TryComp(ent, out var plant) ||
            !SolutionQuery.TryComp(args.Plant, out var solmanager))
            return;

        var solEntity = new Entity<SolutionContainerManagerComponent?>(args.Plant, solmanager);
        if (!_solutionContainer.TryGetSolution(solEntity, plant.Solution, out var soln, out _))
            return;

        if (!_proto.TryIndex(ent.Comp.MetabolizerId, out var metabolizer))
            return;

        var splitted = _solutionContainer.SplitSolution(soln.Value, ent.Comp.SolutionPerUpdate);
        foreach (var reagent in splitted)
        {
            if (!metabolizer.Metabolization.ContainsKey(reagent.Reagent.ToString()))
                continue;

            foreach (var effect in metabolizer.Metabolization[reagent.Reagent.ToString()])
            {
                effect.Effect((ent, plant), reagent.Quantity, EntityManager);
            }
        }
    }
}
