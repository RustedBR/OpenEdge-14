namespace Content.Server._OE14.MagicSpellStorage.Components;

/// <summary>
/// Denotes that this item's spells can be accessed while wearing it on your body
/// </summary>
[RegisterComponent, Access(typeof(OE14SpellStorageSystem))]
public sealed partial class OE14SpellStorageAccessWearingComponent : Component
{
    [DataField]
    public bool Wearing;
}
