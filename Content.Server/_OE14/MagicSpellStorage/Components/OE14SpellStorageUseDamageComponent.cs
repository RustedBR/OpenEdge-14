using Content.Shared.Damage;

namespace Content.Server._OE14.MagicSpellStorage.Components;

/// <summary>
/// Causes damage to the Spell storage when spells from it are used
/// </summary>
[RegisterComponent, Access(typeof(OE14SpellStorageSystem))]
public sealed partial class OE14SpellStorageUseDamageComponent : Component
{
    /// <summary>
    /// the amount of damage this entity will take per unit manacost of the spell used
    /// </summary>
    [DataField(required: true)]
    public DamageSpecifier DamagePerMana = default!;
}
