using System.Linq;
using System.Text;
using Content.Shared._OE14.LockKey.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Timing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared._OE14.LockKey;

public sealed class SharedOE14LockKeySystem : EntitySystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly LockSystem _lock = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly UseDelaySystem _useDelay = default!;

    private EntityQuery<LockComponent> _lockQuery;
    private EntityQuery<OE14LockComponent> _oe14LockQuery;
    private EntityQuery<OE14KeyComponent> _keyQuery;
    private EntityQuery<DoorComponent> _doorQuery;

    public const int DepthComplexity = 2;

    public override void Initialize()
    {
        base.Initialize();

        _lockQuery = GetEntityQuery<LockComponent>();
        _oe14LockQuery = GetEntityQuery<OE14LockComponent>();
        _keyQuery = GetEntityQuery<OE14KeyComponent>();
        _doorQuery = GetEntityQuery<DoorComponent>();

        //Interact
        SubscribeLocalEvent<OE14KeyComponent, AfterInteractEvent>(OnKeyInteract);
        SubscribeLocalEvent<OE14KeyRingComponent, AfterInteractEvent>(OnKeyRingInteract);
        SubscribeLocalEvent<OE14LockComponent, AfterInteractEvent>(OnLockInteract);

        //Verbs
        SubscribeLocalEvent<OE14KeyComponent, GetVerbsEvent<UtilityVerb>>(GetKeysVerbs);
        SubscribeLocalEvent<OE14KeyFileComponent, GetVerbsEvent<UtilityVerb>>(GetKeyFileVerbs);
        SubscribeLocalEvent<OE14LockpickComponent, GetVerbsEvent<UtilityVerb>>(GetLockpickVerbs);
        SubscribeLocalEvent<OE14LockEditerComponent, GetVerbsEvent<UtilityVerb>>(GetLockEditerVerbs);

        SubscribeLocalEvent<OE14LockComponent, LockPickHackDoAfterEvent>(OnLockHacked);
        SubscribeLocalEvent<OE14LockComponent, LockInsertDoAfterEvent>(OnLockInserted);

        SubscribeLocalEvent<OE14KeyComponent, ExaminedEvent>(OnKeyExamine);
        SubscribeLocalEvent<OE14LockComponent, ExaminedEvent>(OnLockExamine);
    }

    private void OnKeyRingInteract(Entity<OE14KeyRingComponent> keyring, ref AfterInteractEvent args)
    {
        if (args.Handled)
            return;

        if (!args.CanReach || args.Target is not { Valid: true })
            return;

        if (!TryComp<StorageComponent>(keyring, out var storageComp))
            return;

        if (!_lockQuery.TryComp(args.Target, out _))
            return;

        if (!_oe14LockQuery.TryComp(args.Target, out var oe14LockComp))
            return;

        if (oe14LockComp.LockShape == null)
            return;

        if (!_timing.IsFirstTimePredicted)
            return;

        args.Handled = true;

        foreach (var (key, _) in storageComp.StoredItems)
        {
            if (!_keyQuery.TryComp(key, out var keyComp))
                continue;

            if (keyComp.LockShape == null)
                continue;

            if (!keyComp.LockShape.SequenceEqual(oe14LockComp.LockShape))
                continue;

            TryUseKeyOnLock(args.User,
                new Entity<OE14LockComponent>(args.Target.Value, oe14LockComp),
                new Entity<OE14KeyComponent>(key, keyComp));
            args.Handled = true;
            return;
        }

        if (_timing.IsFirstTimePredicted)
            _popup.PopupPredicted(Loc.GetString("oe14-lock-key-no-fit"), args.Target.Value, args.User);
    }

    private void OnKeyInteract(Entity<OE14KeyComponent> key, ref AfterInteractEvent args)
    {
        if (args.Handled)
            return;

        if (!args.CanReach || args.Target is not { Valid: true })
            return;

        if (!_lockQuery.TryComp(args.Target, out _))
            return;

        if (!_oe14LockQuery.TryComp(args.Target, out var oe14LockComp))
            return;

        if (!_timing.IsFirstTimePredicted)
            return;

        args.Handled = true;

        TryUseKeyOnLock(args.User, new Entity<OE14LockComponent>(args.Target.Value, oe14LockComp), key);
        args.Handled = true;
    }

    private void OnLockInteract(Entity<OE14LockComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Handled)
            return;

        if (!_timing.IsFirstTimePredicted)
            return;

        if (!ent.Comp.CanEmbedded)
            return;

        if (!_lockQuery.TryComp(args.Target, out _))
            return;

        if (!_oe14LockQuery.TryComp(args.Target, out var targetCp14LockComp))
            return;

        args.Handled = true;

        if (targetCp14LockComp.LockShape is not null)
        {
            _popup.PopupPredicted(Loc.GetString("oe14-lock-insert-fail-have-lock",
                    ("name", MetaData(args.Target.Value).EntityName)),
                ent,
                args.User);
            return;
        }

        //Ok, all checks passed, we ready to install lock into entity

        args.Handled = true;

        _popup.PopupPredicted(Loc.GetString("oe14-lock-insert-start", ("name", MetaData(args.Target.Value).EntityName), ("player", Identity.Name(args.User, EntityManager))),
            ent,
            args.User);

        _doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager,
            args.User,
            TimeSpan.FromSeconds(2f), //Boo, hardcoding
            new LockInsertDoAfterEvent(),
            args.Target,
            args.Target,
            ent)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            BreakOnDropItem = true,
            BreakOnHandChange = true,
        });
    }

    private void OnLockInserted(Entity<OE14LockComponent> ent, ref LockInsertDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        if (!_oe14LockQuery.TryComp(args.Used, out var usedLock))
            return;

        ent.Comp.LockShape = usedLock.LockShape;
        DirtyField(ent, ent.Comp, nameof(OE14LockComponent.LockShape));

        _popup.PopupPredicted(Loc.GetString("oe14-lock-insert-success", ("name", MetaData(ent).EntityName)),
            ent,
            args.User);

        _audio.PlayPredicted(usedLock.EmbedSound, ent, args.User);

        if (_net.IsServer)
            QueueDel(args.Used);
    }

    private void OnLockHacked(Entity<OE14LockComponent> ent, ref LockPickHackDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        if (ent.Comp.LockShape == null)
            return;

        if (!_lockQuery.TryComp(ent, out var lockComp))
            return;

        if (!TryComp<OE14LockpickComponent>(args.Used, out var lockPick))
            return;

        if (!_timing.IsFirstTimePredicted)
            return;

        if (args.Height == ent.Comp.LockShape[ent.Comp.LockPickStatus]) //Success
        {
            _audio.PlayPredicted(lockPick.SuccessSound, ent, args.User);
            ent.Comp.LockPickStatus++;
            DirtyField(ent, ent.Comp, nameof(OE14LockComponent.LockPickStatus));
            if (ent.Comp.LockPickStatus >= ent.Comp.LockShape.Count) // Final success
            {
                if (lockComp.Locked)
                {
                    _lock.TryUnlock(ent, args.User, lockComp);
                    _popup.PopupPredicted(Loc.GetString("oe14-lock-unlock", ("lock", MetaData(ent).EntityName)),
                        ent,
                        args.User);

                    ent.Comp.LockPickStatus = 0;
                    DirtyField(ent, ent.Comp, nameof(OE14LockComponent.LockPickStatus));
                    return;
                }

                _lock.TryLock(ent, args.User, lockComp);

                _popup.PopupPredicted(Loc.GetString("oe14-lock-lock", ("lock", MetaData(ent).EntityName)),
                    ent,
                    args.User);
                ent.Comp.LockPickStatus = 0;

                DirtyField(ent, ent.Comp, nameof(OE14LockComponent.LockPickStatus));
                return;
            }

            _popup.PopupClient(Loc.GetString("oe14-lock-lock-pick-success") +
                               $" ({ent.Comp.LockPickStatus}/{ent.Comp.LockShape.Count})",
                ent,
                args.User);
        }
        else //Fail
        {
            _audio.PlayPredicted(lockPick.FailSound, ent, args.User);
            if (_net.IsServer)
            {
                lockPick.Health--;
                if (lockPick.Health > 0)
                {
                    _popup.PopupEntity(Loc.GetString("oe14-lock-lock-pick-failed", ("lock", MetaData(ent).EntityName)),
                        ent,
                        args.User);
                }
                else
                {
                    _popup.PopupEntity(
                        Loc.GetString("oe14-lock-lock-pick-failed-break", ("lock", MetaData(ent).EntityName)),
                        ent,
                        args.User);
                    QueueDel(args.Used);
                }
            }

            ent.Comp.LockPickStatus = 0;
            DirtyField(ent, ent.Comp, nameof(OE14LockComponent.LockPickStatus));
        }
    }

    private void GetKeysVerbs(Entity<OE14KeyComponent> key, ref GetVerbsEvent<UtilityVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        if (!_lockQuery.TryComp(args.Target, out var lockComp))
            return;

        if (!_oe14LockQuery.TryComp(args.Target, out var oe14LockComponent))
            return;

        var target = args.Target;
        var user = args.User;

        var verb = new UtilityVerb
        {
            Act = () =>
            {
                TryUseKeyOnLock(user, new Entity<OE14LockComponent>(target, oe14LockComponent), key);
            },
            IconEntity = GetNetEntity(key),
            Text = Loc.GetString(
                lockComp.Locked ? "oe14-lock-verb-use-key-text-open" : "oe14-lock-verb-use-key-text-close",
                ("item", MetaData(args.Target).EntityName)),
            Message = Loc.GetString("oe14-lock-verb-use-key-message", ("item", MetaData(args.Target).EntityName)),
        };

        args.Verbs.Add(verb);
    }

    private void GetKeyFileVerbs(Entity<OE14KeyFileComponent> ent, ref GetVerbsEvent<UtilityVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        if (!_keyQuery.TryComp(args.Target, out var keyComp))
            return;

        if (keyComp.LockShape == null)
            return;

        var target = args.Target;
        var user = args.User;

        var lockShapeCount = keyComp.LockShape.Count;
        for (var i = 0; i <= lockShapeCount - 1; i++)
        {
            var i1 = i;
            var verb = new UtilityVerb
            {
                Act = () =>
                {
                    if (keyComp.LockShape[i1] <= -DepthComplexity)
                        return;

                    if (!_timing.IsFirstTimePredicted)
                        return;

                    if (TryComp<UseDelayComponent>(ent, out var useDelayComp) &&
                        _useDelay.IsDelayed((ent, useDelayComp)))
                        return;

                    if (!_net.IsServer)
                        return;

                    keyComp.LockShape[i1]--;
                    DirtyField(target, keyComp, nameof(OE14KeyComponent.LockShape));
                    _audio.PlayPvs(ent.Comp.UseSound, Transform(target).Coordinates);
                    Spawn("EffectSparks", Transform(target).Coordinates);
                    var shapeString = "[" + string.Join(", ", keyComp.LockShape) + "]";
                    _popup.PopupEntity(Loc.GetString("oe14-lock-key-file-updated") + shapeString, target, user);
                    _useDelay.TryResetDelay(ent);
                },
                IconEntity = GetNetEntity(ent),
                Category = VerbCategory.OE14KeyFile,
                Priority = -i,
                Disabled = keyComp.LockShape[i] <= -DepthComplexity,
                Text = Loc.GetString("oe14-lock-key-file-use-hint", ("num", i)),
                CloseMenu = false,
            };
            args.Verbs.Add(verb);
        }
    }

    private void GetLockpickVerbs(Entity<OE14LockpickComponent> lockPick, ref GetVerbsEvent<UtilityVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        if (!_lockQuery.TryComp(args.Target, out var lockComp) || !lockComp.Locked)
            return;

        if (!_oe14LockQuery.HasComp(args.Target))
            return;

        var target = args.Target;
        var user = args.User;

        for (var i = DepthComplexity; i >= -DepthComplexity; i--)
        {
            var height = i;
            var verb = new UtilityVerb()
            {
                Act = () =>
                {
                    _doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager,
                        user,
                        lockPick.Comp.HackTime,
                        new LockPickHackDoAfterEvent(height),
                        target,
                        target,
                        lockPick)
                    {
                        BreakOnDamage = true,
                        BreakOnMove = true,
                        BreakOnDropItem = true,
                        BreakOnHandChange = true,
                    });
                },
                Text = Loc.GetString("oe14-lock-verb-lock-pick-use-text") + $" {height}",
                Message = Loc.GetString("oe14-lock-verb-lock-pick-use-message"),
                Category = VerbCategory.OE14LockPick,
                Priority = height,
                CloseMenu = false,
            };

            args.Verbs.Add(verb);
        }
    }

    private void GetLockEditerVerbs(Entity<OE14LockEditerComponent> ent, ref GetVerbsEvent<UtilityVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        if (!_oe14LockQuery.TryComp(args.Target, out var lockComp) || !lockComp.CanEmbedded)
            return;

        if (lockComp.LockShape is null)
            return;

        var target = args.Target;
        var user = args.User;

        var lockShapeCount = lockComp.LockShape.Count;
        for (var i = 0; i <= lockShapeCount - 1; i++)
        {
            var i1 = i;
            var verb = new UtilityVerb
            {
                Act = () =>
                {
                    if (!_timing.IsFirstTimePredicted)
                        return;

                    if (TryComp<UseDelayComponent>(ent, out var useDelayComp) &&
                        _useDelay.IsDelayed((ent, useDelayComp)))
                        return;

                    if (!_net.IsServer)
                        return;

                    lockComp.LockShape[i1]--;
                    if (lockComp.LockShape[i1] < -DepthComplexity)
                        lockComp.LockShape[i1] = DepthComplexity; //Cycle back to max

                    DirtyField(target, lockComp, nameof(OE14LockComponent.LockShape));
                    _audio.PlayPvs(ent.Comp.UseSound, Transform(target).Coordinates);
                    var shapeString = "[" + string.Join(", ", lockComp.LockShape) + "]";
                    _popup.PopupEntity(Loc.GetString("oe14-lock-editor-updated") + shapeString, target, user);
                    _useDelay.TryResetDelay(ent);
                },
                IconEntity = GetNetEntity(ent),
                Category = VerbCategory.OE14EditLock,
                Priority = -i,
                Text = Loc.GetString("oe14-lock-editor-use-hint", ("num", i)),
                CloseMenu = false,
            };
            args.Verbs.Add(verb);
        }
    }

    private void TryUseKeyOnLock(EntityUid user, Entity<OE14LockComponent> target, Entity<OE14KeyComponent> key)
    {
        if (!TryComp<LockComponent>(target, out var lockComp))
            return;

        if (_doorQuery.TryComp(target, out var doorComponent) && doorComponent.State == DoorState.Open)
            return;

        var keyShape = key.Comp.LockShape;
        var lockShape = target.Comp.LockShape;

        if (keyShape == null || lockShape == null)
            return;

        var isEqual = keyShape.SequenceEqual(lockShape);

        if (HasComp<OE14KeyUniversalComponent>(key) && !isEqual)
        {
            // Make new shape for key and force equality for this use
            _popup.PopupClient(Loc.GetString("oe14-lock-key-transforming"), key, user);
            key.Comp.LockShape = new List<int>(lockShape);
            DirtyField(key, key.Comp, nameof(OE14KeyComponent.LockShape));
            isEqual = true;
        }

        if (isEqual)
        {
            if (lockComp.Locked)
            {
                _lock.TryUnlock(target, user);
            }
            else
            {
                _lock.TryLock(target, user);
            }
        }
        else
        {
            _popup.PopupClient(Loc.GetString("oe14-lock-key-no-fit"), target, user);
        }
    }

    private void OnKeyExamine(Entity<OE14KeyComponent> ent, ref ExaminedEvent args)
    {
        var parent = Transform(ent).ParentUid;
        if (parent != args.Examiner)
            return;

        if (ent.Comp.LockShape == null)
            return;

        var sb = new StringBuilder(Loc.GetString("oe14-lock-examine-key", ("item", MetaData(ent).EntityName)));
        sb.Append(" (");
        foreach (var item in ent.Comp.LockShape)
        {
            sb.Append($"{item} ");
        }

        sb.Append(")");
        args.PushMarkup(sb.ToString());
    }

    private void OnLockExamine(Entity<OE14LockComponent> ent, ref ExaminedEvent args)
    {
        if (!ent.Comp.CanEmbedded)
            return;

        var parent = Transform(ent).ParentUid;
        if (parent != args.Examiner)
            return;

        if (ent.Comp.LockShape == null)
            return;

        var sb = new StringBuilder(Loc.GetString("oe14-lock-examine-key", ("item", MetaData(ent).EntityName)));
        sb.Append(" (");
        foreach (var item in ent.Comp.LockShape)
        {
            sb.Append($"{item} ");
        }

        sb.Append(")");
        args.PushMarkup(sb.ToString());
    }
}

[Serializable, NetSerializable]
public sealed partial class LockPickHackDoAfterEvent : DoAfterEvent
{
    [DataField]
    public int Height = 0;

    public LockPickHackDoAfterEvent(int h)
    {
        Height = h;
    }

    public override DoAfterEvent Clone() => this;
}

[Serializable, NetSerializable]
public sealed partial class LockInsertDoAfterEvent : SimpleDoAfterEvent;
