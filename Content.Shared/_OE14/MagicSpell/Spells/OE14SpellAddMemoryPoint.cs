using Content.Shared._OE14.Skill;
using Content.Shared._OE14.Skill.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellAddMemoryPoint : OE14SpellEffect
{
    [DataField]
    public float AddedPoints = 0.5f;

    [DataField]
    public float Limit = 6.5f;

    [DataField]
    public ProtoId<OE14SkillPointPrototype> SkillPointType = "Memory";

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        var skillSys = entManager.System<OE14SharedSkillSystem>();

        skillSys.AddSkillPoints(args.Target.Value, SkillPointType, AddedPoints, Limit);
    }
}
