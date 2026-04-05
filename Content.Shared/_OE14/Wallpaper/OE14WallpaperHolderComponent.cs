using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared._OE14.Wallpaper;

/// <summary>
/// Stores all wallpapers added to the wall
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true), Access(typeof(OE14SharedWallpaperSystem))]
public sealed partial class OE14WallpaperHolderComponent : Component
{
    [DataField, AutoNetworkedField]
    public List<SpriteSpecifier> Layers = new();

    public HashSet<string> RevealedLayers = new();

    [DataField]
    public int MaxLayers = 4;
}
