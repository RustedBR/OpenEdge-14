/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Shared._OE14.Workbench;
using Robust.Client.UserInterface;

namespace Content.Client._OE14.Workbench;

public sealed class OE14WorkbenchBoundUserInterface : BoundUserInterface
{
    private OE14WorkbenchWindow? _window;

    public OE14WorkbenchBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<OE14WorkbenchWindow>();

        _window.OnCraft += entry => SendMessage(new OE14WorkbenchUiCraftMessage(entry.ProtoId));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        switch (state)
        {
            case OE14WorkbenchUiRecipesState recipesState:
                _window?.UpdateState(recipesState);
                break;
        }
    }
}
