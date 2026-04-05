using Content.Server.Temperature.Systems;

namespace Content.Server._OE14.Temperature;

/// <summary>
/// Adds thermal energy from FlammableComponent to entities with <see cref="TemperatureComponent"/> placed on it.
/// </summary>
[RegisterComponent, Access(typeof(EntityHeaterSystem))]
public sealed partial class OE14FlammableEntityHeaterComponent : Component
{
    [DataField]
    public float DegreesPerStack = 300f;
}
