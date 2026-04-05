namespace Content.Shared._OE14.WeatherEffect;

/// <summary>
/// Allows weather to fill said solution with liquids if said entity is outdoors
/// </summary>
[RegisterComponent]
public sealed partial class OE14WeatherRefillableComponent : Component
{
    [DataField(required: true)]
    public string Solution = string.Empty;
}
