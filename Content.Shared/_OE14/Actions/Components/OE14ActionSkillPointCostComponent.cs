using Content.Shared._OE14.Skill.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Actions.Components;

/// <summary>
/// Restricts the use of this action, by spending user skillpoints
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class OE14ActionSkillPointCostComponent : Component
{
    [DataField(required: true)]
    public ProtoId<OE14SkillPointPrototype>? SkillPoint;

    [DataField]
    public FixedPoint2 Count = 1f;
}
