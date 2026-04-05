using Content.Shared._OE14.Actions.Components;
using Content.Shared.Actions.Events;
using Content.Shared.Movement.Systems;

namespace Content.Shared._OE14.Actions;

public abstract partial class OE14SharedActionSystem
{
    private void InitializeDoAfter()
    {
        SubscribeLocalEvent<OE14ActionDoAfterSlowdownComponent, OE14ActionStartDoAfterEvent>(OnStartDoAfter);
        SubscribeLocalEvent<OE14ActionDoAfterSlowdownComponent, ActionDoAfterEvent>(OnEndDoAfter);
        SubscribeLocalEvent<OE14SlowdownFromActionsComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovespeed);
    }

    private void OnStartDoAfter(Entity<OE14ActionDoAfterSlowdownComponent> ent, ref OE14ActionStartDoAfterEvent args)
    {
        var performer = GetEntity(args.Performer);
        EnsureComp<OE14SlowdownFromActionsComponent>(performer, out var slowdown);

        slowdown.SpeedAffectors.TryAdd(GetNetEntity(ent), ent.Comp.SpeedMultiplier);
        Dirty(performer, slowdown);
        _movement.RefreshMovementSpeedModifiers(performer);
    }

    private void OnEndDoAfter(Entity<OE14ActionDoAfterSlowdownComponent> ent, ref ActionDoAfterEvent args)
    {
        if (args.Repeat)
            return;

        var performer = GetEntity(args.Performer);
        if (!TryComp<OE14SlowdownFromActionsComponent>(performer, out var slowdown))
            return;

        slowdown.SpeedAffectors.Remove(GetNetEntity(ent));
        Dirty(performer, slowdown);

        _movement.RefreshMovementSpeedModifiers(performer);

        if (slowdown.SpeedAffectors.Count == 0)
            RemCompDeferred<OE14SlowdownFromActionsComponent>(performer);
    }

    private void OnRefreshMovespeed(Entity<OE14SlowdownFromActionsComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
    {
        var targetSpeedModifier = 1f;

        foreach (var (_, affector) in ent.Comp.SpeedAffectors)
        {
            targetSpeedModifier *= affector;
        }

        args.ModifySpeed(targetSpeedModifier);
    }
}
