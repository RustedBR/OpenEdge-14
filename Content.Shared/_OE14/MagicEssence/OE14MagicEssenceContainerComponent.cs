using Content.Shared._OE14.MagicRitual.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicEssence;

/// <summary>
/// Reflects the amount of essence stored in this item. The item can be destroyed to release the essence from it.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(OE14MagicEssenceSystem))]
public sealed partial class OE14MagicEssenceContainerComponent : Component
{
    [DataField]
    public Dictionary<ProtoId<OE14MagicTypePrototype>, int> Essences = new();
}
