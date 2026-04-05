using Content.Server.Stack;
using Content.Shared._OE14.Temperature;
using Content.Shared.Atmos.Components;
using Content.Shared.Stacks;
using Robust.Server.Audio;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Shared.Timing;

namespace Content.Server._OE14.Temperature.Fireplace;

public sealed partial class OE14FireplaceSystem : EntitySystem
{
    [Dependency] private readonly AppearanceSystem _appearance = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly ContainerSystem _containerSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly StackSystem _stackSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14FireplaceComponent, OnFireChangedEvent>(OnFireChanged);
    }

    private void OnFireChanged(Entity<OE14FireplaceComponent> fireplace, ref OnFireChangedEvent args)
    {
        if (!TryComp<FlammableComponent>(fireplace, out var flammable))
            return;

        if (args.OnFire)
            flammable.FirestackFade = 0;
    }

    private bool TryFoundFuelInStorage(Entity<OE14FireplaceComponent> fireplace, out Entity<FlammableComponent>? fuel)
    {
        fuel = null;

        if (!_containerSystem.TryGetContainer(fireplace, fireplace.Comp.ContainerId, out var container))
            return false;

        if (container.ContainedEntities.Count == 0)
            return false;

        foreach (var ent in container.ContainedEntities)
        {
            if (!TryComp<FlammableComponent>(ent, out var flammable))
                continue;

            fuel = new Entity<FlammableComponent>(ent, flammable);
            return true;
        }

        return false;
    }

    private void ConsumeFuel(EntityUid uid, OE14FireplaceComponent component, Entity<FlammableComponent> fuel)
    {
        if (!TryComp<FlammableComponent>(uid, out var flammable))
            return;

        component.Fuel += fuel.Comp.OE14FireplaceFuel;

        if (flammable.OnFire)
            _audio.PlayPvs(component.InsertFuelSound, uid);

        if (TryComp<StackComponent>(fuel, out var stack))
        {
            _stackSystem.SetCount(fuel, stack.Count - 1);
        }
        else
        {
            QueueDel(fuel);
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = AllEntityQuery<OE14FireplaceComponent, FlammableComponent>();
        while (query.MoveNext(out var uid, out var fireplace, out var flammable))
        {
            if (!flammable.OnFire)
                continue;

            if (_timing.CurTime <= fireplace.NextUpdateTime)
                continue;

            fireplace.NextUpdateTime = _timing.CurTime + fireplace.UpdateFrequency;

            if (fireplace.Fuel >= fireplace.FuelDrainingPerUpdate)
            {
                fireplace.Fuel -= fireplace.FuelDrainingPerUpdate;
                UpdateAppearance(uid, fireplace);
                flammable.FirestackFade = fireplace.FireFadeDelta;
            }
            else
            {
                if (TryFoundFuelInStorage(new Entity<OE14FireplaceComponent>(uid, fireplace), out var fuel) && fuel != null)
                    ConsumeFuel(uid, fireplace, fuel.Value);

                flammable.FirestackFade = -fireplace.FireFadeDelta;

                if (flammable.FireStacks == 0 && fireplace.DeleteOnEmpty)
                {
                    QueueDel(uid);
                    return;
                }
            }
        }
    }

    public void UpdateAppearance(EntityUid uid, OE14FireplaceComponent? fireplace = null, AppearanceComponent? appearance = null)
    {
        if (!Resolve(uid, ref fireplace, ref appearance))
            return;

        if (fireplace.Fuel < fireplace.FuelDrainingPerUpdate)
        {
            _appearance.SetData(uid, FireplaceFuelVisuals.Status, FireplaceFuelStatus.Empty, appearance);
            return;
        }

        if (fireplace.Fuel < fireplace.MaxFuelLimit / 2)
            _appearance.SetData(uid, FireplaceFuelVisuals.Status, FireplaceFuelStatus.Medium, appearance);
        else
            _appearance.SetData(uid, FireplaceFuelVisuals.Status, FireplaceFuelStatus.Full, appearance);
    }
}
