using Content.Shared._OE14.Trading.Components;
using Content.Shared._OE14.Trading.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Trading.Systems;

public abstract partial class OE14SharedStationEconomySystem : EntitySystem
{
    public int? GetPrice(ProtoId<OE14TradingPositionPrototype> position)
    {
        var query = EntityQueryEnumerator<OE14StationEconomyComponent>();

        while (query.MoveNext(out var uid, out var economy))
        {
            if (!economy.Pricing.TryGetValue(position, out var price))
                return null;

            return price;
        }

        return null;
    }

    public int? GetPrice(ProtoId<OE14TradingRequestPrototype> request)
    {
        var query = EntityQueryEnumerator<OE14StationEconomyComponent>();

        while (query.MoveNext(out var uid, out var economy))
        {
            if (!economy.RequestPricing.TryGetValue(request, out var price))
                return null;

            return price;
        }

        return null;
    }

    public HashSet<ProtoId<OE14TradingRequestPrototype>> GetRequests(ProtoId<OE14TradingFactionPrototype> faction)
    {
        var query = EntityQueryEnumerator<OE14StationEconomyComponent>();

        while (query.MoveNext(out var uid, out var economy))
        {
            if (!economy.ActiveRequests.TryGetValue(faction, out var requests))
                continue;

            return requests;
        }

        return [];
    }
}
