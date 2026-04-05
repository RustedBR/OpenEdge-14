namespace Content.Shared._OE14.Wallmount;

/// <summary>
/// Automatically attaches the entity to the wall when it appears, or removes it
/// </summary>
[RegisterComponent, Access(typeof(OE14WallmountSystem))]
public sealed partial class OE14WallmountComponent : Component
{
    [DataField]
    public int AttachAttempts = 3;

    [DataField]
    public TimeSpan NextAttachTime = TimeSpan.Zero;
}
