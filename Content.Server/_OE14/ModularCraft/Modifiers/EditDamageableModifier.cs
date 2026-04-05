using Content.Shared._OE14.Damageable;
using Content.Shared._OE14.ModularCraft;
using Content.Shared._OE14.ModularCraft.Components;

namespace Content.Server._OE14.ModularCraft.Modifiers;

public sealed partial class EditDamageableModifier : OE14ModularCraftModifier
{
    [DataField(required: true)]
    public float Multiplier = 1f;

    public override void Effect(EntityManager entManager, Entity<OE14ModularCraftStartPointComponent> start, Entity<OE14ModularCraftPartComponent>? part)
    {
        var damageable = entManager.EnsureComponent<OE14DamageableModifierComponent>(start);

        damageable.Modifier *= Multiplier;
        entManager.Dirty(start);
    }
}
