/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Server._OE14.BiomeSpawner.EntitySystems;
using Content.Shared.Parallax.Biomes;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.BiomeSpawner.Components;

/// <summary>
/// fills the tile in which it is located with the contents of the biome. Includes: tile, decals and entities
/// </summary>
[RegisterComponent, Access(typeof(OE14BiomeSpawnerSystem))]
public sealed partial class OE14BiomeSpawnerComponent : Component
{
    [DataField]
    public ProtoId<BiomeTemplatePrototype> Biome = "Grasslands";

    /// <summary>
    /// entities that we don't remove.
    /// </summary>
    [DataField(required: true)]
    public EntityWhitelist DeleteBlacklist = new();
}
