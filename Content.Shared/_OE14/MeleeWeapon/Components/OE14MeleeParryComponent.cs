namespace Content.Shared._OE14.MeleeWeapon.Components;

/// <summary>
/// attacks with this item may knock OE14ParriableComponent items out of your hand on a hit
/// </summary>
[RegisterComponent]
public sealed partial class OE14MeleeParryComponent : Component
{
    [DataField]
    public TimeSpan ParryWindow = TimeSpan.FromSeconds(1f);

    [DataField]
    public float ParryPower = 1f;
}
