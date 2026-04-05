using Content.Shared._OE14.ModularCraft;
using Content.Shared._OE14.ModularCraft.Components;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.ModularCraft.Modifiers;

public sealed partial class AddComponents : OE14ModularCraftModifier
{
    [DataField(required: true)]
    public ComponentRegistry Components = new();

    [DataField]
    public bool Override = false;

    public override void Effect(EntityManager entManager, Entity<OE14ModularCraftStartPointComponent> start, Entity<OE14ModularCraftPartComponent>? part)
    {
        if (Components is null)
            return;

        entManager.AddComponents(start, Components, Override);
    }
}
