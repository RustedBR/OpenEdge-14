using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Nutrition.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._OE14.WeatherEffect.Effects;

public sealed partial class RefillSolutions : OE14WeatherEffect
{
    [DataField(required: true)]
    public Dictionary<ProtoId<ReagentPrototype>, float> Reagents = new();

    public override void ApplyEffect(IEntityManager entManager, IRobustRandom random, EntityUid target)
    {
        if (!random.Prob(Prob))
            return;

        if (!entManager.TryGetComponent<OE14WeatherRefillableComponent>(target, out var refillable))
            return;

        if (entManager.TryGetComponent<OpenableComponent>(target, out var openable) && !openable.Opened)
            return;

        var solutionSystem = entManager.System<SharedSolutionContainerSystem>();

        solutionSystem.TryGetSolution(target, refillable.Solution, out var ent, out var solution);

        if (ent is null)
            return;

        foreach (var r in Reagents)
        {
            solutionSystem.TryAddReagent(ent.Value, r.Key, r.Value);
        }
    }
}
