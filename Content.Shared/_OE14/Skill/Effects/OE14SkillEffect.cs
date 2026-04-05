using Content.Shared._OE14.Skill.Prototypes;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Skill.Effects;

[ImplicitDataDefinitionForInheritors]
[MeansImplicitUse]
public abstract partial class OE14SkillEffect
{
    public abstract void AddSkill(IEntityManager entManager, EntityUid target);

    public abstract void RemoveSkill(IEntityManager entManager, EntityUid target);

    public abstract string? GetName(IEntityManager entMagager, IPrototypeManager protoManager);

    public abstract string? GetDescription(IEntityManager entMagager, IPrototypeManager protoManager, ProtoId<OE14SkillPrototype> skill);
}
