using Robust.Shared.GameStates;

namespace Content.Shared._OE14.MagicEssence;

/// <summary>
/// Reflects the amount of essence stored in this item. The item can be destroyed to release the essence from it.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(OE14MagicEssenceSystem))]
public sealed partial class OE14MagicEssenceCollectorComponent : Component
{
    [DataField]
    public float CollectRange = 1f;

    [DataField]
    public float AttractRange = 5f;

    [DataField]
    public string Solution = "collector";
}
