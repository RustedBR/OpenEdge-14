using Content.Shared._OE14.Trading.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._OE14.Trading.Components;

/// <summary>
/// The server calculates all prices for all product items, saves them in this component at the station,
/// and synchronizes the data with the client for the entire round.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class OE14StationEconomyComponent : Component
{
    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<OE14TradingPositionPrototype>, int> Pricing = new();

    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<OE14TradingRequestPrototype>, int> RequestPricing = new();

    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<OE14TradingFactionPrototype>, HashSet<ProtoId<OE14TradingRequestPrototype>> > ActiveRequests = new();

    [DataField]
    public int MaxRequestCount = 5;
}
