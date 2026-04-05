using Content.Shared.Construction;
using Content.Shared.Construction.Conditions;
using Content.Shared.Tag;
using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Shared._OE14.Wallmount;

[UsedImplicitly]
[DataDefinition]
public sealed partial class OE14WallRequired : IConstructionCondition
{
    public ConstructionGuideEntry GenerateGuideEntry()
    {
        return new ConstructionGuideEntry
        {
            Localization = "oe14-construction-step-condition-wall-required",
        };
    }

    public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
    {
        var entityManager = IoCManager.Resolve<IEntityManager>();
        var mapSystem = entityManager.System<SharedMapSystem>();
        var transformSystem = entityManager.System<SharedTransformSystem>();
        var tagSystem = entityManager.System<TagSystem>();
        var grid = transformSystem.GetGrid(user);

        if (grid == null || !entityManager.TryGetComponent<MapGridComponent>(grid, out var gridComp))
            return false;

        var offset = direction.ToAngle().ToWorldVec();
        var targetPos = location.Offset(-offset);
        var anchored = mapSystem.GetAnchoredEntities(grid.Value, gridComp, targetPos);

        bool hasParent = false;
        foreach (var entt in anchored)
        {
            if (!tagSystem.HasAnyTag(entt, OE14WallmountSystem.WallTags))
                continue;

            return true;
        }

        return false;
    }
}
