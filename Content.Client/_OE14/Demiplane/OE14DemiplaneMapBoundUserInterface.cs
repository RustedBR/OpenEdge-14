using Content.Shared._OE14.Demiplane;
using Robust.Client.UserInterface;

namespace Content.Client._OE14.Demiplane;

public sealed class OE14DemiplaneMapBoundUserInterface : BoundUserInterface
{
    private OE14DemiplaneMapWindow? _window;

    public OE14DemiplaneMapBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        IoCManager.InjectDependencies(this);
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<OE14DemiplaneMapWindow>();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (_window == null || state is not OE14DemiplaneMapUiState mapState)
            return;

        _window?.UpdateState(mapState);
    }
}
