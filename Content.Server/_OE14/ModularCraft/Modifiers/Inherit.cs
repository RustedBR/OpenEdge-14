using Content.Shared._OE14.ModularCraft;
using Content.Shared._OE14.ModularCraft.Components;
using Content.Shared._OE14.ModularCraft.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.ModularCraft.Modifiers;

public sealed partial class Inherit : OE14ModularCraftModifier
{
    [DataField(required: true)]
    public List<ProtoId<OE14ModularCraftPartPrototype>> CopyFrom = new();

    public override void Effect(EntityManager entManager, Entity<OE14ModularCraftStartPointComponent> start, Entity<OE14ModularCraftPartComponent>? part)
    {
        var prototypeManager = IoCManager.Resolve<IPrototypeManager>();

        foreach (var copy in CopyFrom)
        {
            foreach (var modifier in prototypeManager.Index(copy).Modifiers)
            {
                modifier.Effect(entManager, start, part);
            }
        }
    }
}
