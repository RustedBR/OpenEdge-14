
using Content.Shared._OE14.Skill.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Skill.Effects;

public sealed partial class AddComponents : OE14SkillEffect
{
    [DataField(required: true)]
    public ComponentRegistry Components = new();

    public override void AddSkill(IEntityManager entManager, EntityUid target)
    {
        entManager.AddComponents(target, Components);
    }

    public override void RemoveSkill(IEntityManager entManager, EntityUid target)
    {
        entManager.RemoveComponents(target, Components);
    }

    public override string? GetName(IEntityManager entMagager, IPrototypeManager protoManager)
    {
        return null;
    }

    public override string? GetDescription(IEntityManager entMagager, IPrototypeManager protoManager, ProtoId<OE14SkillPrototype> skill)
    {
        return null;
    }
}
