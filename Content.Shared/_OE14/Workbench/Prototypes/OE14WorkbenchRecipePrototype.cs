/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Shared._OE14.Skill.Prototypes;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Workbench.Prototypes;

[Prototype("OE14Recipe")]
public sealed class OE14WorkbenchRecipePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public ProtoId<TagPrototype> Tag;

    [DataField]
    public TimeSpan CraftTime = TimeSpan.FromSeconds(1f);

    [DataField]
    public SoundSpecifier? OverrideCraftSound;

    /// <summary>
    /// Mandatory conditions, without which the craft button will not even be active
    /// </summary>
    [DataField(required: true)]
    public List<OE14WorkbenchCraftRequirement> Requirements = new();

    /// <summary>
    /// Mandatory conditions for completion, but not blocking the craft button.
    /// Players must monitor compliance themselves.
    /// If the conditions are not met, negative effects occur.
    /// </summary>
    [DataField]
    public List<OE14WorkbenchCraftCondition> Conditions = new();

    /// <summary>
    /// What skills do you need to know to see this recipe in the interface?
    /// </summary>
    [DataField]
    public HashSet<ProtoId<OE14SkillPrototype>> RequiredSkills = new();

    [DataField(required: true)]
    public EntProtoId Result;

    [DataField]
    public int ResultCount = 1;

    [DataField]
    public ProtoId<OE14WorkbenchRecipeCategoryPrototype>? Category;

    [DataField]
    public int Priority = 0;  // In descending order. More means it will be first.
}
