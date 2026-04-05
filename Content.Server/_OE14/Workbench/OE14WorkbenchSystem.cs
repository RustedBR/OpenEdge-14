/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using System.Numerics;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared._OE14.Skill;
using Content.Shared._OE14.Workbench;
using Content.Shared._OE14.Workbench.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.Placeable;
using Content.Shared.UserInterface;
using Robust.Server.Audio;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._OE14.Workbench;

public sealed partial class OE14WorkbenchSystem : OE14SharedWorkbenchSystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly DoAfterSystem _doAfter = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly UserInterfaceSystem _userInterface = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        InitProviders();

        SubscribeLocalEvent<OE14WorkbenchComponent, MapInitEvent>(OnMapInit);

        SubscribeLocalEvent<OE14WorkbenchComponent, ItemPlacedEvent>(OnItemPlaced);
        SubscribeLocalEvent<OE14WorkbenchComponent, ItemRemovedEvent>(OnItemRemoved);

        SubscribeLocalEvent<OE14WorkbenchComponent, BeforeActivatableUIOpenEvent>(OnBeforeUIOpen);
        SubscribeLocalEvent<OE14WorkbenchComponent, OE14WorkbenchUiCraftMessage>(OnCraft);

        SubscribeLocalEvent<OE14WorkbenchComponent, OE14CraftDoAfterEvent>(OnCraftFinished);
    }

    private void OnMapInit(Entity<OE14WorkbenchComponent> ent, ref MapInitEvent args)
    {
        foreach (var recipe in _proto.EnumeratePrototypes<OE14WorkbenchRecipePrototype>())
        {
            if (ent.Comp.Recipes.Contains(recipe))
                continue;

            if (!ent.Comp.RecipeTags.Contains(recipe.Tag))
                continue;

            ent.Comp.Recipes.Add(recipe);
        }
    }

    private void OnItemRemoved(Entity<OE14WorkbenchComponent> ent, ref ItemRemovedEvent args)
    {
        UpdateUIRecipes(ent);
    }

    private void OnItemPlaced(Entity<OE14WorkbenchComponent> ent, ref ItemPlacedEvent args)
    {
        UpdateUIRecipes(ent);
    }

    private void OnBeforeUIOpen(Entity<OE14WorkbenchComponent> ent, ref BeforeActivatableUIOpenEvent args)
    {
        UpdateUIRecipes(ent);
    }

    private void OnCraftFinished(Entity<OE14WorkbenchComponent> ent, ref OE14CraftDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        if (!_proto.TryIndex(args.Recipe, out var recipe))
            return;

        var getResource = new OE14WorkbenchGetResourcesEvent();
        RaiseLocalEvent(ent.Owner, getResource);

        var resources = getResource.Resources;

        if (!CanCraftRecipe(recipe, resources, args.User))
        {
            _popup.PopupEntity(Loc.GetString("oe14-workbench-cant-craft"), ent, args.User);
            return;
        }

        //Check conditions
        var passConditions = true;
        foreach (var condition in recipe.Conditions)
        {
            if (!condition.CheckCondition(EntityManager, _proto, ent, args.User))
            {
                condition.FailedEffect(EntityManager, _proto, ent, args.User);
                passConditions = false;
            }
            condition.PostCraft(EntityManager, _proto, ent, args.User);
        }

        foreach (var req in recipe.Requirements)
        {
            req.PostCraft(EntityManager, _proto, resources);
        }

        if (passConditions)
        {
            var resultEntities = new HashSet<EntityUid>();
            for (var i = 0; i < recipe.ResultCount; i++)
            {
                var resultEntity = Spawn(recipe.Result);
                resultEntities.Add(resultEntity);
            }

            //We teleport result to workbench AFTER craft.
            foreach (var resultEntity in resultEntities)
            {
                _transform.SetCoordinates(resultEntity, Transform(ent).Coordinates.Offset(new Vector2(_random.NextFloat(-0.25f, 0.25f), _random.NextFloat(-0.25f, 0.25f))));
            }
        }

        UpdateUIRecipes(ent);
        args.Handled = true;
    }

    private void StartCraft(Entity<OE14WorkbenchComponent> workbench,
        EntityUid user,
        OE14WorkbenchRecipePrototype recipe)
    {
        var craftDoAfter = new OE14CraftDoAfterEvent
        {
            Recipe = recipe.ID,
        };

        var doAfterArgs = new DoAfterArgs(EntityManager,
            user,
            recipe.CraftTime * workbench.Comp.CraftSpeed,
            craftDoAfter,
            workbench,
            workbench)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            NeedHand = true,
        };

        _doAfter.TryStartDoAfter(doAfterArgs);
        _audio.PlayPvs(recipe.OverrideCraftSound ?? workbench.Comp.CraftSound, workbench);
    }

    private bool CanCraftRecipe(OE14WorkbenchRecipePrototype recipe, HashSet<EntityUid> entities, EntityUid user)
    {
        foreach (var skill in recipe.RequiredSkills)
        {
            if (!_skill.HaveSkill(user, skill))
                return false;
        }
        foreach (var req in recipe.Requirements)
        {
            if (!req.CheckRequirement(EntityManager, _proto, entities))
                return false;
        }

        return true;
    }
}
