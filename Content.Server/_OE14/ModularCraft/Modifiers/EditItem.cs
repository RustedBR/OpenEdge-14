using System.Linq;
using Content.Shared._OE14.ModularCraft;
using Content.Shared._OE14.ModularCraft.Components;
using Content.Shared.Item;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.ModularCraft.Modifiers;

public sealed partial class EditItem : OE14ModularCraftModifier
{
    [DataField]
    public ProtoId<ItemSizePrototype>? NewSize;

    /// <summary>
    /// Only works if the item has 1 shape. Increases or decreases it size.
    /// </summary>
    [DataField]
    public Vector2i? AdjustShape;

    [DataField]
    public Vector2i? StoredOffsetBonus;

    public override void Effect(EntityManager entManager, Entity<OE14ModularCraftStartPointComponent> start, Entity<OE14ModularCraftPartComponent>? part)
    {
        if (!entManager.TryGetComponent<ItemComponent>(start, out var itemComp) || itemComp.Shape is null)
            return;

        var itemSystem = entManager.System<SharedItemSystem>();

        if (NewSize is not null)
            itemSystem.SetSize(start, NewSize.Value);

        var itemShape = itemSystem.GetItemShape((start, itemComp));
        if (AdjustShape is not null && itemShape.Count == 1)
        {
            var box = itemComp.Shape.First();
            box.Right += AdjustShape.Value.X;
            box.Top += AdjustShape.Value.Y;
            itemSystem.SetShape(start, new List<Box2i>{box});
        }

        if (StoredOffsetBonus is not null)
        {
            var newOffset = itemComp.StoredOffset + StoredOffsetBonus.Value;
            itemSystem.SetStoredOffset(start, newOffset, itemComp);
        }
    }
}
