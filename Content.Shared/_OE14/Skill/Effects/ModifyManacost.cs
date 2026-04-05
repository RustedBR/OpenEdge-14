using System.Text;
using Content.Shared._OE14.MagicManacostModify;
using Content.Shared._OE14.MagicRitual.Prototypes;
using Content.Shared._OE14.Skill.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Skill.Effects;

public sealed partial class ModifyManacost : OE14SkillEffect
{
    [DataField]
    public FixedPoint2 Global = 0f;

    public override void AddSkill(IEntityManager entManager, EntityUid target)
    {
        entManager.EnsureComponent<OE14MagicManacostModifyComponent>(target, out var magicEffectManaCost);

        magicEffectManaCost.GlobalModifier += Global;
    }

    public override void RemoveSkill(IEntityManager entManager, EntityUid target)
    {
        entManager.EnsureComponent<OE14MagicManacostModifyComponent>(target, out var magicEffectManaCost);

        magicEffectManaCost.GlobalModifier -= Global;
    }

    public override string? GetName(IEntityManager entMagager, IPrototypeManager protoManager)
    {
        return null;
    }

    public override string? GetDescription(IEntityManager entMagager, IPrototypeManager protoManager, ProtoId<OE14SkillPrototype> skill)
    {
        var sb = new StringBuilder();
        sb.Append(Loc.GetString("oe14-clothing-magic-examine")+"\n");

        if (Global != 0)
        {

            var plus = (float)Global > 0 ? "+" : "";
            sb.Append(
                $"{Loc.GetString("oe14-clothing-magic-global")}: {plus}{MathF.Round((float)Global * 100, MidpointRounding.AwayFromZero)}%\n");
        }

        return sb.ToString();
    }
}
