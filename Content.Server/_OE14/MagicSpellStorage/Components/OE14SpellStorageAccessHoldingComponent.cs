namespace Content.Server._OE14.MagicSpellStorage.Components;

/// <summary>
/// Denotes that this item's spells can be accessed while holding it in your hand
/// </summary>
[RegisterComponent, Access(typeof(OE14SpellStorageSystem))]
public sealed partial class OE14SpellStorageAccessHoldingComponent : Component
{
}
