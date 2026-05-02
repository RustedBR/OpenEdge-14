using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;

namespace Content.Shared._OE14.MagicSpell.Spells;

/// <summary>
///     Adjusts the thermal energy of all the solutions inside the container.
/// </summary>
public sealed partial class OE14AdjustAllSolutionThermalEnergy : OE14SpellEffect
{
    /// <summary>
    ///     The change in energy.
    /// </summary>
    [DataField(required: true)]
    public float Delta;

    /// <summary>
    ///     The minimum temperature this effect can reach.
    /// </summary>
    [DataField]
    public float? MinTemp;

    /// <summary>
    ///     The maximum temperature this effect can reach.
    /// </summary>
    [DataField]
    public float? MaxTemp;

    /// <summary>
    /// When true, multiplies Delta by the caster's INT multiplier.
    /// INT 5 = 1.0x, INT 10 = 1.5x, INT 1 = 0.5x.
    /// </summary>
    [DataField]
    public bool ScaleWithCasterInt = false;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        var solutionContainer = entManager.System<SharedSolutionContainerSystem>();

        if (!entManager.TryGetComponent<SolutionContainerManagerComponent>(args.Target, out var solutionComp))
            return;

        var effectiveDelta = Delta;

        if (ScaleWithCasterInt &&
            args.User is not null &&
            entManager.TryGetComponent<OE14CharacterStatsComponent>(args.User.Value, out var stats))
        {
            var effInt = Math.Clamp(stats.Intelligence + stats.IntelligenceModifier, 1, stats.MaxStatValue);
            var multiplier = effInt >= 5
                ? 1.0f + (effInt - 5) * 0.10f
                : 1.0f + (effInt - 5) * 0.125f;
            effectiveDelta *= multiplier;
        }

        var target = new Entity<SolutionContainerManagerComponent?>(args.Target.Value, solutionComp);
        foreach (var (_, solution) in solutionContainer.EnumerateSolutions(target))
        {
            if (solution.Comp.Solution.Volume == 0)
                continue;

            var maxTemp = MaxTemp ?? float.PositiveInfinity;
            var minTemp = Math.Max(MinTemp ?? 0, 0);
            var oldTemp = solution.Comp.Solution.Temperature;

            if (effectiveDelta > 0 && oldTemp >= maxTemp)
                continue;
            if (effectiveDelta < 0 && oldTemp <= minTemp)
                continue;

            var heatCap = solution.Comp.Solution.GetHeatCapacity(null);
            var deltaT = effectiveDelta / heatCap;

            solutionContainer.SetTemperature(solution, Math.Clamp(oldTemp + deltaT, minTemp, maxTemp));
        }
    }
}
