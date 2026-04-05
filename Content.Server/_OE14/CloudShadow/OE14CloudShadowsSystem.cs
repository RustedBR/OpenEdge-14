using Content.Shared._OE14.CloudShadow;
using Robust.Shared.Random;

namespace Content.Server._OE14.CloudShadow;

public sealed class OE14CloudShadowsSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14CloudShadowsComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<OE14CloudShadowsComponent> entity, ref MapInitEvent args)
    {
        entity.Comp.CloudSpeed = _random.NextVector2(-entity.Comp.MaxSpeed, entity.Comp.MaxSpeed);
    }
}
