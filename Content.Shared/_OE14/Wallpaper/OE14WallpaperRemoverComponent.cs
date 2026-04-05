namespace Content.Shared._OE14.Wallpaper;

/// <summary>
/// After a delay, it removes all wallpaper from the entity.
/// </summary>
[RegisterComponent, Access(typeof(OE14SharedWallpaperSystem))]
public sealed partial class OE14WallpaperRemoverComponent : Component
{
    [DataField]
    public float Delay = 1f;
}
