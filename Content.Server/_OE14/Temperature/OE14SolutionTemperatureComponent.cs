namespace Content.Server._OE14.Temperature;

/// <summary>
/// passively returns the solution temperature to the standard
/// </summary>
[RegisterComponent, Access(typeof(OE14TemperatureSystem))]
public sealed partial class OE14SolutionTemperatureComponent : Component
{
    [DataField]
    public float StandardTemp = 300f;
}
