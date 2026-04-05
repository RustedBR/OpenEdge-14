using Content.Server._OE14.MagicSpellStorage.Components;
using Content.Shared.Clothing;
using Content.Shared.Hands;

namespace Content.Server._OE14.MagicSpellStorage;

public sealed partial class OE14SpellStorageSystem
{
    private void InitializeAccess()
    {
        SubscribeLocalEvent<OE14SpellStorageAccessHoldingComponent, GotEquippedHandEvent>(OnEquippedHand);

        SubscribeLocalEvent<OE14SpellStorageAccessWearingComponent, ClothingGotEquippedEvent>(OnClothingEquipped);
        SubscribeLocalEvent<OE14SpellStorageAccessWearingComponent, ClothingGotUnequippedEvent>(OnClothingUnequipped);
    }

    private void OnEquippedHand(Entity<OE14SpellStorageAccessHoldingComponent> ent, ref GotEquippedHandEvent args)
    {
        if (!TryComp<OE14SpellStorageComponent>(ent, out var spellStorage))
            return;

        TryGrantAccess((ent, spellStorage), args.User);
    }

    private void OnClothingEquipped(Entity<OE14SpellStorageAccessWearingComponent> ent, ref ClothingGotEquippedEvent args)
    {
        ent.Comp.Wearing = true;

        if (!TryComp<OE14SpellStorageComponent>(ent, out var spellStorage))
            return;

        TryGrantAccess((ent, spellStorage), args.Wearer);
    }

    private void OnClothingUnequipped(Entity<OE14SpellStorageAccessWearingComponent> ent, ref ClothingGotUnequippedEvent args)
    {
        ent.Comp.Wearing = false;

        _actions.RemoveProvidedActions(args.Wearer, ent);
    }
}
