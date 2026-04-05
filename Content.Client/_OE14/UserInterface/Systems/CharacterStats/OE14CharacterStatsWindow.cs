using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;
using Robust.Shared.Localization;

namespace Content.Client._OE14.UserInterface.Systems.CharacterStats;

public sealed class OE14CharacterStatsWindow : BaseWindow
{
    public Label StrengthValue { get; }
    public Label VitalityValue { get; }
    public Label DexterityValue { get; }
    public Label IntelligenceValue { get; }

    public ProgressBar StrengthBar { get; }
    public ProgressBar VitalityBar { get; }
    public ProgressBar DexterityBar { get; }
    public ProgressBar IntelligenceBar { get; }

    public RichTextLabel BonusesInfo { get; }

    public OE14CharacterStatsWindow()
    {
        MinSize = new Vector2(300, 250);
        SetSize = new Vector2(350, 400);

        var mainContainer = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            Margin = new Thickness(10)
        };

        // Title
        mainContainer.AddChild(new Label
        {
            Text = Loc.GetString("oe14-window-character-stats"),
            StyleClasses = { "LabelHeadingBigger" },
            HorizontalAlignment = HAlignment.Center,
            Margin = new Thickness(0, 0, 0, 10)
        });

        // Strength
        var strengthBox = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            Margin = new Thickness(5)
        };
        strengthBox.AddChild(new Label { Text = Loc.GetString("oe14-stat-strength-full") + ":", MinWidth = 100, VerticalAlignment = VAlignment.Center });
        strengthBox.AddChild(StrengthValue = new Label { Text = "10", HorizontalExpand = true, HorizontalAlignment = HAlignment.Right, VerticalAlignment = VAlignment.Center });
        mainContainer.AddChild(strengthBox);
        mainContainer.AddChild(StrengthBar = new ProgressBar { MinValue = 0, MaxValue = 50, Value = 10, MinHeight = 20, Margin = new Thickness(5, 0, 5, 10) });

        // Vitality
        var vitalityBox = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            Margin = new Thickness(5)
        };
        vitalityBox.AddChild(new Label { Text = Loc.GetString("oe14-stat-vitality-full") + ":", MinWidth = 100, VerticalAlignment = VAlignment.Center });
        vitalityBox.AddChild(VitalityValue = new Label { Text = "10", HorizontalExpand = true, HorizontalAlignment = HAlignment.Right, VerticalAlignment = VAlignment.Center });
        mainContainer.AddChild(vitalityBox);
        mainContainer.AddChild(VitalityBar = new ProgressBar { MinValue = 0, MaxValue = 50, Value = 10, MinHeight = 20, Margin = new Thickness(5, 0, 5, 10) });

        // Dexterity
        var dexterityBox = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            Margin = new Thickness(5)
        };
        dexterityBox.AddChild(new Label { Text = Loc.GetString("oe14-stat-dexterity-full") + ":", MinWidth = 100, VerticalAlignment = VAlignment.Center });
        dexterityBox.AddChild(DexterityValue = new Label { Text = "10", HorizontalExpand = true, HorizontalAlignment = HAlignment.Right, VerticalAlignment = VAlignment.Center });
        mainContainer.AddChild(dexterityBox);
        mainContainer.AddChild(DexterityBar = new ProgressBar { MinValue = 0, MaxValue = 50, Value = 10, MinHeight = 20, Margin = new Thickness(5, 0, 5, 10) });

        // Intelligence
        var intelligenceBox = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            Margin = new Thickness(5)
        };
        intelligenceBox.AddChild(new Label { Text = Loc.GetString("oe14-stat-intelligence-full") + ":", MinWidth = 100, VerticalAlignment = VAlignment.Center });
        intelligenceBox.AddChild(IntelligenceValue = new Label { Text = "10", HorizontalExpand = true, HorizontalAlignment = HAlignment.Right, VerticalAlignment = VAlignment.Center });
        mainContainer.AddChild(intelligenceBox);
        mainContainer.AddChild(IntelligenceBar = new ProgressBar { MinValue = 0, MaxValue = 50, Value = 10, MinHeight = 20, Margin = new Thickness(5, 0, 5, 10) });

        // Bonuses Info
        mainContainer.AddChild(new Control { MinHeight = 5 });
        mainContainer.AddChild(BonusesInfo = new RichTextLabel
        {
            HorizontalExpand = true,
            VerticalExpand = true,
            Text = Loc.GetString("oe14-bonuses-from-stats") + "\n" +
                   Loc.GetString("oe14-bonus-damage-multiplier") + " 1.0x\n" +
                   Loc.GetString("oe14-bonus-health") + ": 0\n" +
                   Loc.GetString("oe14-bonus-stamina") + ": 1.0x\n" +
                   Loc.GetString("oe14-bonus-mana") + ": 0"
        });

        AddChild(mainContainer);
    }
}
