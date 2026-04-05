using Content.Shared._OE14.Skill.Components;
using Content.Shared._OE14.Skill.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Skill.Restrictions;

public sealed partial class NeedPrerequisite : OE14SkillRestriction
{
    [DataField(required: true)]
    public ProtoId<OE14SkillPrototype> Prerequisite = new();

    public override bool Check(IEntityManager entManager, EntityUid target)
    {
        if (!entManager.TryGetComponent<OE14SkillStorageComponent>(target, out var skillStorage))
            return false;

        var learned = skillStorage.LearnedSkills;
        return learned.Contains(Prerequisite);
    }

    public override string GetDescription(IEntityManager entManager, IPrototypeManager protoManager)
    {
        var skillSystem = entManager.System<OE14SharedSkillSystem>();

        return Loc.GetString("oe14-skill-req-prerequisite", ("name", skillSystem.GetSkillName(Prerequisite)));
    }
}
