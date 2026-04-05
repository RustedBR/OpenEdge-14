using Content.Server._OE14.Objectives.Systems;
using Content.Shared._OE14.Vampire;
using Content.Shared.StatusIcon;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server._OE14.Objectives.Components;

[RegisterComponent, Access(typeof(OE14VampireObjectiveConditionsSystem))]
public sealed partial class OE14VampireBloodPurityConditionComponent : Component
{
    [DataField]
    public ProtoId<OE14VampireFactionPrototype>? Faction;

    [DataField]
    public SpriteSpecifier Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/_OE14/Actions/Spells/vampire.rsi"), "blood_moon");
}
