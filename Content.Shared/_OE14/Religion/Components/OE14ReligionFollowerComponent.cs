using Content.Shared._OE14.Religion.Prototypes;
using Content.Shared._OE14.Religion.Systems;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Religion.Components;

/// <summary>
/// Determines whether the entity is a follower of God, or may never be able to become one
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(OE14SharedReligionGodSystem))]
public sealed partial class OE14ReligionFollowerComponent : Component
{
    [DataField, AutoNetworkedField]
    public ProtoId<OE14ReligionPrototype>? Religion;

    [DataField, AutoNetworkedField]
    public HashSet<ProtoId<OE14ReligionPrototype>> RejectedReligions = new();

    [DataField]
    public EntProtoId RenounceActionProto = "OE14ActionRenounceFromGod";

    [DataField]
    public EntProtoId AppealToGofProto = "OE14ActionAppealToGod";

    [DataField]
    public EntityUid? RenounceAction;

    [DataField]
    public EntityUid? AppealAction;

    /// <summary>
    /// how much energy does the entity transfer to its god
    /// </summary>
    [DataField]
    public FixedPoint2 EnergyToGodTransfer = 0.5f;

    /// <summary>
    /// how often will the entity transfer mana to its patreon
    /// </summary>
    [DataField]
    public float ManaTransferDelay = 3f;

    /// <summary>
    /// the time of the next magic energy change
    /// </summary>
    [DataField]
    public TimeSpan NextUpdateTime { get; set; } = TimeSpan.Zero;
}
