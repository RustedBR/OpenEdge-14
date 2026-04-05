
namespace Content.Server._OE14.Alchemy;

[RegisterComponent, Access(typeof(OE14AlchemyExtractionSystem))]
public sealed partial class OE14MortarComponent : Component
{
    [DataField(required: true)]
    public string Solution = string.Empty;

    [DataField(required: true)]
    public string ContainerId = string.Empty;
}

