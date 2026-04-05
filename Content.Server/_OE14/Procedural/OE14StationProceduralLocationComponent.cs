using Content.Shared._OE14.Procedural.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.Procedural;

/// <summary>
/// Generates the surrounding procedural world on the game map, surrounding the mapped settlement.
/// </summary>
[RegisterComponent, Access(typeof(OE14LocationGenerationSystem))]
public sealed partial class OE14StationProceduralLocationComponent : Component
{
    [DataField(required: true)]
    public ProtoId<OE14ProceduralLocationPrototype> Location;

    [DataField]
    public List<ProtoId<OE14ProceduralModifierPrototype>> Modifiers = [];

    [DataField]
    public Vector2i GenerationOffset = Vector2i.Zero;
}
