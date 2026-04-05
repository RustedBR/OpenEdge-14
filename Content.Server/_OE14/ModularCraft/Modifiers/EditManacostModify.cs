using Content.Shared._OE14.ModularCraft;
using Content.Shared._OE14.ModularCraft.Components;
using Content.Shared._OE14.MagicManacostModify;
using Content.Shared._OE14.MagicRitual.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.ModularCraft.Modifiers;

public sealed partial class EditManacostModify : OE14ModularCraftModifier
{
    [DataField]
    public FixedPoint2 GlobalModifier = 1f;

    public override void Effect(EntityManager entManager, Entity<OE14ModularCraftStartPointComponent> start, Entity<OE14ModularCraftPartComponent>? part)
    {
        if (!entManager.TryGetComponent<OE14MagicManacostModifyComponent>(start, out var manacostModifyComp))
            return;

        manacostModifyComp.GlobalModifier += GlobalModifier;
    }
}
