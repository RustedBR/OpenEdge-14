using Content.Server._OE14.MagicEnergy.Components;
using Content.Shared._OE14.DayCycle;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared.Damage;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Timing;

namespace Content.Server._OE14.MagicEnergy;

public partial class OE14MagicEnergySystem
{
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly OE14DayCycleSystem _dayCycle = default!;

    private void InitializeDraw()
    {
        SubscribeLocalEvent<OE14MagicEnergyDrawComponent, MapInitEvent>(OnDrawMapInit);
        SubscribeLocalEvent<OE14MagicEnergyFromDamageComponent, DamageChangedEvent>(OnDamageChanged);
    }

    private void OnDamageChanged(Entity<OE14MagicEnergyFromDamageComponent> ent, ref DamageChangedEvent args)
    {
        if (args.DamageDelta is null || !args.DamageIncreased)
            return;

        foreach (var dict in args.DamageDelta.DamageDict)
        {
            if (dict.Value <= 0)
                continue;

            if (!ent.Comp.Damage.TryGetValue(dict.Key, out var modifier))
                continue;

            ChangeEnergy(ent.Owner, modifier * dict.Value, out _, out _, safe: true);
        }
    }

    private void OnDrawMapInit(Entity<OE14MagicEnergyDrawComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.NextUpdateTime = _gameTiming.CurTime + TimeSpan.FromSeconds(ent.Comp.Delay);
    }

    private void UpdateDraw(float frameTime)
    {
        UpdateEnergyContainer();
        UpdateEnergyCrystalSlot();
    }

    private void UpdateEnergyContainer()
    {
        var query = EntityQueryEnumerator<OE14MagicEnergyDrawComponent, OE14MagicEnergyContainerComponent>();
        while (query.MoveNext(out var uid, out var draw, out var magicContainer))
        {
            if (!draw.Enable)
                continue;

            if (draw.NextUpdateTime >= _gameTiming.CurTime)
                continue;

            if (TryComp<MobStateComponent>(uid, out var mobState) && !_mobState.IsAlive(uid, mobState))
                continue;

            draw.NextUpdateTime = _gameTiming.CurTime + TimeSpan.FromSeconds(draw.Delay);

            ChangeEnergy((uid, magicContainer), draw.Energy, out _, out _, draw.Safe);
        }

        var query2 = EntityQueryEnumerator<OE14MagicEnergyPhotosynthesisComponent, OE14MagicEnergyContainerComponent>();
        while (query2.MoveNext(out var uid, out var draw, out var magicContainer))
        {
            if (draw.NextUpdateTime >= _gameTiming.CurTime)
                continue;

            if (TryComp<MobStateComponent>(uid, out var mobState) && !_mobState.IsAlive(uid, mobState))
                continue;

            draw.NextUpdateTime = _gameTiming.CurTime + TimeSpan.FromSeconds(draw.Delay);

            var daylight = _dayCycle.UnderSunlight(uid);

            ChangeEnergy((uid, magicContainer), daylight ? draw.DaylightEnergy : draw.DarknessEnergy, out _, out _, true);
        }
    }

    private void UpdateEnergyCrystalSlot()
    {
        var query = EntityQueryEnumerator<OE14MagicEnergyDrawComponent, OE14MagicEnergyCrystalSlotComponent>();
        while (query.MoveNext(out var uid, out var draw, out var slot))
        {
            if (!draw.Enable)
                continue;

            if (draw.NextUpdateTime >= _gameTiming.CurTime)
                continue;

            draw.NextUpdateTime = _gameTiming.CurTime + TimeSpan.FromSeconds(draw.Delay);

            if (!_magicSlot.TryGetEnergyCrystalFromSlot(uid, out var energyEnt))
                continue;

            ChangeEnergy((energyEnt.Value, energyEnt.Value), draw.Energy, out _, out _, draw.Safe);
        }
    }
}
