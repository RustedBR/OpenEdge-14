namespace Content.Shared._OE14.DayCycle;


[RegisterComponent]
public sealed partial class OE14DayCycleComponent : Component
{
    public float LastLightLevel = 0f;

    [DataField]
    public float Threshold = 0.6f;
}
