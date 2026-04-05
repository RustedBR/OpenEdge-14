using Content.Shared._OE14.CharacterStats;
using Content.Shared._OE14.CharacterStats.Systems;
using Content.Shared._OE14.Skill.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Skill.Effects;

/// <summary>
/// Skill effect that grants +1 to a character stat modifier.
///
/// If the stat is already at MaxStatValue (10), the modifier is still tracked
/// but a free AvailablePoint is refunded to the player so it isn't wasted.
/// Removing the skill reclaims that point if one was given.
///
/// Use this for skills that increase INT (mana) or DEX (stamina) via the stats system,
/// so that the stats system handles all mana/stamina threshold recalculations automatically.
/// </summary>
public sealed partial class OE14AddStatBonus : OE14SkillEffect
{
    /// <summary>
    /// Which stat to boost: "strength", "vitality", "dexterity", or "intelligence".
    /// </summary>
    [DataField(required: true)]
    public string Stat = string.Empty;

    public override void AddSkill(IEntityManager entManager, EntityUid target)
    {
        var statsSystem = entManager.System<OE14SharedCharacterStatsSystem>();
        statsSystem.AddStatModifier(target, Stat, +1, ModifierSource.Spell);
    }

    public override void RemoveSkill(IEntityManager entManager, EntityUid target)
    {
        var statsSystem = entManager.System<OE14SharedCharacterStatsSystem>();
        statsSystem.AddStatModifier(target, Stat, -1, ModifierSource.Spell);
    }

    public override string? GetName(IEntityManager entManager, IPrototypeManager protoManager)
    {
        return null;
    }

    public override string? GetDescription(IEntityManager entManager, IPrototypeManager protoManager, ProtoId<OE14SkillPrototype> skill)
    {
        return Loc.GetString("oe14-skill-desc-add-stat-bonus", ("stat", Stat));
    }
}
