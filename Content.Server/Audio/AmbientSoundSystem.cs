using Content.Server._OE14.Temperature;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared._OE14.Temperature;
using Content.Shared.Audio;
using Content.Shared.Mobs;
using Content.Shared.Power;

namespace Content.Server.Audio;

public sealed class AmbientSoundSystem : SharedAmbientSoundSystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AmbientOnPoweredComponent, PowerChangedEvent>(HandlePowerChange);
        SubscribeLocalEvent<AmbientOnPoweredComponent, PowerNetBatterySupplyEvent>(HandlePowerSupply);
        SubscribeLocalEvent<OE14FlammableAmbientSoundComponent, OnFireChangedEvent>(OnFireChanged); //CrystallEdge bonfire moment
    }

    private void HandlePowerSupply(EntityUid uid, AmbientOnPoweredComponent component, ref PowerNetBatterySupplyEvent args)
    {
        SetAmbience(uid, args.Supply);
    }

    private void HandlePowerChange(EntityUid uid, AmbientOnPoweredComponent component, ref PowerChangedEvent args)
    {
        SetAmbience(uid, args.Powered);
    }

    //CrystallEdge bonfire moment
    private void OnFireChanged(Entity<OE14FlammableAmbientSoundComponent> ent, ref OnFireChangedEvent args)
    {
        SetAmbience(ent, args.OnFire);
    }
    //CrystallEdge bonfire moment end
}
