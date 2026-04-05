using Content.Shared._OE14.Trading.Prototypes;
using Content.Shared._OE14.Trading.Systems;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Trading;

public sealed partial class OE14AddTradingReputationSpecial : JobSpecial
{
    [DataField]
    public float Reputation = 1f;

    [DataField]
    public HashSet<ProtoId<OE14TradingFactionPrototype>> Factions = new();

    public override void AfterEquip(EntityUid mob)
    {
        var entMan = IoCManager.Resolve<IEntityManager>();
        var tradeSys = entMan.System<OE14SharedTradingPlatformSystem>();

        foreach (var faction in Factions)
        {
            tradeSys.AddReputation(mob, faction, Reputation);
        }
    }
}
