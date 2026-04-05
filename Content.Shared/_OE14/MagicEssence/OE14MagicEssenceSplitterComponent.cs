using Content.Shared.Whitelist;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicEssence;

/// <summary>
///
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(OE14MagicEssenceSystem))]
public sealed partial class OE14MagicEssenceSplitterComponent : Component
{
    [DataField]
    public EntProtoId ImpactEffect = "OE14EssenceSplitterImpactEffect";

    [DataField]
    public float ThrowForce = 10f;

    [DataField]
    public EntityWhitelist? Whitelist;
}
