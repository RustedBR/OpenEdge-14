using Content.Server.Cargo.Components;
using Content.Server.Item;
using Content.Shared._OE14.ModularCraft;
using Content.Shared._OE14.ModularCraft.Components;
using Content.Shared._OE14.ModularCraft.Prototypes;
using Content.Shared.Cargo.Components;
using Content.Shared.Throwing;
using Content.Shared.Examine;
using Content.Shared.Materials;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server._OE14.ModularCraft;

public sealed class OE14ModularCraftSystem : OE14SharedModularCraftSystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ItemSystem _item = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14ModularCraftStartPointComponent, MapInitEvent>(OnStartPointMapInit);
        SubscribeLocalEvent<OE14ModularCraftStartPointComponent, OE14ModularCraftAddPartDoAfter>(OnAddedToStart);
        SubscribeLocalEvent<OE14ModularCraftPartComponent, OE14ModularCraftAddPartDoAfter>(OnAddedToPart);
        SubscribeLocalEvent<OE14ModularCraftStartPointComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(Entity<OE14ModularCraftStartPointComponent> ent, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        var markup = GetExamine(ent.Comp);
        args.PushMessage(markup, -3);
    }

    private FormattedMessage GetExamine(OE14ModularCraftStartPointComponent comp)
    {
        var msg = new FormattedMessage();

        msg.AddMarkupOrThrow(Loc.GetString("oe14-modular-craft-examine-freeslots"));

        foreach (var slot in comp.FreeSlots)
        {
            if (!_proto.TryIndex(slot, out var slotProto))
                continue;

            msg.AddMarkupOrThrow("\n - " + Loc.GetString(slotProto.Name));
        }

        return msg;
    }

    private void OnAddedToStart(Entity<OE14ModularCraftStartPointComponent> start, ref OE14ModularCraftAddPartDoAfter args)
    {
        if (args.Cancelled || args.Handled)
            return;

        if (!TryComp<OE14ModularCraftPartComponent>(args.Used, out var partComp))
            return;

        if (!TryAddPartToFirstSlot(start, (args.Used.Value, partComp)))
            return;

        //TODO: Sound

        args.Handled = true;
    }

    private void OnAddedToPart(Entity<OE14ModularCraftPartComponent> part, ref OE14ModularCraftAddPartDoAfter args)
    {
        if (args.Cancelled || args.Handled)
            return;

        if (!TryComp<OE14ModularCraftStartPointComponent>(args.Used, out var startComp))
            return;

        if (!TryAddPartToFirstSlot((args.Used.Value, startComp), part))
            return;

        //TODO: Sound

        args.Handled = true;
    }

    private void OnStartPointMapInit(Entity<OE14ModularCraftStartPointComponent> ent, ref MapInitEvent args)
    {
        foreach (var startSlot in ent.Comp.StartSlots)
        {
            ent.Comp.FreeSlots.Add(startSlot);
        }

        if (TryComp<OE14ModularCraftAutoAssembleComponent>(ent, out var autoAssemble))
        {
            foreach (var detail in autoAssemble.Details)
            {
                var detailEnt = Spawn(detail);
                if (!TryComp<OE14ModularCraftPartComponent>(detailEnt, out var partComp))
                {
                    QueueDel(detailEnt);
                    continue;
                }
                TryAddPartToFirstSlot(ent, (detailEnt, partComp));
            }
        }
    }

    private bool TryAddPartToFirstSlot(Entity<OE14ModularCraftStartPointComponent> start,
        Entity<OE14ModularCraftPartComponent> part)
    {
        foreach (var partProto in part.Comp.PossibleParts)
        {
            if (!_proto.TryIndex(partProto, out var partIndexed))
                continue;

            if (partIndexed.Slots.Count == 0)
                continue;

            foreach (var slot in partIndexed.Slots)
            {
                if (!start.Comp.FreeSlots.Contains(slot))
                    continue;

                if (TryAddPartToSlot(start, part, partProto, slot))
                {
                    QueueDel(part);
                    return true;
                }
            }
        }

        return false;
    }

    private bool TryAddPartToSlot(Entity<OE14ModularCraftStartPointComponent> start,
        Entity<OE14ModularCraftPartComponent> part,
        ProtoId<OE14ModularCraftPartPrototype> partProto,
        ProtoId<OE14ModularCraftSlotPrototype> slot)
    {
        if (!start.Comp.FreeSlots.Contains(slot))
            return false;

        //TODO: Size changing broken in gridstorage

        AddPartToSlot(start, part, partProto, slot);
        return true;
    }

    private void AddPartToSlot(Entity<OE14ModularCraftStartPointComponent> start,
        Entity<OE14ModularCraftPartComponent> part,
        ProtoId<OE14ModularCraftPartPrototype> partProto,
        ProtoId<OE14ModularCraftSlotPrototype> slot)
    {
        start.Comp.FreeSlots.Remove(slot);
        start.Comp.InstalledParts.Add(partProto);

        var indexedPart = _proto.Index(partProto);

        if (TryComp<PhysicalCompositionComponent>(part, out var partMaterial))
        {
            var startMaterial = EnsureComp<PhysicalCompositionComponent>(start);

            //Merge materials
            foreach (var (material, count) in partMaterial.MaterialComposition)
            {
                if (startMaterial.MaterialComposition.TryGetValue(material, out var existingCount))
                    startMaterial.MaterialComposition[material] = existingCount + count;
                else
                    startMaterial.MaterialComposition[material] = count;
            }

            //Merge solutions
            foreach (var (sol, count) in partMaterial.ChemicalComposition)
            {
                if (startMaterial.ChemicalComposition.TryGetValue(sol, out var existingCount))
                    startMaterial.ChemicalComposition[sol] = existingCount + count;
                else
                    startMaterial.ChemicalComposition[sol] = count;
            }
        }

        var startStaticPrice = EnsureComp<StaticPriceComponent>(start);
        startStaticPrice.Price += part.Comp.AddPrice;

        if (TryComp<StaticPriceComponent>(part, out var partStaticPrice))
        {

            startStaticPrice.Price += partStaticPrice.Price;
        }

        foreach (var modifier in indexedPart.Modifiers)
        {
            modifier.Effect(EntityManager, start, part);
        }

        _item.VisualsChanged(start);
        Dirty(start);
    }

    public void DisassembleModular(EntityUid target)
    {
        if (!TryComp<OE14ModularCraftStartPointComponent>(target, out var modular))
            return;

        var sourceCoord = _transform.GetMapCoordinates(target);

        //Spawn start part
        if (modular.StartProtoPart is not null)
        {
            if (_random.Prob(0.5f)) //TODO: Dehardcode
            {
                var spawned = Spawn(modular.StartProtoPart, sourceCoord);
                _throwing.TryThrow(spawned, _random.NextAngle().ToWorldVec(), 1f);
            }
        }

        //Spawn parts
        foreach (var part in modular.InstalledParts)
        {
            if (!_proto.TryIndex(part, out var indexedPart))
                continue;

            if (_random.Prob(indexedPart.DestroyProb))
                continue;

            if (indexedPart.SourcePart is null)
                continue;

            var spawned = Spawn(indexedPart.SourcePart, sourceCoord);
            _throwing.TryThrow(spawned, _random.NextAngle().ToWorldVec(), 1f);
        }

        //Delete
        QueueDel(target);
    }
}
