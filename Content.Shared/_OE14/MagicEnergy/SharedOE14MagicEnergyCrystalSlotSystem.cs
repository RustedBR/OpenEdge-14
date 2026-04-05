using System.Diagnostics.CodeAnalysis;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Robust.Shared.Containers;

namespace Content.Shared._OE14.MagicEnergy;

public abstract class SharedOE14MagicEnergyCrystalSlotSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    [Dependency] private readonly OE14SharedMagicEnergySystem _magicEnergy = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14MagicEnergyCrystalSlotComponent, EntInsertedIntoContainerMessage>(OnCrystalInserted);
        SubscribeLocalEvent<OE14MagicEnergyCrystalSlotComponent, EntRemovedFromContainerMessage>(OnCrystalRemoved);

        SubscribeLocalEvent<OE14MagicEnergyCrystalComponent, OE14MagicEnergyLevelChangeEvent>(OnEnergyChanged);

        SubscribeLocalEvent<OE14MagicEnergyCrystalSlotComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<OE14MagicEnergyCrystalSlotComponent, OE14SlotCrystalChangedEvent>(OnCrystalChanged);
    }

    private void OnCrystalRemoved(Entity<OE14MagicEnergyCrystalSlotComponent> slot, ref EntRemovedFromContainerMessage args)
    {
        if (args.Container.ID != slot.Comp.SlotId)
            return;

        _appearance.SetData(slot, OE14MagicSlotVisuals.Inserted, false);
        _appearance.SetData(slot, OE14MagicSlotVisuals.Powered, false);
        RaiseLocalEvent(slot, new OE14SlotCrystalChangedEvent(true));
    }

    private void OnCrystalInserted(Entity<OE14MagicEnergyCrystalSlotComponent> slot, ref EntInsertedIntoContainerMessage args)
    {
        if (!slot.Comp.Initialized)
            return;

        if (args.Container.ID != slot.Comp.SlotId)
            return;

        _appearance.SetData(slot, OE14MagicSlotVisuals.Inserted, true);
        RaiseLocalEvent(slot, new OE14SlotCrystalChangedEvent(false));
    }

    public bool TryGetEnergyCrystalFromSlot(Entity<OE14MagicEnergyCrystalSlotComponent?> ent,
        [NotNullWhen(true)] out Entity<OE14MagicEnergyContainerComponent>? energyEnt)
    {
        energyEnt = null;

        if (!Resolve(ent, ref ent.Comp, false))
            return false;

        if (!_itemSlots.TryGetSlot(ent, ent.Comp.SlotId, out var slot))
            return false;

        if (slot.Item is null)
            return false;

        if (!TryComp<OE14MagicEnergyContainerComponent>(slot.Item, out var energyComp))
            return false;

        energyEnt = (slot.Item.Value, energyComp);
        return true;
    }
    public bool HasEnergy(Entity<OE14MagicEnergyCrystalSlotComponent?> ent,
        FixedPoint2 energy,
        EntityUid? user = null)
    {
        if (!TryGetEnergyCrystalFromSlot(ent, out var energyEnt))
        {
            if (user is not null)
                _popup.PopupEntity(Loc.GetString("oe14-magic-energy-no-crystal"), ent,user.Value);

            return false;
        }

        if (energyEnt.Value.Comp.Energy >= energy)
            return true;

        if (user is not null)
            _popup.PopupEntity(Loc.GetString("oe14-magic-energy-insufficient"), ent, user.Value);

        return false;
    }

    public bool TryChangeEnergy(Entity<OE14MagicEnergyCrystalSlotComponent?> ent,
        FixedPoint2 energy,
        EntityUid? user = null,
        bool safe = false)
    {
        if (!TryGetEnergyCrystalFromSlot(ent, out var energyEnt))
        {
            if (user is not null)
                _popup.PopupEntity(Loc.GetString("oe14-magic-energy-no-crystal"), ent, user.Value);

            return false;
        }

        _magicEnergy.ChangeEnergy((energyEnt.Value, energyEnt.Value), energy, out _, out _, safe);
        return true;
    }

    private void OnCrystalChanged(Entity<OE14MagicEnergyCrystalSlotComponent> ent, ref OE14SlotCrystalChangedEvent args)
    {
        var realPowered = TryGetEnergyCrystalFromSlot((ent, ent), out var energyComp);
        if (energyComp is not null)
            realPowered = energyComp.Value.Comp.Energy > 0;

        if (ent.Comp.Powered == realPowered)
            return;

        ent.Comp.Powered = realPowered;
        _appearance.SetData(ent, OE14MagicSlotVisuals.Powered, realPowered);

        RaiseLocalEvent(ent, new OE14SlotCrystalPowerChangedEvent(realPowered));
    }

    private void OnEnergyChanged(Entity<OE14MagicEnergyCrystalComponent> crystal, ref OE14MagicEnergyLevelChangeEvent args)
    {
        if (!_container.TryGetContainingContainer((crystal.Owner, null, null), out var container))
            return;

        if (!TryComp(container.Owner, out OE14MagicEnergyCrystalSlotComponent? slot))
            return;

        if (!_itemSlots.TryGetSlot(container.Owner, slot.SlotId, out var itemSlot))
            return;

        if (itemSlot.Item != crystal)
            return;

        RaiseLocalEvent(container.Owner, new OE14SlotCrystalChangedEvent(false));
    }

    private void OnExamined(Entity<OE14MagicEnergyCrystalSlotComponent> ent, ref ExaminedEvent args)
    {
        if (!TryGetEnergyCrystalFromSlot((ent, ent), out var energyEnt))
            return;

        if (!args.IsInDetailsRange)
            return;

        args.PushMarkup(_magicEnergy.GetEnergyExaminedText((energyEnt.Value, energyEnt)));
    }
}
