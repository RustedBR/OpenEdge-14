using Content.Shared._OE14.MagicRitual.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicManacostModify;

/// <summary>
/// Changes the manacost of spells for the bearer
/// </summary>
[RegisterComponent]
public sealed partial class OE14MagicManacostModifyComponent : Component
{
    [DataField]
    public FixedPoint2 GlobalModifier = 1f;

    [DataField]
    public bool Examinable = false;
}
