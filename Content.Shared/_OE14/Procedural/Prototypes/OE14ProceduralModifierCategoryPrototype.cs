using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Procedural.Prototypes;

/// <summary>
///
/// </summary>
[Prototype("oe14LocationModifierCategory")]
public sealed partial class OE14ProceduralModifierCategoryPrototype : IPrototype
{
    [IdDataField] public string ID { get; } = default!;
}
