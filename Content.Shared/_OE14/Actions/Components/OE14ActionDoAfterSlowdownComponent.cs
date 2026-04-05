namespace Content.Shared._OE14.Actions.Components;

/// <summary>
/// Slows the caster while using action
/// </summary>
[RegisterComponent, Access(typeof(OE14SharedActionSystem))]
public sealed partial class OE14ActionDoAfterSlowdownComponent : Component
{
    [DataField]
    public float SpeedMultiplier = 1f;
}
