using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._OE14.ModularCraft.Prototypes;

[Prototype("modularSlot")]
public sealed partial class OE14ModularCraftSlotPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public LocId Name = string.Empty;

    [DataField]
    public SpriteSpecifier? IconDisplacement { get; private set; }
}
