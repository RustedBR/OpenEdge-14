using Content.Shared._OE14.Religion.Components;
using Content.Shared._OE14.Skill.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Skill.Restrictions;

public sealed partial class GodFollowerPercentage : OE14SkillRestriction
{
    [DataField]
    public FixedPoint2 Percentage = 0.5f;
    public override bool Check(IEntityManager entManager, EntityUid target)
    {
        if (!entManager.TryGetComponent<OE14ReligionEntityComponent>(target, out var god))
            return false;

        if (god.Religion is null)
            return false;

        return god.FollowerPercentage >= Percentage;
    }

    public override string GetDescription(IEntityManager entManager, IPrototypeManager protoManager)
    {
        return Loc.GetString("oe14-skill-req-god-follower-percentage", ("count", Percentage * 100));
    }
}
