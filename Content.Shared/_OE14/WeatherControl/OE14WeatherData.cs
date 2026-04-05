using Content.Shared.Destructible.Thresholds;
using Content.Shared.Weather;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.WeatherControl;

[DataRecord, Serializable]
public sealed class OE14WeatherData
{
    [DataField(required: true)]
    public ProtoId<WeatherPrototype>? Visuals { get; set; } = null;

    [DataField]
    public MinMax Duration { get; set; } = new(120, 600);

    [DataField]
    public float Weight { get; set; } = 1f;
}
