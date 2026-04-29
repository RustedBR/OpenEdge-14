using Content.Shared._OE14.CommunicationCrystal;
using Robust.Client.UserInterface;

namespace Content.Client._OE14.CommunicationCrystal;

public sealed class OE14CommunicationCrystalBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private OE14CommunicationCrystalWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<OE14CommunicationCrystalWindow>();

        if (_window != null)
        {
            _window.OnSendMessage += (isGlobal, message) =>
            {
                SendMessage(new OE14CommunicationCrystalSendMessage(isGlobal, message));
            };

            _window.OnRemoveCrystal += () =>
            {
                SendMessage(new OE14CommunicationCrystalRemoveCrystal());
            };
        }
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        switch (state)
        {
            case OE14CommunicationCrystalUiState uiState:
                _window?.UpdateState(uiState);
                break;
        }
    }
}