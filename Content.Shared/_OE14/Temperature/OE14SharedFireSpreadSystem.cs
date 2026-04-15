using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared._OE14.Temperature;

public abstract partial class OE14SharedFireSpreadSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14FireSpreadComponent, OnFireChangedEvent>(OnFireChangedSpread);
        SubscribeLocalEvent<OE14DespawnOnExtinguishComponent, OnFireChangedEvent>(OnFireChangedDespawn);
        SubscribeLocalEvent<OE14DelayedIgnitionSourceComponent, OnFireChangedEvent>(OnIgnitionSourceFireChanged);
        SubscribeLocalEvent<OE14DelayedIgnitionSourceComponent, AfterInteractEvent>(OnDelayedIgniteAttempt);
        // Handle ignite on the TARGET side with priority before Storage, so the ignite check
        // wins over the storage "insert item" interaction when using a lighter on a furnace/bonfire.
        SubscribeLocalEvent<OE14IgnitionModifierComponent, InteractUsingEvent>(OnIgnitionTargetInteractUsing,
            before: new[] { typeof(SharedStorageSystem) });
    }

    private void OnFireChangedDespawn(Entity<OE14DespawnOnExtinguishComponent> ent, ref OnFireChangedEvent args)
    {
        if (!args.OnFire)
            QueueDel(ent);
    }

    private void OnFireChangedSpread(Entity<OE14FireSpreadComponent> ent, ref OnFireChangedEvent args)
    {
        if (args.OnFire)
        {
            EnsureComp<OE14ActiveFireSpreadingComponent>(ent);
        }
        else
        {
            if (HasComp<OE14ActiveFireSpreadingComponent>(ent))
                RemCompDeferred<OE14ActiveFireSpreadingComponent>(ent);
        }

        ent.Comp.NextSpreadTime = _gameTiming.CurTime + TimeSpan.FromSeconds(ent.Comp.SpreadCooldownMax);
    }

    private void OnDelayedIgniteAttempt(Entity<OE14DelayedIgnitionSourceComponent> ent, ref AfterInteractEvent args)
    {
        if (!args.CanReach || args.Handled || args.Target == null)
            return;

        if (!ent.Comp.Enabled)
            return;

        var time = ent.Comp.Delay;
        var caution = true;

        if (TryComp<OE14IgnitionModifierComponent>(args.Target, out var modifier))
        {
            time *= modifier.IgnitionTimeModifier;
            caution = !modifier.HideCaution;
        }

        _doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager,
            args.User,
            time,
            new OE14IgnitionDoAfter(),
            args.Target,
            args.Target,
            ent)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            BreakOnDropItem = true,
            BreakOnHandChange = true,
            BlockDuplicate = true,
            CancelDuplicate = true
        });

        var selfMessage = Loc.GetString("oe14-attempt-ignite-caution-self",
            ("target", MetaData(args.Target.Value).EntityName));
        var otherMessage = Loc.GetString("oe14-attempt-ignite-caution",
            ("name", Identity.Entity(args.User, EntityManager)),
            ("target", Identity.Entity(args.Target.Value, EntityManager)));
        _popup.PopupPredicted(selfMessage,
            otherMessage,
            args.User,
            args.User,
            caution ? PopupType.MediumCaution : PopupType.Small);
    }

    private void OnIgnitionSourceFireChanged(Entity<OE14DelayedIgnitionSourceComponent> ent, ref OnFireChangedEvent args)
    {
        ent.Comp.Enabled = args.OnFire;
        Dirty(ent);
    }

    /// <summary>
    ///     Raised on the TARGET (furnace/bonfire) before Storage gets the event.
    ///     If the item being used is an ignition source, trigger ignition and block storage.
    /// </summary>
    private void OnIgnitionTargetInteractUsing(Entity<OE14IgnitionModifierComponent> target, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp<OE14DelayedIgnitionSourceComponent>(args.Used, out var ignitionSource))
            return;

        if (!ignitionSource.Enabled)
            return;

        var time = ignitionSource.Delay;
        var caution = !target.Comp.HideCaution;

        _doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager,
            args.User,
            time,
            new OE14IgnitionDoAfter(),
            target,
            target,
            args.Used)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            BreakOnDropItem = true,
            BreakOnHandChange = true,
            BlockDuplicate = true,
            CancelDuplicate = true,
        });

        var selfMessage = Loc.GetString("oe14-attempt-ignite-caution-self",
            ("target", MetaData(target.Owner).EntityName));
        var otherMessage = Loc.GetString("oe14-attempt-ignite-caution",
            ("name", Identity.Entity(args.User, EntityManager)),
            ("target", Identity.Entity(target.Owner, EntityManager)));
        _popup.PopupPredicted(selfMessage,
            otherMessage,
            args.User,
            args.User,
            caution ? PopupType.MediumCaution : PopupType.Small);

        args.Handled = true;
    }
}

/// <summary>
/// Raised whenever an FlammableComponent OnFire is Changed
/// </summary>
[ByRefEvent]
public readonly record struct OnFireChangedEvent(bool OnFire)
{
    public readonly bool OnFire = OnFire;
}

[Serializable, NetSerializable]
public sealed partial class OE14IgnitionDoAfter : SimpleDoAfterEvent
{
}
