using Content.Shared._OE14.MeleeWeapon.Components;
using Content.Shared._OE14.ModularCraft;
using Content.Shared._OE14.ModularCraft.Components;

namespace Content.Server._OE14.ModularCraft.Modifiers;

public sealed partial class EditSharpened : OE14ModularCraftModifier
{
    [DataField]
    public float SharpnessDamageMultiplier = 1f;
    public override void Effect(EntityManager entManager, Entity<OE14ModularCraftStartPointComponent> start, Entity<OE14ModularCraftPartComponent>? part)
    {
        if (!entManager.TryGetComponent<OE14SharpenedComponent>(start, out var sharpened))
            return;

        sharpened.SharpnessDamageBy1Damage *= SharpnessDamageMultiplier;
    }
}
