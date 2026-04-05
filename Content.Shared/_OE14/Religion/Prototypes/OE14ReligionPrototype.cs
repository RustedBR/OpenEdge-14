using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Religion.Prototypes;

/// <summary>
///
/// </summary>
[Prototype("oe14Religion")]
public sealed partial class OE14ReligionPrototype : IPrototype
{
    [IdDataField] public string ID { get; } = default!;

    [DataField]
    public float FollowerObservationRadius = 10f;
}
