using Content.Shared.Mobs;
using Robust.Shared.GameStates;

namespace Content.Shared._OE14.Actions.Components;

/// <summary>
/// Allows you to limit the use of a spell based on the target's alive/dead status
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class OE14ActionTargetMobStatusRequiredComponent : Component
{
    [DataField]
    public HashSet<MobState> AllowedStates = new() { MobState.Alive };
}
