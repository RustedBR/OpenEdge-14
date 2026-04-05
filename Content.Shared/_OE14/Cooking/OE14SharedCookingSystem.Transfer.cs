/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Shared._OE14.Cooking.Components;
using Content.Shared.Interaction;
using Robust.Shared.Containers;

namespace Content.Shared._OE14.Cooking;

public abstract partial class OE14SharedCookingSystem
{
    private void InitTransfer()
    {
        SubscribeLocalEvent<OE14FoodHolderComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<OE14FoodHolderComponent, InteractUsingEvent>(OnInteractUsing);

        SubscribeLocalEvent<OE14FoodCookerComponent, ContainerIsInsertingAttemptEvent>(OnInsertAttempt);
    }

    private void OnInteractUsing(Entity<OE14FoodHolderComponent> target, ref InteractUsingEvent args)
    {
        if (!TryComp<OE14FoodHolderComponent>(args.Used, out var used))
            return;

        TryTransferFood(target, (args.Used, used));
    }

    private void OnAfterInteract(Entity<OE14FoodHolderComponent> ent, ref AfterInteractEvent args)
    {
        if (!TryComp<OE14FoodHolderComponent>(args.Target, out var target))
            return;

        TryTransferFood(ent, (args.Target.Value, target));
    }

    private void OnInsertAttempt(Entity<OE14FoodCookerComponent> ent, ref ContainerIsInsertingAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        if (!TryComp<OE14FoodHolderComponent>(ent, out var holder))
            return;

        if (holder.FoodData is not null)
        {
            _popup.PopupEntity(Loc.GetString("oe14-cooking-popup-not-empty", ("name", MetaData(ent).EntityName)), ent);
            args.Cancel();
        }
    }
}
