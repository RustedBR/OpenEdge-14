using Robust.Shared.GameStates;

namespace Content.Shared._OE14.ModularCraft.Components;

/// <summary>
/// Grants stat bonuses (STR/VIT/DEX/INT) when the item is equipped.
/// Applied by modular crafting parts and other items.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class OE14StatBonusComponent : Component
{
    /// <summary>
    /// Stat bonuses granted when item is equipped.
    /// Keys: "strength", "vitality", "dexterity", "intelligence"
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<string, int> StatBonuses = new()
    {
        ["strength"] = 0,
        ["vitality"] = 0,
        ["dexterity"] = 0,
        ["intelligence"] = 0
    };
}
