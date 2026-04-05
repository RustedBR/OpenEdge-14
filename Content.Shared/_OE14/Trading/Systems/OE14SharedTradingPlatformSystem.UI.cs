using Content.Shared._OE14.Trading.Components;
using Content.Shared._OE14.Trading.Prototypes;
using Content.Shared.UserInterface;

namespace Content.Shared._OE14.Trading.Systems;

public abstract partial class OE14SharedTradingPlatformSystem
{
    private void InitializeUI()
    {
        SubscribeLocalEvent<OE14TradingPlatformComponent, BeforeActivatableUIOpenEvent>(OnBeforeTradingUIOpen);
    }

    private void OnBeforeTradingUIOpen(Entity<OE14TradingPlatformComponent> ent, ref BeforeActivatableUIOpenEvent args)
    {
        UpdateTradingUIState(ent, args.User);
    }

    protected void UpdateTradingUIState(Entity<OE14TradingPlatformComponent> ent, EntityUid user)
    {
        _userInterface.SetUiState(ent.Owner, OE14TradingUiKey.Buy, new OE14TradingPlatformUiState(GetNetEntity(ent)));
    }

    public string GetTradeDescription(OE14TradingPositionPrototype position)
    {
        if (position.Desc != null)
            return Loc.GetString(position.Desc);

        if (position.Service is null)
            return string.Empty;

        return position.Service.GetDesc(Proto);
    }

    public string GetTradeName(OE14TradingPositionPrototype position)
    {
        if (position.Name != null)
            return Loc.GetString(position.Name);

        if (position.Service is null)
            return string.Empty;

        return position.Service.GetName(Proto);
    }
}
