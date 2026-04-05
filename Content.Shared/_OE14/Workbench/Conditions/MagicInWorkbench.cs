using Content.Shared._OE14.MagicEnergy;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Workbench.Conditions;

public sealed partial class MagicInWorkbench : OE14WorkbenchCraftCondition
{
    [DataField]
    public FixedPoint2 Energy = 10;

    public override bool CheckCondition(
        EntityManager entManager,
        IPrototypeManager protoManager,
        EntityUid workbench,
        EntityUid user)
    {
        if (!entManager.TryGetComponent<OE14MagicEnergyContainerComponent>(workbench, out var energyContainer))
            return false;

        return energyContainer.Energy >= Energy;
    }

    public override void PostCraft(
        EntityManager entManager,
        IPrototypeManager protoManager,
        EntityUid workbench,
        EntityUid user)
    {
        var magicSys = entManager.System<OE14SharedMagicEnergySystem>();

        magicSys.ChangeEnergy(workbench, -Energy, out _, out _);
    }

    public override void FailedEffect(
        EntityManager entManager,
        IPrototypeManager protoManager,
        EntityUid workbench,
        EntityUid user)
    {
        var magicSys = entManager.System<OE14SharedMagicEnergySystem>();
        magicSys.ChangeEnergy(workbench, -Energy, out _, out _);

        if (entManager.TryGetComponent<TransformComponent>(workbench, out var xform))
            entManager.SpawnAtPosition("OE14SkyLightning", xform.Coordinates);
    }

    public override string GetConditionTitle(
        EntityManager entManager,
        IPrototypeManager protoManager,
        EntityUid workbench,
        EntityUid user)
    {
        if (!entManager.TryGetComponent<OE14MagicEnergyContainerComponent>(workbench, out var energyContainer))
            return string.Empty;
        var manaProcent = Energy / energyContainer.MaxEnergy * 100;

        return Loc.GetString("oe14-workbench-condition-mana-in-w", ("count", manaProcent));
    }
}
