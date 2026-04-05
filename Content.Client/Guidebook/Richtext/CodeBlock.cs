using JetBrains.Annotations;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook.Richtext;

/// <summary>
///     A code block for the guidebook. Renders raw text in monospace font with a dark background.
///     Use as: &lt;CodeBlock&gt;code here&lt;/CodeBlock&gt;
///     Content is parsed as raw text — C# generics, braces, brackets, and other special characters
///     work without escaping.
/// </summary>
[UsedImplicitly]
public sealed class CodeBlock : PanelContainer
{
    public CodeBlock(string content)
    {
        HorizontalExpand = true;
        Margin = new Thickness(0, 5, 0, 10);

        var styleBox = new StyleBoxFlat
        {
            BackgroundColor = Color.FromHex("#111827"),
            BorderColor = Color.FromHex("#374151"),
            BorderThickness = new Thickness(1),
            ContentMarginLeftOverride = 10,
            ContentMarginRightOverride = 10,
            ContentMarginTopOverride = 8,
            ContentMarginBottomOverride = 8,
        };
        PanelOverride = styleBox;

        var trimmed = content.Trim('\n', '\r').TrimEnd();

        var rt = new RichTextLabel { HorizontalExpand = true };
        var msg = new FormattedMessage();
        // Use markup only for font/color tags — the actual code content is added via AddText
        // so that C# brackets, generics, and other special chars are not interpreted as rich text.
        msg.TryAddMarkup("[font=Monospace][color=#D4D4D4]", out _);
        msg.AddText(trimmed);
        msg.TryAddMarkup("[/color][/font]", out _);
        rt.SetMessage(msg);

        AddChild(rt);
    }
}
