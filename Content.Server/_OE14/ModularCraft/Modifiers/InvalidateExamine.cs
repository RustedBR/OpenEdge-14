using Content.Shared._OE14.ModularCraft;
using Content.Shared._OE14.ModularCraft.Components;
using Content.Shared.Damage.Components;
using Content.Shared.Weapons.Melee;

namespace Content.Server._OE14.ModularCraft.Modifiers;

/// <summary>
/// Ensures examine information remains visible after modular parts are attached.
/// Guarantees DamageExaminableComponent is present and MeleeWeapon is not hidden.
/// </summary>
public sealed partial class InvalidateExamine : OE14ModularCraftModifier
{
    public override void Effect(EntityManager entManager, Entity<OE14ModularCraftStartPointComponent> start, Entity<OE14ModularCraftPartComponent>? part)
    {
        // Ensure DamageExaminable component is present (for damage display)
        entManager.EnsureComponent<DamageExaminableComponent>(start);

        // Ensure MeleeWeaponComponent is not hidden so damage is visible in examine
        if (entManager.TryGetComponent<MeleeWeaponComponent>(start, out var melee))
        {
            melee.Hidden = false;
            entManager.Dirty(start, melee);
        }

        // Mark entity as dirty to refresh examine text
        entManager.Dirty(start);
    }
}
