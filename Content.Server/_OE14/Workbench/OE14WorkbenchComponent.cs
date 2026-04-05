/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Shared._OE14.Workbench.Prototypes;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.Workbench;

/// <summary>
/// This entity can be used to craft other objects through the interface
/// </summary>
[RegisterComponent]
[Access(typeof(OE14WorkbenchSystem))]
public sealed partial class OE14WorkbenchComponent : Component
{
    /// <summary>
    /// Crafting speed modifier on this workbench.
    /// </summary>
    [DataField]
    public float CraftSpeed = 1f;

    [DataField]
    public float WorkbenchRadius = 1.5f;

    /// <summary>
    /// List of recipes available for crafting on this type of workbench
    /// </summary>
    [DataField]
    public List<ProtoId<OE14WorkbenchRecipePrototype>> Recipes = new();

    /// <summary>
    /// Auto recipe list fill based on tags
    /// </summary>
    [DataField]
    public List<ProtoId<TagPrototype>> RecipeTags = new();

    /// <summary>
    /// Played during crafting. Can be overwritten by the crafting sound of a specific recipe.
    /// </summary>
    [DataField]
    public SoundSpecifier CraftSound = new SoundCollectionSpecifier("OE14Hammering");
}
