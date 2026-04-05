using System.Numerics;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Client._OE14.UserInterface.Systems.NodeTree;

public sealed class OE14NodeTreeElement(string nodeKey, bool gained ,bool active, Vector2 uiPosition, SpriteSpecifier? icon = null)
{
    public string NodeKey = nodeKey;
    public bool Gained = gained;
    public bool Active = active;

    public Vector2 UiPosition = uiPosition;
    public SpriteSpecifier? Icon = icon;
}

public sealed class OE14NodeTreeUiState(
    HashSet<OE14NodeTreeElement> nodes,
    HashSet<(string, string)>? edges = null,
    SpriteSpecifier? frameIcon = null,
    SpriteSpecifier? hoveredIcon = null,
    SpriteSpecifier? selectedIcon = null,
    SpriteSpecifier? learnedIcon = null,
    Color? lineColor = null,
    Color? activeLineColor = null
    ) : BoundUserInterfaceState
{
    public HashSet<OE14NodeTreeElement> Nodes = nodes;
    public HashSet<(string, string)> Edges = edges ?? new HashSet<(string, string)>();

    public SpriteSpecifier FrameIcon = frameIcon ?? new SpriteSpecifier.Rsi(new ResPath("/Textures/_OE14/Interface/NodeTree/default.rsi"), "frame");
    public SpriteSpecifier HoveredIcon = hoveredIcon ?? new SpriteSpecifier.Rsi(new ResPath("/Textures/_OE14/Interface/NodeTree/default.rsi"), "hovered");
    public SpriteSpecifier SelectedIcon = selectedIcon?? new SpriteSpecifier.Rsi(new ResPath("/Textures/_OE14/Interface/NodeTree/default.rsi"), "selected");
    public SpriteSpecifier LearnedIcon = learnedIcon?? new SpriteSpecifier.Rsi(new ResPath("/Textures/_OE14/Interface/NodeTree/default.rsi"), "learned");

    public Color LineColor = lineColor ?? Color.Gray;
    public Color ActiveLineColor = activeLineColor ?? Color.White;
}
