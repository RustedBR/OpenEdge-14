using Content.Shared._OE14.Vampire;
using Robust.Client.GameObjects;
using Robust.Shared.Timing;

namespace Content.Client._OE14.Vampire;

public sealed class OE14ClientVampireSystem : OE14SharedVampireSystem
{
    [Dependency] private readonly SpriteSystem _sprite = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    protected override void OnVampireVisualsInit(Entity<OE14VampireVisualsComponent> vampire, ref ComponentInit args)
    {
        base.OnVampireVisualsInit(vampire, ref args);

        if (!EntityManager.TryGetComponent(vampire, out SpriteComponent? sprite))
            return;

        if (_sprite.LayerMapTryGet(vampire.Owner, vampire.Comp.FangsMap, out var fangsLayerIndex, false))
            _sprite.LayerSetVisible(vampire.Owner, fangsLayerIndex, true);

        if (_timing.IsFirstTimePredicted)
            SpawnAtPosition(vampire.Comp.EnableVFX, Transform(vampire).Coordinates);
    }

    protected override void OnVampireVisualsShutdown(Entity<OE14VampireVisualsComponent> vampire, ref ComponentShutdown args)
    {
        base.OnVampireVisualsShutdown(vampire, ref args);

        if (!EntityManager.TryGetComponent(vampire, out SpriteComponent? sprite))
            return;

        if (_sprite.LayerMapTryGet(vampire.Owner, vampire.Comp.FangsMap, out var fangsLayerIndex, false))
            _sprite.LayerSetVisible(vampire.Owner, fangsLayerIndex, false);

        if (_timing.IsFirstTimePredicted)
            SpawnAtPosition(vampire.Comp.DisableVFX, Transform(vampire).Coordinates);
    }
}
