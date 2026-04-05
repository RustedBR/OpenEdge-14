using Robust.Shared.GameStates;

namespace Content.Shared._OE14.Damageable;

/// <summary>
/// Increases or decreases incoming damage, regardless of the damage type.
/// Unlike standard Damageable modifiers, this value can be changed during the game.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class OE14DamageableModifierComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Modifier = 1f;
}
