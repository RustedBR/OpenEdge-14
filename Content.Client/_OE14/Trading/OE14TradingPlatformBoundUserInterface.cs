using Content.Shared._OE14.Trading;
using Content.Shared._OE14.Trading.Systems;
using Robust.Client.UserInterface;

namespace Content.Client._OE14.Trading;

public sealed class OE14TradingPlatformBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private OE14TradingPlatformWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<OE14TradingPlatformWindow>();

        _window.OnBuy += pos => SendMessage(new OE14TradingPositionBuyAttempt(pos));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        switch (state)
        {
            case OE14TradingPlatformUiState storeState:
                _window?.UpdateState(storeState);
                break;
        }
    }
}
