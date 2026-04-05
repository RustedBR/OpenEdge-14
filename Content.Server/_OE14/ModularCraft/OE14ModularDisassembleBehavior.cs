using Content.Server.Destructible;
using Content.Server.Destructible.Thresholds.Behaviors;

namespace Content.Server._OE14.ModularCraft;

[Serializable]
[DataDefinition]
public sealed partial class OE14ModularDisassembleBehavior : IThresholdBehavior
{
    public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
    {
        var modular = system.EntityManager.System<OE14ModularCraftSystem>();
        modular.DisassembleModular(owner);
    }
}
