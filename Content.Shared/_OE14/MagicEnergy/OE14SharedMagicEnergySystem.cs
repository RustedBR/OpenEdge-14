using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared._OE14.MagicEssence;
using Content.Shared.Alert;
using Content.Shared.Audio;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Rejuvenate;
using Content.Shared.Rounding;

namespace Content.Shared._OE14.MagicEnergy;

public abstract class OE14SharedMagicEnergySystem : EntitySystem
{
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly SharedAmbientSoundSystem _ambient = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<OE14MagicEnergyContainerComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<OE14MagicEnergyContainerComponent, ComponentShutdown>(OnComponentShutdown);
        SubscribeLocalEvent<OE14MagicEnergyContainerComponent, RejuvenateEvent>(OnRejuvenate);

        SubscribeLocalEvent<OE14MagicEnergyExaminableComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<OE14MagicEnergyAmbientSoundComponent, OE14SlotCrystalPowerChangedEvent>(OnSlotPowerChanged);
    }

    private void OnRejuvenate(Entity<OE14MagicEnergyContainerComponent> ent, ref RejuvenateEvent args)
    {
        ChangeEnergy((ent, ent.Comp), ent.Comp.MaxEnergy - ent.Comp.Energy, out var deltaEnergy, out var overloadEnergy, true);
    }

    private void OnComponentStartup(Entity<OE14MagicEnergyContainerComponent> ent, ref ComponentStartup args)
    {
        UpdateMagicAlert(ent);
    }

    private void OnComponentShutdown(Entity<OE14MagicEnergyContainerComponent> ent, ref ComponentShutdown args)
    {
        if (ent.Comp.MagicAlert is null)
            return;

        _alerts.ClearAlert(ent.Owner, ent.Comp.MagicAlert.Value);
    }

    private void OnExamined(Entity<OE14MagicEnergyExaminableComponent> ent, ref ExaminedEvent args)
    {
        if (!TryComp<OE14MagicEnergyContainerComponent>(ent, out var magicContainer))
            return;

        if (!args.IsInDetailsRange)
            return;

        args.PushMarkup(GetEnergyExaminedText((ent, magicContainer)));
    }

    private void OnSlotPowerChanged(Entity<OE14MagicEnergyAmbientSoundComponent> ent, ref OE14SlotCrystalPowerChangedEvent args)
    {
        _ambient.SetAmbience(ent, args.Powered);
    }

    private void UpdateMagicAlert(Entity<OE14MagicEnergyContainerComponent> ent)
    {
        if (ent.Comp.MagicAlert is null)
            return;

        var level = ContentHelpers.RoundToLevels(
            MathF.Max(0f, (float) ent.Comp.Energy),
            (float) ent.Comp.MaxEnergy,
            _alerts.GetMaxSeverity(ent.Comp.MagicAlert.Value));

        _alerts.ShowAlert(ent.Owner, ent.Comp.MagicAlert.Value, (short) level);
    }

    public void ChangeEnergy(Entity<OE14MagicEnergyContainerComponent?> ent,
        FixedPoint2 energy,
        out FixedPoint2 deltaEnergy,
        out FixedPoint2 overloadEnergy,
        bool safe = false)
    {
        deltaEnergy = 0;
        overloadEnergy = 0;

        if (!Resolve(ent, ref ent.Comp, false))
            return;

        if (!safe)
        {
            // Overload
            if (ent.Comp.Energy + energy > ent.Comp.MaxEnergy && ent.Comp.UnsafeSupport)
            {
                overloadEnergy = ent.Comp.Energy + energy - ent.Comp.MaxEnergy;
                RaiseLocalEvent(ent, new OE14MagicEnergyOverloadEvent(overloadEnergy));
            }

            // Burn out
            if (ent.Comp.Energy + energy < 0 && ent.Comp.UnsafeSupport)
            {
                overloadEnergy = ent.Comp.Energy + energy;
                RaiseLocalEvent(ent, new OE14MagicEnergyBurnOutEvent(-energy - ent.Comp.Energy));
            }
        }

        var oldEnergy = ent.Comp.Energy;
        var newEnergy = Math.Clamp((float) ent.Comp.Energy + (float) energy, 0, (float) ent.Comp.MaxEnergy);

        deltaEnergy = newEnergy - oldEnergy;
        ent.Comp.Energy = newEnergy;
        Dirty(ent);

        if (oldEnergy != newEnergy)
            RaiseLocalEvent(ent, new OE14MagicEnergyLevelChangeEvent(oldEnergy, newEnergy, ent.Comp.MaxEnergy));

        UpdateMagicAlert((ent, ent.Comp));
    }

    /// <summary>
    /// Set energy to 0
    /// </summary>
    public void ClearEnergy(Entity<OE14MagicEnergyContainerComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return;

        ChangeEnergy(ent, -ent.Comp.Energy, out _, out _);
    }

    public void TransferEnergy(Entity<OE14MagicEnergyContainerComponent?> sender,
        Entity<OE14MagicEnergyContainerComponent?> receiver,
        FixedPoint2 energy,
        out FixedPoint2 deltaEnergy,
        out FixedPoint2 overloadEnergy,
        bool safe = false)
    {
        deltaEnergy = 0;
        overloadEnergy = 0;

        if (!Resolve(sender, ref sender.Comp) || !Resolve(receiver, ref receiver.Comp))
            return;

        var transferEnergy = energy;
        //We check how much space is left in the container so as not to overload it, but only if it does not support overloading
        if (!receiver.Comp.UnsafeSupport || safe)
        {
            var freeSpace = receiver.Comp.MaxEnergy - receiver.Comp.Energy;
            transferEnergy = FixedPoint2.Min(freeSpace, energy);
        }

        ChangeEnergy(sender, -transferEnergy, out var change, out var overload, safe);
        ChangeEnergy(receiver , -(change + overload), out deltaEnergy, out overloadEnergy, safe);
    }

    public bool HasEnergy(EntityUid uid, FixedPoint2 energy, OE14MagicEnergyContainerComponent? component = null, bool safe = false)
    {
        if (!Resolve(uid, ref component))
            return false;

        if (!safe && component.UnsafeSupport)
            return true;

        return component.Energy >= energy;
    }

    public string GetEnergyExaminedText(Entity<OE14MagicEnergyContainerComponent> ent)
    {
        var power = (int) (ent.Comp.Energy / ent.Comp.MaxEnergy * 100);

        // TODO: customization for examined

        var color = "#3fc488";
        if (power < 66)
            color = "#f2a93a";

        if (power < 33)
            color = "#c23030";

        return Loc.GetString("oe14-magic-energy-scan-result",
            ("item", MetaData(ent).EntityName),
            ("power", power),
            ("color", color));
    }

    public void ChangeMaximumEnergy(Entity<OE14MagicEnergyContainerComponent?> ent, FixedPoint2 energy)
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return;

        ent.Comp.MaxEnergy += energy;

        ChangeEnergy(ent, energy, out _, out _);
    }
}

/// <summary>
/// It's triggered when the energy change in MagicEnergyContainer
/// </summary>
public sealed class OE14MagicEnergyLevelChangeEvent : EntityEventArgs
{
    public readonly FixedPoint2 OldValue;
    public readonly FixedPoint2 NewValue;
    public readonly FixedPoint2 MaxValue;

    public OE14MagicEnergyLevelChangeEvent(FixedPoint2 oldValue, FixedPoint2 newValue, FixedPoint2 maxValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
        MaxValue = maxValue;
    }
}

/// <summary>
/// It's triggered when more energy enters the MagicEnergyContainer than it can hold.
/// </summary>
public sealed class OE14MagicEnergyOverloadEvent : EntityEventArgs
{
    public readonly FixedPoint2 OverloadEnergy;

    public OE14MagicEnergyOverloadEvent(FixedPoint2 overloadEnergy)
    {
        OverloadEnergy = overloadEnergy;
    }
}

/// <summary>
/// It's triggered they something try to get energy out of MagicEnergyContainer that is lacking there.
/// </summary>
public sealed class OE14MagicEnergyBurnOutEvent : EntityEventArgs
{
    public readonly FixedPoint2 BurnOutEnergy;

    public OE14MagicEnergyBurnOutEvent(FixedPoint2 burnOutEnergy)
    {
        BurnOutEnergy = burnOutEnergy;
    }
}
