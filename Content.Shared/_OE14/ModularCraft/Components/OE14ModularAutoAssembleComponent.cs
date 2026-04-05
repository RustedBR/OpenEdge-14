using Content.Shared._OE14.ModularCraft.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.ModularCraft.Components;

/// <summary>
/// Adds all details to the item when initializing. This is useful for spawning modular items directly when mapping or as loot in dungeons
/// </summary>
[RegisterComponent, Access(typeof(OE14SharedModularCraftSystem))]
public sealed partial class OE14ModularCraftAutoAssembleComponent : Component
{
    [DataField]
    public List<EntProtoId> Details = new();
}
