using Content.Shared._OE14.MagicEnergy;
using Content.Shared.Bed.Sleep;
using Content.Shared.Damage;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.StatusEffectNew;
using Content.Shared.Trigger.Systems;

namespace Content.Shared._OE14.MagicWeakness;

public abstract class OE14SharedMagicWeaknessSystem : EntitySystem
{
    [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly TriggerSystem _trigger = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14MagicUnsafeDamageComponent, OE14MagicEnergyBurnOutEvent>(OnMagicEnergyBurnOutDamage);
        SubscribeLocalEvent<OE14MagicUnsafeDamageComponent, OE14MagicEnergyOverloadEvent>(OnMagicEnergyOverloadDamage);

        SubscribeLocalEvent<OE14MagicUnsafeSleepComponent, OE14MagicEnergyBurnOutEvent>(OnMagicEnergyBurnOutSleep);
        SubscribeLocalEvent<OE14MagicUnsafeSleepComponent, OE14MagicEnergyOverloadEvent>(OnMagicEnergyOverloadSleep);

        SubscribeLocalEvent<OE14MagicUnsafeTriggerComponent, OE14MagicEnergyBurnOutEvent>(OnMagicEnergyBurnOutTrigger);
        SubscribeLocalEvent<OE14MagicUnsafeTriggerComponent, OE14MagicEnergyOverloadEvent>(OnMagicEnergyOverloadTrigger);
    }

    private void OnMagicEnergyBurnOutSleep(Entity<OE14MagicUnsafeSleepComponent> ent,
        ref OE14MagicEnergyBurnOutEvent args)
    {
        if (args.BurnOutEnergy > ent.Comp.SleepThreshold)
        {
            _popup.PopupEntity(Loc.GetString("oe14-magic-energy-damage-burn-out-fall"),
                ent,
                ent,
                PopupType.LargeCaution);
            _statusEffects.TryAddStatusEffectDuration(
                ent,
                SleepingSystem.StatusEffectForcedSleeping,
                TimeSpan.FromSeconds(ent.Comp.SleepPerEnergy * (float)args.BurnOutEnergy));
        }
    }

    private void OnMagicEnergyOverloadSleep(Entity<OE14MagicUnsafeSleepComponent> ent,
        ref OE14MagicEnergyOverloadEvent args)
    {
        if (args.OverloadEnergy > ent.Comp.SleepThreshold)
        {
            _popup.PopupEntity(Loc.GetString("oe14-magic-energy-damage-burn-out-fall"),
                ent,
                ent,
                PopupType.LargeCaution);
            _statusEffects.TryAddStatusEffectDuration(
                ent,
                SleepingSystem.StatusEffectForcedSleeping,
                TimeSpan.FromSeconds(ent.Comp.SleepPerEnergy * (float)args.OverloadEnergy));
        }
    }

    private void OnMagicEnergyOverloadTrigger(Entity<OE14MagicUnsafeTriggerComponent> ent, ref OE14MagicEnergyOverloadEvent args)
    {
        _trigger.Trigger(ent);
    }

    private void OnMagicEnergyBurnOutTrigger(Entity<OE14MagicUnsafeTriggerComponent> ent, ref OE14MagicEnergyBurnOutEvent args)
    {
        _trigger.Trigger(ent);
    }

    private void OnMagicEnergyBurnOutDamage(Entity<OE14MagicUnsafeDamageComponent> ent,
        ref OE14MagicEnergyBurnOutEvent args)
    {
        //TODO: Idk why this dont popup recipient
        //Others popup
        _popup.PopupPredicted(Loc.GetString("oe14-magic-energy-damage-burn-out"),
            Loc.GetString("oe14-magic-energy-damage-burn-out-other", ("name", Identity.Name(ent, EntityManager))),
            ent,
            ent);

        //Local self popup
        _popup.PopupEntity(
            Loc.GetString("oe14-magic-energy-damage-burn-out"),
            ent,
            ent,
            PopupType.LargeCaution);

        _damageable.TryChangeDamage(ent, ent.Comp.DamagePerEnergy * args.BurnOutEnergy, interruptsDoAfters: false);
    }

    private void OnMagicEnergyOverloadDamage(Entity<OE14MagicUnsafeDamageComponent> ent,
        ref OE14MagicEnergyOverloadEvent args)
    {
        //TODO: Idk why this dont popup recipient
        //Others popup
        _popup.PopupPredicted(Loc.GetString("oe14-magic-energy-damage-overload"),
            Loc.GetString("oe14-magic-energy-damage-overload-other", ("name", Identity.Name(ent, EntityManager))),
            ent,
            ent);

        //Local self popup
        _popup.PopupEntity(
            Loc.GetString("oe14-magic-energy-damage-overload"),
            ent,
            ent,
            PopupType.LargeCaution);

        _damageable.TryChangeDamage(ent, ent.Comp.DamagePerEnergy * args.OverloadEnergy, interruptsDoAfters: false);
    }
}
