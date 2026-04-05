namespace Content.Client._OE14.Wave;

[RegisterComponent]
[Access(typeof(OE14WaveShaderSystem))]
public sealed partial class OE14WaveShaderComponent : Component
{
    [DataField]
    public float Speed = 10f;

    [DataField]
    public float Dis = 10f;

    [DataField]
    public float Offset = 0f;
}
