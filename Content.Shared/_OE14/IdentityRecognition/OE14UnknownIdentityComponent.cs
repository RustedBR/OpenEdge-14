using Robust.Shared.GameStates;

namespace Content.Shared._OE14.IdentityRecognition;

/// <summary>
/// defines this character's name as unknown.
/// The name can be memorized via KnownNamesComponent,
/// and is hidden when IdentityBlocker is enabled.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class OE14UnknownIdentityComponent : Component
{
}
