using Content.Shared._OE14.Religion.Prototypes;
using Content.Shared._OE14.Religion.Systems;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Religion.Components;

/// <summary>
///
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(OE14SharedReligionGodSystem))]
public sealed partial class OE14ReligionEntityComponent : Component
{
    [DataField(required: true)]
    public ProtoId<OE14ReligionPrototype>? Religion;

    public HashSet<EntityUid> PvsOverridedObservers = new();
    public ICommonSession? Session;

    /// <summary>
    /// Number of followers as a percentage. Automatically calculated on the server and sent to the client for data synchronization.
    /// </summary>
    [DataField, AutoNetworkedField]
    public FixedPoint2 FollowerPercentage = 0;
}
