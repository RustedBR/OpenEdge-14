using Content.Shared._OE14.ModularCraft;
using Content.Shared._OE14.ModularCraft.Components;
using Content.Shared.Armor;
using Content.Shared.Clothing;
using Content.Shared.Damage;

namespace Content.Server._OE14.ModularCraft.Modifiers;

public sealed partial class EditClothingSpeed : OE14ModularCraftModifier
{
    [DataField]
    public float WalkModifier = 1f;

    [DataField]
    public float SprintModifier = 1f;

    public override void Effect(EntityManager entManager, Entity<OE14ModularCraftStartPointComponent> start, Entity<OE14ModularCraftPartComponent>? part)
    {
        if (!entManager.TryGetComponent<ClothingSpeedModifierComponent>(start, out var speed))
            return;

        var speedModifierSystem = entManager.System<ClothingSpeedModifierSystem>();

        speedModifierSystem.EditSpeedModifiers(start, WalkModifier, SprintModifier, speed);
    }
}
