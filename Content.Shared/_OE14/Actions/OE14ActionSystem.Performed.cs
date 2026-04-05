using Content.Shared._OE14.Actions.Components;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared._OE14.MagicSpell.Events;
using Content.Shared.Actions.Events;
using Content.Shared.FixedPoint;

namespace Content.Shared._OE14.Actions;

public abstract partial class OE14SharedActionSystem
{
    private void InitializePerformed()
    {
        SubscribeLocalEvent<OE14ActionMaterialCostComponent, ActionPerformedEvent>(OnMaterialCostActionPerformed);
        SubscribeLocalEvent<OE14ActionStaminaCostComponent, ActionPerformedEvent>(OnStaminaCostActionPerformed);
        SubscribeLocalEvent<OE14ActionManaCostComponent, ActionPerformedEvent>(OnManaCostActionPerformed);
        SubscribeLocalEvent<OE14ActionSkillPointCostComponent, ActionPerformedEvent>(OnSkillPointCostActionPerformed);
    }

    private void OnMaterialCostActionPerformed(Entity<OE14ActionMaterialCostComponent> ent, ref ActionPerformedEvent args)
    {
        if (ent.Comp.Requirement is null)
            return;

        HashSet<EntityUid> heldedItems = new();

        foreach (var hand in _hand.EnumerateHands(args.Performer))
        {
            var helded = _hand.GetHeldItem(args.Performer, hand);
            if (helded is not null)
                heldedItems.Add(helded.Value);
        }

        ent.Comp.Requirement.PostCraft(EntityManager, _proto, heldedItems);
    }

    private void OnStaminaCostActionPerformed(Entity<OE14ActionStaminaCostComponent> ent, ref ActionPerformedEvent args)
    {
        _stamina.TakeStaminaDamage(args.Performer, ent.Comp.Stamina, visual: false);
    }

    private void OnManaCostActionPerformed(Entity<OE14ActionManaCostComponent> ent, ref ActionPerformedEvent args)
    {
        if (!_actionQuery.TryComp(ent, out var action))
            return;

        if (action.Container is null)
            return;

        var innate = action.Container == args.Performer;

        var manaCost = ent.Comp.ManaCost;

        if (ent.Comp.CanModifyManacost)
        {
            var manaEv = new OE14CalculateManacostEvent(args.Performer, ent.Comp.ManaCost);

            RaiseLocalEvent(args.Performer, manaEv);

            if (!innate)
                RaiseLocalEvent(action.Container.Value, manaEv);

            manaCost = manaEv.GetManacost();
        }

        //First - try to take mana from container
        if (!innate && TryComp<OE14MagicEnergyContainerComponent>(action.Container, out var magicStorage))
        {
            var spellEv = new OE14SpellFromSpellStorageUsedEvent(args.Performer, ent, manaCost);
            RaiseLocalEvent(action.Container.Value, ref spellEv);

            _magicEnergy.ChangeEnergy((action.Container.Value, magicStorage), -manaCost, out var changedEnergy, out var overloadedEnergy, safe: false);
            manaCost -= FixedPoint2.Abs(changedEnergy + overloadedEnergy);
        }

        //Second - action user
        if (manaCost > 0 &&
            TryComp<OE14MagicEnergyContainerComponent>(args.Performer, out var playerMana))
            _magicEnergy.ChangeEnergy((args.Performer, playerMana), -manaCost, out _, out _, safe: false);

        //And spawn mana trace
        _magicVision.SpawnMagicTrace(
                Transform(args.Performer).Coordinates,
                action.Icon,
                Loc.GetString("oe14-magic-vision-used-spell", ("name", MetaData(ent).EntityName)),
                TimeSpan.FromSeconds((float)ent.Comp.ManaCost * 50),
                args.Performer,
                null); //TODO: We need a way to pass spell target here
    }

    private void OnSkillPointCostActionPerformed(Entity<OE14ActionSkillPointCostComponent> ent, ref ActionPerformedEvent args)
    {
        if (ent.Comp.SkillPoint is null)
            return;

        _skill.RemoveSkillPoints(args.Performer, ent.Comp.SkillPoint.Value,  ent.Comp.Count);
    }
}
