using Content.Shared._OE14.Religion.Prototypes;
using Content.Shared._OE14.Religion.Systems;
using Content.Shared.Alert;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Religion.Components;

/// <summary>
/// This entity has not yet become a follower of God, but wants to become one. Confirmation from god is expected
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(OE14SharedReligionGodSystem))]
public sealed partial class OE14ReligionPendingFollowerComponent : Component
{
    [DataField, AutoNetworkedField]
    public ProtoId<OE14ReligionPrototype>? Religion;
}
