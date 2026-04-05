namespace Content.Shared._OE14.Wallpaper;

/// <summary>
/// After a delay, it adds a new layer of wallpaper, depending on the player's relative position to the wall
/// </summary>
[RegisterComponent, Access(typeof(OE14SharedWallpaperSystem))]
public sealed partial class OE14WallpaperComponent : Component
{
    [DataField]
    public float Delay = 1f;

    [DataField(required: true)]
    public string RsiPath = default!;

    [DataField]
    public string Bottom = "bottom";
    [DataField]
    public string Top = "top";
    [DataField]
    public string Left = "left";
    [DataField]
    public string Right = "right";
}
