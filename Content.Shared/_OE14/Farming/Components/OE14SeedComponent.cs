using Content.Shared.Maps;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Farming.Components;

/// <summary>
/// a component that allows for the creation of the entity on the tile
/// </summary>
[RegisterComponent, Access(typeof(OE14SharedFarmingSystem))]
public sealed partial class OE14SeedComponent : Component
{
    [DataField]
    public TimeSpan PlantingTime = TimeSpan.FromSeconds(1f);

    [DataField(required: true)]
    public EntProtoId PlantProto;

    [DataField]
    public HashSet<ProtoId<ContentTileDefinition>> SoilTile = new() { "OE14FloorDirtSeedbed" };
}
