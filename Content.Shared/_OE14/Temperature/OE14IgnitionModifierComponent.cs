namespace Content.Shared._OE14.Temperature;

/// <summary>
/// Modifies how the entity is set on fire. Given that in oe14 the ignition is done via doAfter by a separate add-on
/// </summary>
[RegisterComponent, Access(typeof(OE14SharedFireSpreadSystem))]
public sealed partial class OE14IgnitionModifierComponent : Component
{
    /// <summary>
    /// Allows you to slow down or speed up doAfter the burning of this entity. (Example: candles are lit quickly, they should have a high value)
    /// </summary>
    [DataField]
    public float IgnitionTimeModifier = 1f;

    /// <summary>
    /// Should burning this entity warn other players? Put false if it is safe to ignite this entity.
    /// </summary>
    [DataField]
    public bool HideCaution = false;
}
