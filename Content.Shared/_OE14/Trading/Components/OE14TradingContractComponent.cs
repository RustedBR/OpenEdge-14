using Content.Shared._OE14.Trading.Prototypes;
using Content.Shared._OE14.Trading.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Trading.Components;

[RegisterComponent, Access(typeof(OE14SharedTradingPlatformSystem))]
public sealed partial class OE14TradingContractComponent : Component
{
    [DataField]
    public ProtoId<OE14TradingFactionPrototype> Faction;
}
