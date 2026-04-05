namespace Content.Server._OE14.Temperature;

/// <summary>
/// allows you to heat the temperature of solutions depending on the number of stacks of fire
/// </summary>
[RegisterComponent, Access(typeof(OE14TemperatureSystem))]
public sealed partial class OE14FlammableSolutionHeaterComponent : Component
{
    [DataField]
    public float DegreesPerStack = 100f;
}
