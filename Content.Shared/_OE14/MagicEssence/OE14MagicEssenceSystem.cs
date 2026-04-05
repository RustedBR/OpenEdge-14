using System.Text;
using Content.Shared._OE14.MagicEnergy;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Stacks;
using Content.Shared.Throwing;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._OE14.MagicEssence;

public partial class OE14MagicEssenceSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly OE14SharedMagicEnergySystem _magicEnergy = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14MagicEssenceContainerComponent, ExaminedEvent>(OnExamine);

        SubscribeLocalEvent<OE14MagicEssenceScannerComponent, OE14MagicEssenceScanEvent>(OnMagicScanAttempt);
        SubscribeLocalEvent<OE14MagicEssenceScannerComponent, InventoryRelayedEvent<OE14MagicEssenceScanEvent>>((e, c, ev) => OnMagicScanAttempt(e, c, ev.Args));

        SubscribeLocalEvent<OE14MagicEssenceSplitterComponent, OE14MagicEnergyOverloadEvent>(OnEnergyOverload);

        SubscribeLocalEvent<OE14MagicEssenceCollectorComponent, OE14SlotCrystalPowerChangedEvent>(OnPowerChanged);
        SubscribeLocalEvent<OE14MagicEssenceCollectorComponent, StartCollideEvent>(OnCollectorCollide);
    }

    private void OnPowerChanged(Entity<OE14MagicEssenceCollectorComponent> ent, ref OE14SlotCrystalPowerChangedEvent args)
    {
        if (args.Powered)
        {
            EnsureComp<OE14MagicEssenceAttractorComponent>(ent);
            return;
        }

        RemCompDeferred<OE14MagicEssenceAttractorComponent>(ent);
    }

    private void OnCollectorCollide(Entity<OE14MagicEssenceCollectorComponent> ent, ref StartCollideEvent args)
    {
        if (TryComp<OE14MagicEnergyCrystalSlotComponent>(ent, out var energySlot) && !energySlot.Powered)
            return;

        if (!TryComp<OE14MagicEssenceComponent>(args.OtherEntity, out var essenceComp))
            return;

        if (!TryComp<SolutionContainerManagerComponent>(args.OtherEntity, out var essenceSolutionManager))
            return;

        if (!TryComp<SolutionContainerManagerComponent>(ent, out var collectorSolutionManager))
            return;

        if (!_solution.TryGetSolution((args.OtherEntity, essenceSolutionManager), essenceComp.Solution, out var essenceSoln, out var essenceSolution))
            return;

        if (!_solution.TryGetSolution((ent, collectorSolutionManager), ent.Comp.Solution, out var collectorSoln, out var collectorSolution))
            return;

        if (!_solution.TryTransferSolution(collectorSoln.Value, essenceSolution, essenceSolution.Volume))
            return;

        _audio.PlayPvs(essenceComp.ConsumeSound, ent);

        if (_net.IsServer)
            QueueDel(args.OtherEntity);
    }

    private void OnEnergyOverload(Entity<OE14MagicEssenceSplitterComponent> ent, ref OE14MagicEnergyOverloadEvent args)
    {
        if (!TryComp<OE14MagicEnergyContainerComponent>(ent, out var energyContainer))
            return;

        _magicEnergy.ChangeEnergy((ent, energyContainer), -energyContainer.Energy, out _, out _, safe: true);

        //TODO move to server
        if (_net.IsClient)
            return;

        var entities = _lookup.GetEntitiesInRange(ent, 0.5f, LookupFlags.Uncontained);
        foreach (var entUid in entities)
        {
            var splitting = !(ent.Comp.Whitelist is not null && !_whitelist.IsValid(ent.Comp.Whitelist, entUid));
            if (splitting)
                TrySplitToEssence(entUid);

            //Vector from splitter to item
            var dir = (Transform(entUid).Coordinates.Position - Transform(ent).Coordinates.Position).Normalized() * ent.Comp.ThrowForce;
            _throwing.TryThrow(entUid, dir, ent.Comp.ThrowForce);
        }
        SpawnAttachedTo(ent.Comp.ImpactEffect, Transform(ent).Coordinates);
    }

    private void OnMagicScanAttempt(EntityUid uid, OE14MagicEssenceScannerComponent component, OE14MagicEssenceScanEvent args)
    {
        args.CanScan = true;
    }

    private bool TrySplitToEssence(EntityUid uid)
    {
        if (!TryComp<OE14MagicEssenceContainerComponent>(uid, out var essenceContainer))
            return false;

   	    var count = 1;

        if (TryComp<StackComponent>(uid, out var stack))
        {
            count = stack.Count;
        }

        foreach (var essence in essenceContainer.Essences)
        {
            if (_proto.TryIndex(essence.Key, out var magicType))
            {
                for (var i = 0; i < essence.Value; i++)
                {
                    for (var j = 0; j < count; j++)
                    {
                        var spawned = SpawnAtPosition(magicType.EssenceProto, Transform(uid).Coordinates);
                        _transform.AttachToGridOrMap(spawned);
                    }
                }
            }
        }

        QueueDel(uid);
        return true;
    }

    private void OnExamine(Entity<OE14MagicEssenceContainerComponent> ent, ref ExaminedEvent args)
    {
        var scanEvent = new OE14MagicEssenceScanEvent();
        RaiseLocalEvent(args.Examiner, scanEvent);

        if (!scanEvent.CanScan)
            return;

        var count = 1;

        if (TryComp<StackComponent>(ent, out var stack))
        {
            count = stack.Count;
        }


        var sb = new StringBuilder();
        sb.Append(Loc.GetString("oe14-magic-essence-title") + "\n");
        foreach (var essence in ent.Comp.Essences)
        {
            if (_proto.TryIndex(essence.Key, out var magicType))
            {
                sb.Append($"[color={magicType.Color.ToHex()}]{Loc.GetString(magicType.Name)}[/color]: x{essence.Value * count}\n");
            }
        }

        args.PushMarkup(sb.ToString());
    }
}

public sealed class OE14MagicEssenceScanEvent : EntityEventArgs, IInventoryRelayEvent
{
    public bool CanScan;
    public SlotFlags TargetSlots { get; } = SlotFlags.EYES;
}
