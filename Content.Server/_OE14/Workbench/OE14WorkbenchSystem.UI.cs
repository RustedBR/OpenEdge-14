/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Shared._OE14.Skill;
using Content.Shared._OE14.Workbench;
using Content.Shared.Placeable;

namespace Content.Server._OE14.Workbench;

public sealed partial class OE14WorkbenchSystem
{
    [Dependency] private readonly OE14SharedSkillSystem _skill = default!;

    private void OnCraft(Entity<OE14WorkbenchComponent> entity, ref OE14WorkbenchUiCraftMessage args)
    {
        if (!entity.Comp.Recipes.Contains(args.Recipe))
            return;

        if (!_proto.TryIndex(args.Recipe, out var prototype))
            return;

        StartCraft(entity, args.Actor, prototype);
    }

    private void UpdateUIRecipes(Entity<OE14WorkbenchComponent> entity)
    {
        var getResource = new OE14WorkbenchGetResourcesEvent();
        RaiseLocalEvent(entity, getResource);

        var resources = getResource.Resources;

        var recipes = new List<OE14WorkbenchUiRecipesEntry>();
        foreach (var recipeId in entity.Comp.Recipes)
        {
            if (!_proto.TryIndex(recipeId, out var indexedRecipe))
                continue;

            var canCraft = true;

            foreach (var requirement in indexedRecipe.Requirements)
            {
                if (!requirement.CheckRequirement(EntityManager, _proto, resources))
                {
                    canCraft = false;
                    break;
                }
            }

            var entry = new OE14WorkbenchUiRecipesEntry(recipeId, canCraft);

            recipes.Add(entry);
        }

        _userInterface.SetUiState(entity.Owner, OE14WorkbenchUiKey.Key, new OE14WorkbenchUiRecipesState(recipes));
    }
}
