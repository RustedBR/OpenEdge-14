using Content.Shared._OE14.Skill;
using Content.Shared._OE14.Skill.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellRemoveMemoryPoint : OE14SpellEffect
{
    [DataField]
    public float RemovedPoints = 0.5f;

    [DataField]
    public ProtoId<OE14SkillPointPrototype> SkillPointType = "Memory";

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        var skillSys = entManager.System<OE14SharedSkillSystem>();

        skillSys.RemoveSkillPoints(args.Target.Value, SkillPointType, RemovedPoints);
    }
}
