using Content.Server._OE14.ZLevels.Commands;
using Content.Server._OE14.ZLevels.EntitySystems;
using Robust.Shared.Map;
using Robust.Shared.Utility;

namespace Content.Server._OE14.ZLevels.Components;

/// <summary>
/// Initializes the z-level system by creating a series of linked maps
/// </summary>
[RegisterComponent, Access(typeof(OE14StationZLevelsSystem), typeof(OE14CombineMapsIntoZLevelsCommand))]
public sealed partial class OE14StationZLevelsComponent : Component
{
    [DataField(required: true)]
    public int DefaultMapLevel = 0;

    [DataField(required: true)]
    public Dictionary<int, OE14ZLevelEntry> Levels = new();

    public bool Initialized = false;

    public Dictionary<MapId, int> LevelEntities = new();
}

[DataRecord, Serializable]
public sealed class OE14ZLevelEntry
{
    public ResPath? Path { get; set; } = null;
}
