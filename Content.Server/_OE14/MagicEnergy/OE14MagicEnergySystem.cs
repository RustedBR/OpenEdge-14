using Content.Server._OE14.MagicEnergy.Components;
using Content.Shared._OE14.MagicEnergy;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared.Cargo;
using Content.Shared.FixedPoint;
using Robust.Shared.Timing;

namespace Content.Server._OE14.MagicEnergy;

public sealed partial class OE14MagicEnergySystem : OE14SharedMagicEnergySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly OE14MagicEnergyCrystalSlotSystem _magicSlot = default!;

    public override void Initialize()
    {
        base.Initialize();

        InitializeDraw();
        InitializePortRelay();

        SubscribeLocalEvent<OE14MagicEnergyContainerComponent, PriceCalculationEvent>(OnMagicEnergyPriceCalculation);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        UpdateDraw(frameTime);
        UpdatePortRelay(frameTime);
    }

    private void OnMagicEnergyPriceCalculation(Entity<OE14MagicEnergyContainerComponent> ent, ref PriceCalculationEvent args)
    {
        args.Price += (double)(ent.Comp.Energy * 0.1f);
    }

    /// <summary>
    /// Convenience overload: changes maximum energy by uid without needing the component reference.
    /// </summary>
    public void ChangeMaximumEnergy(EntityUid uid, FixedPoint2 delta)
    {
        ChangeMaximumEnergy((uid, (OE14MagicEnergyContainerComponent?)null), delta);
    }

    /// <summary>
    /// Returns the current maximum energy of an entity's mana container, or null if none.
    /// Used by external systems (e.g., OE14CharacterStatsSystem) to read MaxEnergy
    /// without violating the [Access] restriction on OE14MagicEnergyContainerComponent.
    /// </summary>
    public FixedPoint2? GetMaxEnergy(EntityUid uid)
    {
        if (!TryComp<OE14MagicEnergyContainerComponent>(uid, out var mana))
            return null;

        return mana.MaxEnergy;
    }

    /// <summary>
    /// Sets the mana regeneration rate for an entity's OE14MagicEnergyDrawComponent.
    /// Called by OE14CharacterStatsSystem to scale regen with Intelligence.
    /// </summary>
    public void SetDrawRate(EntityUid uid, FixedPoint2 energyPerTick, float delay)
    {
        if (!TryComp<OE14MagicEnergyDrawComponent>(uid, out var draw))
            return;

        draw.Energy = energyPerTick;
        draw.Delay = delay;
    }
}
