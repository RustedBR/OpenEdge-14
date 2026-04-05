using Content.Shared.EntityEffects;

namespace Content.Server._OE14.StatusEffect;

/// <summary>
/// Applies Entity Effects at a given frequency
/// </summary>
[RegisterComponent, AutoGenerateComponentState, Access(typeof(OE14EntityEffectsStatusEffectSystem))]

public sealed partial class OE14EntityEffectsStatusEffectComponent : Component
{
    /// <summary>
    /// List of Effects that will be applied
    /// </summary>
    [DataField]
    public List<EntityEffect> Effects = [];

    /// <summary>
    /// How often objects will try to apply <see cref="Effects"/>. In Seconds.
    /// </summary>
    [DataField]
    public TimeSpan Frequency = TimeSpan.FromSeconds(5);

    /// <summary>
    /// The time of the next Effect trigger
    /// </summary>
    [DataField]
    public TimeSpan NextUpdateTime { get; set; } = TimeSpan.Zero;
}
