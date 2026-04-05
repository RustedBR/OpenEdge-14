using Content.Shared._OE14.ModularCraft.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.ModularCraft.Components;

[RegisterComponent, Access(typeof(OE14SharedModularCraftSystem))]
public sealed partial class OE14ModularCraftPartComponent : Component
{
    [DataField(required: true)]
    public HashSet<ProtoId<OE14ModularCraftPartPrototype>> PossibleParts = new();

    [DataField]
    public float DoAfter = 1f;

    /// <summary>
    /// Attaching this piece adds an additional price to the target object.
    /// </summary>
    [DataField]
    public double AddPrice = 5;

    //TODO: Sound
}
