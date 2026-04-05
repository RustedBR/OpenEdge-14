using Content.Server._OE14.Objectives.Systems;
using Robust.Shared.Utility;

namespace Content.Server._OE14.Objectives.Components;

[RegisterComponent, Access(typeof(OE14VampireObjectiveConditionsSystem))]
public sealed partial class OE14VampireDefenceVillageConditionComponent : Component
{
    [DataField]
    public SpriteSpecifier Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/_OE14/Actions/Spells/vampire.rsi"), "essence_create");
}
