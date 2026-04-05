/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using System.Linq;
using System.Numerics;
using Content.Server.Nutrition.Components;
using Content.Server.Temperature.Systems;
using Content.Shared._OE14.Cooking;
using Content.Shared._OE14.Cooking.Components;
using Content.Shared._OE14.Temperature;
using Content.Shared.Chemistry.EntitySystems;
using Robust.Shared.Random;

namespace Content.Server._OE14.Cooking;

public sealed class OE14CookingSystem : OE14SharedCookingSystem
{
    [Dependency] private readonly TemperatureSystem _temperature = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14FoodHolderComponent, SolutionContainerChangedEvent>(OnHolderChanged);
    }

    protected override bool TryTransferFood(Entity<OE14FoodHolderComponent> target, Entity<OE14FoodHolderComponent> source)
    {
        if (base.TryTransferFood(target, source))
        {
            //Sliceable
            if (source.Comp.FoodData?.SliceProto is not null)
            {
                var sliceable = EnsureComp<SliceableFoodComponent>(target);
                sliceable.Slice = source.Comp.FoodData.SliceProto;
                sliceable.TotalCount = source.Comp.FoodData.SliceCount;
            }
        }

        return true;
    }

    private void OnHolderChanged(Entity<OE14FoodHolderComponent> ent, ref SolutionContainerChangedEvent args)
    {
        if (args.Solution.Volume != 0)
            return;

        ent.Comp.FoodData = null;
        Dirty(ent);
    }

    protected override void OnCookBurned(Entity<OE14FoodCookerComponent> ent, ref OE14BurningDoAfter args)
    {
        if (args.Cancelled || args.Handled)
            return;

        base.OnCookBurned(ent, ref args);

        if (_random.Prob(ent.Comp.BurntAdditionalSpawnProb))
            Spawn(ent.Comp.BurntAdditionalSpawn, Transform(ent).Coordinates);
    }

    protected override void UpdateFoodDataVisuals(Entity<OE14FoodHolderComponent> ent, OE14FoodData data, bool rename = true)
    {
        base.UpdateFoodDataVisuals(ent, data, rename);

        if (ent.Comp.FoodData?.SliceProto is null)
            return;

        if (!TryComp<SliceableFoodComponent>(ent, out var sliceable))
            return;

        sliceable.Slice = ent.Comp.FoodData.SliceProto;
        sliceable.TotalCount = ent.Comp.FoodData.SliceCount;
    }

    protected override void OnCookFinished(Entity<OE14FoodCookerComponent> ent, ref OE14CookingDoAfter args)
    {
        if (args.Cancelled || args.Handled)
            return;

        //We need transform all BEFORE Shared cooking code
        TryTransformAll(ent);

        base.OnCookFinished(ent, ref args);
    }

    private void TryTransformAll(Entity<OE14FoodCookerComponent> ent)
    {
        if (!_container.TryGetContainer(ent, ent.Comp.ContainerId, out var container))
            return;

        var containedEntities = container.ContainedEntities.ToList();

        foreach (var contained in containedEntities)
        {
            if (!TryComp<OE14TemperatureTransformationComponent>(contained, out var transformable))
                continue;

            if (!transformable.AutoTransformOnCooked)
                continue;

            if (transformable.Entries.Count == 0)
                continue;

            var entry = transformable.Entries[0];

            var newTemp = (entry.TemperatureRange.X + entry.TemperatureRange.Y) / 2;
            _temperature.ForceChangeTemperature(contained, newTemp);
        }
    }
}
