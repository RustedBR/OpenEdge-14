using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Skill.Restrictions;

public sealed partial class Impossible : OE14SkillRestriction
{
    public override bool Check(IEntityManager entManager, EntityUid target)
    {
        return false;
    }

    public override string GetDescription(IEntityManager entManager, IPrototypeManager protoManager)
    {
        return Loc.GetString("oe14-skill-req-impossible");
    }
}
