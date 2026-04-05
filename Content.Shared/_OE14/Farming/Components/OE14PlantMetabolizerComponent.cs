using Content.Shared._OE14.Farming.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Farming.Components;

/// <summary>
/// allows the plant to obtain resources by absorbing liquid from the ground
/// </summary>
[RegisterComponent, Access(typeof(OE14SharedFarmingSystem))]
public sealed partial class OE14PlantMetabolizerComponent : Component
{
    [DataField]
    public FixedPoint2 SolutionPerUpdate = 5f;

    [DataField(required: true)]
    public ProtoId<OE14PlantMetabolizerPrototype> MetabolizerId;
}
