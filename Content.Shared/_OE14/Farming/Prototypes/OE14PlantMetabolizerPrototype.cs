using Content.Shared._OE14.Farming.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Farming.Prototypes;

/// <summary>
/// Allows the plant to drink chemicals. The effect of the drank reagents depends on the selected metabolizer.
/// </summary>
[Prototype("OE14PlantMetabolizer")]
public sealed partial class OE14PlantMetabolizerPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = string.Empty;

    [DataField]
    public Dictionary<ProtoId<ReagentPrototype>, HashSet<OE14MetabolizerEffect>> Metabolization = new();
}

[ImplicitDataDefinitionForInheritors]
[MeansImplicitUse]
public abstract partial class OE14MetabolizerEffect
{
    public abstract void Effect(Entity<OE14PlantComponent> plant, FixedPoint2 amount, EntityManager entityManager);
}
