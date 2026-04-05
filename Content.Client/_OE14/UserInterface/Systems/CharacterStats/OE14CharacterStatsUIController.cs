using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared._OE14.Input;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Input.Binding;
using Robust.Shared.Timing;

namespace Content.Client._OE14.UserInterface.Systems.CharacterStats;

/// <summary>
/// DEPRECATED: Character stats are now integrated into the CharacterWindow (opened with C key).
/// This controller is kept for reference but is no longer used.
/// See: Content.Client.UserInterface.Systems.Character.CharacterUIController.UpdateCharacterStats()
/// </summary>
[UsedImplicitly]
public sealed class OE14CharacterStatsUIController : UIController
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;

    private OE14CharacterStatsWindow? _window;
    private EntityUid? _playerEntity;

    public override void Initialize()
    {
        // DEPRECATED: Stats are now shown in the Character Menu (C key)
        // Keeping this disabled to avoid conflicts
        return;

        base.Initialize();

        _window = UIManager.CreateWindow<OE14CharacterStatsWindow>();

        CommandBinds.Builder
            .Bind(OE14ContentKeyFunctions.OE14OpenStatsMenu,
                InputCmdHandler.FromDelegate(_ => ToggleWindow()))
            .Register<OE14CharacterStatsUIController>();

        UpdateStatsDisplay();
    }

    private void ToggleWindow()
    {
        if (_window == null)
            return;

        if (_window.IsOpen)
            _window.Close();
        else
            _window.Open();
    }

    private void UpdateStatsDisplay()
    {
        _playerEntity = _playerManager.LocalPlayer?.ControlledEntity;

        if (_playerEntity == null || _window == null || !_entityManager.TryGetComponent<OE14CharacterStatsComponent>(_playerEntity.Value, out var stats))
        {
            return;
        }

        // Update labels
        _window.StrengthValue.Text = stats.Strength.ToString();
        _window.VitalityValue.Text = stats.Vitality.ToString();
        _window.DexterityValue.Text = stats.Dexterity.ToString();
        _window.IntelligenceValue.Text = stats.Intelligence.ToString();

        // Update progress bars
        _window.StrengthBar.Value = stats.Strength;
        _window.VitalityBar.Value = stats.Vitality;
        _window.DexterityBar.Value = stats.Dexterity;
        _window.IntelligenceBar.Value = stats.Intelligence;

        // Update bonuses info
        var dmgMult = stats.DamageMultiplier.ToString("F2");
        var healthB = stats.HealthBonus.ToString("F0");
        var stamMult = stats.StaminaMultiplier.ToString("F2");
        var manaB = stats.ManaBonus.ToString("F0");

        var bonusesText = "[bold]Bonuses from Stats:[/bold]\n" +
                          $"Damage Multiplier: {dmgMult}x\n" +
                          $"Health Bonus: +{healthB} HP\n" +
                          $"Stamina Multiplier: {stamMult}x\n" +
                          $"Mana Bonus: +{manaB} mana";

        _window.BonusesInfo.Text = bonusesText;
    }

    public override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (_playerManager.LocalPlayer?.ControlledEntity != _playerEntity)
        {
            _playerEntity = _playerManager.LocalPlayer?.ControlledEntity;
            UpdateStatsDisplay();
        }
        else if (_playerEntity != null && _entityManager.TryGetComponent<OE14CharacterStatsComponent>(_playerEntity.Value, out var stats))
        {
            UpdateStatsDisplay();
        }
    }
}
