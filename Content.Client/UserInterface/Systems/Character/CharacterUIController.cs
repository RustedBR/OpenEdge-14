using System.Linq;
using Content.Client.CharacterInfo;
using Content.Client.Gameplay;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Character.Controls;
using Content.Client.UserInterface.Systems.Character.Windows;
using Content.Client.UserInterface.Systems.Objectives.Controls;
using Content.Client._OE14.CharacterStats;
using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Input;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Roles;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input.Binding;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using static Content.Client.CharacterInfo.CharacterInfoSystem;
using static Robust.Client.UserInterface.Controls.BaseButton;

namespace Content.Client.UserInterface.Systems.Character;

[UsedImplicitly]
public sealed class CharacterUIController : UIController,
    IOnStateEntered<GameplayState>,
    IOnStateExited<GameplayState>,
    IOnSystemChanged<CharacterInfoSystem>
{
    [Dependency] private readonly IEntityManager _ent = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    [UISystemDependency] private readonly CharacterInfoSystem _characterInfo = default!;
    [UISystemDependency] private readonly SpriteSystem _sprite = default!;
    [UISystemDependency] private readonly OE14ClientCharacterStatsSystem _statsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<MindRoleTypeChangedEvent>(OnRoleTypeChanged);
    }

    private CharacterWindow? _window;
    private MenuButton? CharacterButton => UIManager.GetActiveUIWidgetOrNull<MenuBar.Widgets.GameTopMenuBar>()?.CharacterButton;

    // Dual-layer stat bars: (foreground panel, setter callback)
    private Action<float>? _strBarSetter;
    private Action<float>? _vitBarSetter;
    private Action<float>? _dexBarSetter;
    private Action<float>? _intBarSetter;

    private static readonly Color StrColor  = Color.FromHex("#d4a84b");
    private static readonly Color VitColor  = Color.FromHex("#e05050");
    private static readonly Color DexColor  = Color.FromHex("#50c878");
    private static readonly Color IntColor  = Color.FromHex("#5090e0");

    public void OnStateEntered(GameplayState state)
    {
        DebugTools.Assert(_window == null);

        _window = UIManager.CreateWindow<CharacterWindow>();
        LayoutContainer.SetAnchorPreset(_window, LayoutContainer.LayoutPreset.CenterTop);

        _window.OnClose += DeactivateButton;
        _window.OnOpen  += ActivateButton;

        _window.StrengthSpendButton.OnPressed     += _ => OnSpendButtonPressed("strength");
        _window.VitalitySpendButton.OnPressed     += _ => OnSpendButtonPressed("vitality");
        _window.DexteritySpendButton.OnPressed    += _ => OnSpendButtonPressed("dexterity");
        _window.IntelligenceSpendButton.OnPressed += _ => OnSpendButtonPressed("intelligence");

        // Build dual-color bars and inject into the slot containers
        BuildStatBar(_window.StrBarSlot, StrColor, out _strBarSetter);
        BuildStatBar(_window.VitBarSlot, VitColor, out _vitBarSetter);
        BuildStatBar(_window.DexBarSlot, DexColor, out _dexBarSetter);
        BuildStatBar(_window.IntBarSlot, IntColor, out _intBarSetter);

        if (_statsSystem != null)
            _statsSystem.OnStatsUpdated += OnStatsUpdated;

        CommandBinds.Builder
            .Bind(ContentKeyFunctions.OpenCharacterMenu,
                InputCmdHandler.FromDelegate(_ => ToggleWindow()))
            .Register<CharacterUIController>();

        // TODO: Bind ToggleDebugPanel to a key (F8 ShowDebug or custom)
    }

    public void OnStateExited(GameplayState state)
    {
        if (_window != null)
        {
            _window.Close();
            _window = null;
        }

        if (_statsSystem != null)
            _statsSystem.OnStatsUpdated -= OnStatsUpdated;
        CommandBinds.Unregister<CharacterUIController>();
    }

    // ── Dual-color bar builder ────────────────────────────────────────────────

    /// <summary>
    /// Creates a two-layer bar inside <paramref name="slot"/>:
    /// - Background: very dark version of the stat color
    /// - Foreground: full stat color, width tracks value/10
    /// Returns a setter callback to update the fill ratio (0-1).
    /// </summary>
    private static void BuildStatBar(BoxContainer slot, Color color, out Action<float> setter)
    {
        var dark = new Color(color.R * 0.18f, color.G * 0.18f, color.B * 0.18f);

        var layout = new LayoutContainer { HorizontalExpand = true, MinHeight = 10 };

        var bg = new PanelContainer { PanelOverride = new StyleBoxFlat { BackgroundColor = dark } };
        LayoutContainer.SetAnchorRight(bg, 1f);
        LayoutContainer.SetAnchorBottom(bg, 1f);
        layout.AddChild(bg);

        var fg = new PanelContainer { PanelOverride = new StyleBoxFlat { BackgroundColor = color } };
        LayoutContainer.SetAnchorRight(fg, 0f);
        LayoutContainer.SetAnchorBottom(fg, 1f);
        layout.AddChild(fg);

        slot.AddChild(layout);

        setter = ratio =>
        {
            ratio = Math.Clamp(ratio, 0f, 1f);
            LayoutContainer.SetAnchorRight(fg, ratio);
        };
    }

    // ── Stats events ─────────────────────────────────────────────────────────

    private void OnStatsUpdated(EntityUid entity)
    {
        if (_window != null && _window.IsOpen)
            UpdateCharacterStats(entity);
    }

    private void UpdateCharacterStats(EntityUid entity)
    {
        if (_window == null || !_ent.TryGetComponent<OE14CharacterStatsComponent>(entity, out var stats))
        {
            if (_window != null)
                _window.StatsPanel.Visible = false;
            return;
        }

        _window.StatsPanel.Visible = true;

        // Effective values (base + modifier, clamped)
        var effStr = Math.Clamp(stats.Strength + stats.StrengthModifier,     1, stats.MaxStatValue);
        var effVit = Math.Clamp(stats.Vitality + stats.VitalityModifier,     1, stats.MaxStatValue);
        var effDex = Math.Clamp(stats.Dexterity + stats.DexterityModifier,   1, stats.MaxStatValue);
        var effInt = Math.Clamp(stats.Intelligence + stats.IntelligenceModifier, 1, stats.MaxStatValue);

        // Column 2: Base stats
        _window.StrengthBaseLabel.Text     = Loc.GetString("oe14-stat-strength-label", ("value", stats.Strength));
        _window.VitalityBaseLabel.Text     = Loc.GetString("oe14-stat-vitality-label", ("value", stats.Vitality));
        _window.DexterityBaseLabel.Text    = Loc.GetString("oe14-stat-dexterity-label", ("value", stats.Dexterity));
        _window.IntelligenceBaseLabel.Text = Loc.GetString("oe14-stat-intelligence-label", ("value", stats.Intelligence));

        // Column 3: Current stats (effective value)
        _window.StrengthValue.Text     = effStr.ToString();
        _window.VitalityValue.Text     = effVit.ToString();
        _window.DexterityValue.Text    = effDex.ToString();
        _window.IntelligenceValue.Text = effInt.ToString();

        // Populate modifier containers with colored labels
        PopulateModifierContainer(_window.StrengthModContainer, stats.StrengthModifiers);
        PopulateModifierContainer(_window.VitalityModContainer, stats.VitalityModifiers);
        PopulateModifierContainer(_window.DexterityModContainer, stats.DexterityModifiers);
        PopulateModifierContainer(_window.IntelligenceModContainer, stats.IntelligenceModifiers);

        // Update bar fills (ratio 0-1 over MaxStatValue)
        var max = (float)stats.MaxStatValue;
        _strBarSetter?.Invoke(effStr / max);
        _vitBarSetter?.Invoke(effVit / max);
        _dexBarSetter?.Invoke(effDex / max);
        _intBarSetter?.Invoke(effInt / max);

        // Available points + spend buttons (validation checks effective value, not just base)
        var hasPoints = stats.AvailablePoints > 0;
        _window.PointsRow.Visible         = hasPoints;
        _window.AvailablePointsValue.Text = stats.AvailablePoints.ToString();

        _window.StrengthSpendButton.Visible     = hasPoints && effStr < stats.MaxStatValue;
        _window.VitalitySpendButton.Visible     = hasPoints && effVit < stats.MaxStatValue;
        _window.DexteritySpendButton.Visible    = hasPoints && effDex < stats.MaxStatValue;
        _window.IntelligenceSpendButton.Visible = hasPoints && effInt < stats.MaxStatValue;

        // HP and thresholds: Show current HP or max if no damage component
        var maxHP = (int)(100f + stats.HealthBonus);
        var critThreshold = maxHP;
        var deadThreshold = critThreshold * 2;

        // Try to get current HP from DamageableComponent
        var currentHP = maxHP;
        if (_ent.TryGetComponent<DamageableComponent>(entity, out var damageComp))
        {
            currentHP = maxHP - (int)damageComp.TotalDamage;
        }

        _window.DerivedHP.Text = currentHP.ToString();

        // Thresholds shown as negatives below the HP value
        var critNeg = -critThreshold;
        var deadNeg = -deadThreshold;
        _window.DerivedHPThresholds.Text = Loc.GetString("oe14-stat-hp-thresholds-dynamic", ("crit", Math.Abs(critNeg)), ("dead", Math.Abs(deadNeg)));

        // Stamina: Show current damage vs max damage (like HP thresholds)
        var maxStamina     = (int)(100f * stats.StaminaMultiplier);
        var staminaSlow = (int)(maxStamina * 0.25f);
        var staminaKnockdown = maxStamina;

        var currentStaminaDamage = maxStamina;
        if (_ent.TryGetComponent<Content.Shared.Damage.Components.StaminaComponent>(entity, out var staminaComp))
        {
            currentStaminaDamage = maxStamina - (int)staminaComp.StaminaDamage;
            currentStaminaDamage = Math.Max(0, currentStaminaDamage);
        }

        _window.DerivedStamina.Text     = Loc.GetString("oe14-stat-mana-regen-dynamic", ("current", currentStaminaDamage), ("max", maxStamina));
        _window.DerivedStaminaSlow.Text = Loc.GetString("oe14-stat-stamina-slow-dynamic", ("slow", staminaSlow), ("knockdown", staminaKnockdown));

        // Mana: Show current mana vs max mana
        var maxMana      = (int)Math.Max(0f, 100f + stats.ManaBonus);
        var regenPerTick = maxMana / 100f;

        var currentMana = maxMana;
        if (_ent.TryGetComponent<Content.Shared._OE14.MagicEnergy.Components.OE14MagicEnergyContainerComponent>(entity, out var manaComp))
        {
            currentMana = (int)manaComp.Energy;
        }

        _window.DerivedMana.Text     = Loc.GetString("oe14-stat-mana-regen-dynamic", ("current", currentMana), ("max", maxMana));
        _window.DerivedManaRegen.Text = Loc.GetString("oe14-stat-mana-regen-value", ("regen", regenPerTick.ToString("0.##")));

        _window.DerivedDamage.Text = Loc.GetString("oe14-stat-damage-multiplier-dynamic", ("value", stats.DamageMultiplier.ToString("F2")));

        // Race info
        UpdateRaceInfo(entity, stats);

        // DEBUG: Update debug panel with raw modifier values
        UpdateDebugPanel(stats);
    }

    /// <summary>
    /// Updates debug panel showing raw modifier values from each source.
    /// </summary>
    private void UpdateDebugPanel(OE14CharacterStatsComponent stats)
    {
        if (_window == null || !_window.DebugPanel.Visible)
            return;

        var strDebug = FormatDebugModifiers("STR", stats.Strength, stats.StrengthModifiers);
        var vitDebug = FormatDebugModifiers("VIT", stats.Vitality, stats.VitalityModifiers);
        var dexDebug = FormatDebugModifiers("DEX", stats.Dexterity, stats.DexterityModifiers);
        var intDebug = FormatDebugModifiers("INT", stats.Intelligence, stats.IntelligenceModifiers);

        _window.DebugStrModifiers.Text = strDebug;
        _window.DebugVitModifiers.Text = vitDebug;
        _window.DebugDexModifiers.Text = dexDebug;
        _window.DebugIntModifiers.Text = intDebug;

        var effStr = Math.Clamp(stats.Strength + stats.StrengthModifier,     1, stats.MaxStatValue);
        var effVit = Math.Clamp(stats.Vitality + stats.VitalityModifier,     1, stats.MaxStatValue);
        var effDex = Math.Clamp(stats.Dexterity + stats.DexterityModifier,   1, stats.MaxStatValue);
        var effInt = Math.Clamp(stats.Intelligence + stats.IntelligenceModifier, 1, stats.MaxStatValue);

        _window.DebugEffectiveStats.Text = Loc.GetString("oe14-debug-effective-stats",
            ("str", effStr), ("vit", effVit), ("dex", effDex), ("int", effInt),
            ("points", stats.AvailablePoints));
    }

    /// <summary>
    /// Formats debug info: "STR: base=8 total_mod=+2 (Spell:+2, Spend:0, Item:0, Buff:0)"
    /// </summary>
    private static string FormatDebugModifiers(string stat, int baseVal, Dictionary<Content.Shared._OE14.CharacterStats.ModifierSource, int> modifiers)
    {
        var totalMod = 0;
        var parts = new System.Collections.Generic.List<string>();

        foreach (var (source, value) in modifiers)
        {
            totalMod += value;
            var sign = value >= 0 ? "+" : "";
            parts.Add($"{source}:{sign}{value}");
        }

        return $"{stat}: base={baseVal} total_mod={totalMod:+0;-0;0} ({string.Join(", ", parts)})";
    }

    /// <summary>
    /// Populates a BoxContainer with colored modifier labels.
    /// Creates individual labels for each modifier source with appropriate colors.
    /// </summary>
    private void PopulateModifierContainer(BoxContainer container, Dictionary<Content.Shared._OE14.CharacterStats.ModifierSource, int> modifiers)
    {
        container.RemoveAllChildren();

        if (modifiers == null || modifiers.Count == 0)
            return;

        bool first = true;
        foreach (var (source, value) in modifiers)
        {
            if (value == 0)
                continue;

            if (!first)
            {
                var separator = new Label { Text = ", ", FontColorOverride = Color.FromHex("#999999") };
                container.AddChild(separator);
            }

            var sign = value > 0 ? "+" : "";
            var abbrev = source switch
            {
                Content.Shared._OE14.CharacterStats.ModifierSource.Spell => "SPEL",
                Content.Shared._OE14.CharacterStats.ModifierSource.Spend => "SPND",
                Content.Shared._OE14.CharacterStats.ModifierSource.Item => "ITEM",
                Content.Shared._OE14.CharacterStats.ModifierSource.Buff => "BUFF",
                _ => "????"
            };

            var color = source switch
            {
                Content.Shared._OE14.CharacterStats.ModifierSource.Spell => Color.FromHex("#d946ef"),  // Magenta
                Content.Shared._OE14.CharacterStats.ModifierSource.Spend => Color.FromHex("#f0c050"),  // Gold
                Content.Shared._OE14.CharacterStats.ModifierSource.Item => Color.FromHex("#50c878"),   // Green
                Content.Shared._OE14.CharacterStats.ModifierSource.Buff => Color.FromHex("#5090e0"),   // Blue
                _ => Color.FromHex("#999999")
            };

            var label = new Label
            {
                Text = $"{sign}{value} {abbrev}",
                FontColorOverride = color
            };
            container.AddChild(label);
            first = false;
        }
    }

    /// <summary>
    /// Formats a stat value showing arrow only if modifiers exist.
    /// If no modifiers: returns empty (base shown in label only)
    /// If modifiers exist: returns "→ effective"
    /// </summary>
    private static string FormatStatValue(int baseVal, int effectiveVal, Dictionary<Content.Shared._OE14.CharacterStats.ModifierSource, int> modifiers)
    {
        // Check if there are any non-zero modifiers
        var hasModifiers = modifiers.Values.Any(v => v != 0);

        if (!hasModifiers)
            return ""; // No modifiers - show base in label only

        if (baseVal == effectiveVal)
            return ""; // Modifiers but value unchanged

        return $"→ {effectiveVal}"; // Show arrow and effective value
    }

    /// <summary>
    /// Formats modifier sources with abbreviations (no color markup - Labels don't support dynamic colors).
    /// SPEL: Spell - from skills
    /// SPND: Spend - from creation/player points
    /// ITEM: Item - from equipment
    /// BUFF: Buff - from temporary effects
    /// Example: "(+1 SPEL, +1 SPND)" or empty string if no modifiers.
    /// </summary>
    private static string FormatModifierSources(Dictionary<Content.Shared._OE14.CharacterStats.ModifierSource, int> modifiers)
    {
        if (modifiers == null)
            return "";

        var parts = new System.Collections.Generic.List<string>();

        foreach (var (source, value) in modifiers)
        {
            if (value == 0)
                continue;

            var sign = value > 0 ? "+" : "";

            // Abbreviation based on source
            var abbrev = source switch
            {
                Content.Shared._OE14.CharacterStats.ModifierSource.Spell => "SPEL", // Skills
                Content.Shared._OE14.CharacterStats.ModifierSource.Spend => "SPND", // Creation points
                Content.Shared._OE14.CharacterStats.ModifierSource.Item => "ITEM",  // Equipment
                Content.Shared._OE14.CharacterStats.ModifierSource.Buff => "BUFF",  // Temporary effects
                _ => "????"
            };

            parts.Add($"{sign}{value} {abbrev}");
        }

        if (parts.Count == 0)
            return "";

        return "(" + string.Join(", ", parts) + ")";
    }

    private void UpdateRaceInfo(EntityUid entity, OE14CharacterStatsComponent stats)
    {
        if (_window == null)
            return;

        if (!_ent.TryGetComponent<HumanoidAppearanceComponent>(entity, out var humanoid))
            return;

        if (!_prototypeManager.TryIndex<SpeciesPrototype>(humanoid.Species, out var specProto))
            return;

        _window.RaceLabel.Text = Loc.GetString(specProto.Name);
    }

    // ── Spend button ─────────────────────────────────────────────────────────

    private void OnSpendButtonPressed(string statName)
    {
        if (_player.LocalEntity is not { } entity)
            return;

        _statsSystem.RequestSpendPoint(entity, statName);
    }

    /// <summary>
    /// Toggle debug panel visibility (F8 key).
    /// </summary>
    private void ToggleDebugPanel()
    {
        if (_window != null && _window.DebugPanel.Visible)
        {
            _window.DebugPanel.Visible = false;
        }
        else if (_window != null)
        {
            _window.DebugPanel.Visible = true;
        }
    }

    // ── Character updated (objectives, sprite, etc.) ─────────────────────────

    public void OnSystemLoaded(CharacterInfoSystem system)
    {
        system.OnCharacterUpdate += CharacterUpdated;
        _player.LocalPlayerDetached += CharacterDetached;
    }

    public void OnSystemUnloaded(CharacterInfoSystem system)
    {
        system.OnCharacterUpdate -= CharacterUpdated;
        _player.LocalPlayerDetached -= CharacterDetached;
    }

    private void CharacterUpdated(CharacterData data)
    {
        if (_window == null)
            return;

        var (entity, job, objectives, briefing, entityName) = data;

        _window.SpriteView.SetEntity(entity);
        UpdateRoleType();

        _window.NameLabel.Text = entityName;
        _window.SubText.Text   = job;
        _window.Objectives.RemoveAllChildren();
        _window.ObjectivesLabel.Visible = objectives.Any();

        foreach (var (groupId, conditions) in objectives)
        {
            var objectiveControl = new CharacterObjectiveControl
            {
                Orientation = BoxContainer.LayoutOrientation.Vertical,
                Modulate = Color.Gray
            };

            var objectiveText = new FormattedMessage();
            objectiveText.TryAddMarkup(groupId, out _);

            var objectiveLabel = new RichTextLabel
            {
                StyleClasses = { StyleNano.StyleClassTooltipActionTitle }
            };
            objectiveLabel.SetMessage(objectiveText);
            objectiveControl.AddChild(objectiveLabel);

            foreach (var condition in conditions)
            {
                var conditionControl = new ObjectiveConditionsControl();
                conditionControl.ProgressTexture.Texture = _sprite.Frame0(condition.Icon);
                conditionControl.ProgressTexture.Progress = condition.Progress;
                var titleMessage = new FormattedMessage();
                var descriptionMessage = new FormattedMessage();
                titleMessage.TryAddMarkup(condition.Title, out _);
                descriptionMessage.TryAddMarkup(condition.Description, out _);
                conditionControl.Title.SetMessage(titleMessage);
                conditionControl.Description.SetMessage(descriptionMessage);
                objectiveControl.AddChild(conditionControl);
            }

            _window.Objectives.AddChild(objectiveControl);
        }

        if (briefing != null)
        {
            var briefingControl = new ObjectiveBriefingControl();
            var text = new FormattedMessage();
            text.PushColor(Color.Yellow);
            text.AddText(briefing);
            briefingControl.Label.SetMessage(text);
            _window.Objectives.AddChild(briefingControl);
        }

        var controls = _characterInfo.GetCharacterInfoControls(entity);
        foreach (var control in controls)
            _window.Objectives.AddChild(control);

        // Only show special roles section when there are actual roles/briefing/objectives
        var hasSpecialRoles = briefing != null || controls.Any() || objectives.Any();
        _window.SpecialRolesLabel.Visible = hasSpecialRoles;
        _window.RolePlaceholder.Visible = hasSpecialRoles;  // Show placeholder ONLY when there ARE special roles
        _window.CharacterInfoPanel.Visible = true;  // Always show character info panel

        UpdateCharacterStats(entity);
    }

    // ── Role type ─────────────────────────────────────────────────────────────

    private void OnRoleTypeChanged(MindRoleTypeChangedEvent ev, EntitySessionEventArgs _)
    {
        UpdateRoleType();
    }

    private void UpdateRoleType()
    {
        if (_window == null || !_window.IsOpen)
            return;

        if (!_ent.TryGetComponent<MindContainerComponent>(_player.LocalEntity, out var container)
            || container.Mind is null)
            return;

        if (!_ent.TryGetComponent<MindComponent>(container.Mind.Value, out var mind))
            return;

        if (!_prototypeManager.TryIndex(mind.RoleType, out var proto))
            Log.Error($"Player '{_player.LocalSession}' has invalid Role Type '{mind.RoleType}'.");

        _window.RoleType.Text = Loc.GetString(proto?.Name ?? "role-type-crew-aligned-name");
        _window.RoleType.FontColorOverride = proto?.Color ?? Color.White;
    }

    // ── Window management ─────────────────────────────────────────────────────

    public void UnloadButton()
    {
        if (CharacterButton == null) return;
        CharacterButton.OnPressed -= CharacterButtonPressed;
    }

    public void LoadButton()
    {
        if (CharacterButton == null) return;
        CharacterButton.OnPressed += CharacterButtonPressed;
    }

    private void DeactivateButton()
    {
        if (CharacterButton != null) CharacterButton.Pressed = false;
    }

    private void ActivateButton()
    {
        if (CharacterButton != null) CharacterButton.Pressed = true;
    }

    private void CharacterDetached(EntityUid uid) => CloseWindow();

    private void CharacterButtonPressed(ButtonEventArgs args) => ToggleWindow();

    private void CloseWindow() => _window?.Close();

    private void ToggleWindow()
    {
        if (_window == null) return;
        CharacterButton?.SetClickPressed(!_window.IsOpen);
        if (_window.IsOpen)
            CloseWindow();
        else
        {
            _characterInfo.RequestCharacterInfo();
            _window.Open();
        }
    }
}
