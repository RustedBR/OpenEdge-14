using Content.Shared._OE14.Trading;
using Content.Shared._OE14.Trading.Systems;
using Robust.Client.UserInterface;

namespace Content.Client._OE14.Trading.Selling;

public sealed class OE14SellingPlatformBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private OE14SellingPlatformWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<OE14SellingPlatformWindow>();

        _window.OnSell += () => SendMessage(new OE14TradingSellAttempt());
        _window.OnRequestSell += pair => SendMessage(new OE14TradingRequestSellAttempt(pair.Item1, pair.Item2));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        switch (state)
        {
            case OE14SellingPlatformUiState storeState:
                _window?.UpdateState(storeState);
                break;
        }
    }
}
