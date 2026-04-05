using Content.Server._OE14.GameTicking.Rules;
using Content.Shared._OE14.WeatherControl;

namespace Content.Server._OE14.WeatherControl;

/// <summary>
/// is the controller that hangs on the prototype map. It regulates which weather rules are run and where they are run.
/// </summary>
[RegisterComponent, AutoGenerateComponentPause, Access(typeof(OE14WeatherControllerSystem), typeof(OE14WeatherRule))]
public sealed partial class OE14WeatherControllerComponent : Component
{
    [DataField]
    public bool Enabled = true;

    [DataField]
    public HashSet<OE14WeatherData> Entries = new();

    [DataField, AutoPausedField]
    public TimeSpan NextWeatherTime = TimeSpan.Zero;
}
