using Content.Shared._OE14.Skill.Prototypes;
using Content.Shared.Body.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Vampire.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState]
[Access(typeof(OE14SharedVampireSystem))]
public sealed partial class OE14VampireComponent : Component
{
    [DataField]
    public ProtoId<ReagentPrototype> NewBloodReagent = "OE14BloodVampire";
    [DataField]
    public ProtoId<OE14SkillTreePrototype> SkillTreeProto = "Vampire";

    [DataField]
    public ProtoId<MetabolizerTypePrototype> MetabolizerType = "OE14Vampire";

    [DataField]
    public ProtoId<OE14SkillPointPrototype> SkillPointProto = "Blood";

    [DataField(required: true), AutoNetworkedField]
    public ProtoId<OE14VampireFactionPrototype>? Faction;

    [DataField]
    public FixedPoint2 SkillPointCount = 2f;

    [DataField]
    public TimeSpan ToggleVisualsTime = TimeSpan.FromSeconds(2f);

    /// <summary>
    /// All this actions was granted to vampires on component added
    /// </summary>
    [DataField]
    public List<EntProtoId> ActionsProto = new() { "OE14ActionVampireToggleVisuals" };

    /// <summary>
    /// For tracking granted actions, and removing them when component is removed.
    /// </summary>
    [DataField]
    public List<EntityUid> Actions = new();

    [DataField]
    public float HeatUnderSunTemperature = 12000f;

    [DataField]
    public TimeSpan HeatFrequency = TimeSpan.FromSeconds(1);

    [DataField]
    public TimeSpan NextHeatTime = TimeSpan.Zero;

    [DataField]
    public float IgniteThreshold = 350f;
}
