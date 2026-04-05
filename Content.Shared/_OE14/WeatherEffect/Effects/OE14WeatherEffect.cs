using JetBrains.Annotations;
using Robust.Shared.Random;

namespace Content.Shared._OE14.WeatherEffect.Effects;

[ImplicitDataDefinitionForInheritors]
[MeansImplicitUse]
public abstract partial class OE14WeatherEffect
{
    [DataField]
    public float Prob = 0.05f;

    public abstract void ApplyEffect(IEntityManager entManager, IRobustRandom random, EntityUid target);
}

[DataDefinition]
public sealed partial class OE14WeatherEffectConfig
{
    [DataField]
    public List<OE14WeatherEffect> Effects = new();

    [DataField]
    public int? MaxEntities = null;

    [DataField]
    public TimeSpan Frequency = TimeSpan.FromSeconds(5f);

    [DataField]
    public TimeSpan NextEffectTime = TimeSpan.Zero;

    [DataField]
    public bool CanAffectOnWeatherBlocker = true;
}
