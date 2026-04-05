using Content.Shared._OE14.ModularCraft;
using Content.Shared._OE14.ModularCraft.Components;
using Content.Shared._OE14.ModularCraft.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.ModularCraft.Modifiers;

public sealed partial class EditModularSlots : OE14ModularCraftModifier
{
    [DataField]
    public HashSet<ProtoId<OE14ModularCraftSlotPrototype>> AddSlots = new();

    [DataField]
    public HashSet<ProtoId<OE14ModularCraftSlotPrototype>> RemoveSlots = new();

    public override void Effect(EntityManager entManager, Entity<OE14ModularCraftStartPointComponent> start, Entity<OE14ModularCraftPartComponent>? part)
    {
        start.Comp.FreeSlots.AddRange(AddSlots);
        foreach (var slot in RemoveSlots)
        {
            if (start.Comp.FreeSlots.Contains(slot))
                start.Comp.FreeSlots.Remove(slot);
        }
    }
}
