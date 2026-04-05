using Content.Shared._OE14.ModularCraft;
using Content.Shared._OE14.ModularCraft.Components;
using Content.Shared.Armor;
using Content.Shared.Damage;

namespace Content.Server._OE14.ModularCraft.Modifiers;

public sealed partial class EditArmor : OE14ModularCraftModifier
{
    [DataField(required: true)]
    public DamageModifierSet Modifiers = new();

    public override void Effect(EntityManager entManager, Entity<OE14ModularCraftStartPointComponent> start, Entity<OE14ModularCraftPartComponent>? part)
    {
        if (!entManager.TryGetComponent<ArmorComponent>(start, out var armor))
            return;

        var armorSystem = entManager.System<SharedArmorSystem>();

        armorSystem.EditArmorCoefficients(start, Modifiers, armor);

        // Synchronize component changes back to the entity
        entManager.Dirty(armor);
    }
}
