
namespace Content.Server._OE14.Demiplane.Components;

/// <summary>
///
/// </summary>
[RegisterComponent, Access(typeof(OE14DemiplaneSystem))]
public sealed partial class OE14DemiplaneMapComponent : Component
{
    [DataField]
    public Vector2i Position = Vector2i.Zero;
}
