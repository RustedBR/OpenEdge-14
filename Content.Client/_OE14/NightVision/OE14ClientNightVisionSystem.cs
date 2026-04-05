using Content.Shared._OE14.NightVision;
using Robust.Client.Player;
using Robust.Shared.Player;

namespace Content.Client._OE14.NightVision;

public sealed class OE14ClientNightVisionSystem : OE14SharedNightVisionSystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14NightVisionComponent, OE14ToggleNightVisionEvent>(OnToggleNightVision);
        SubscribeLocalEvent<OE14NightVisionComponent, PlayerDetachedEvent>(OnPlayerDetached);
    }

    protected override void OnRemove(Entity<OE14NightVisionComponent> ent, ref ComponentRemove args)
    {
        base.OnRemove(ent, ref args);

        NightVisionOff(ent);
    }

    private void OnPlayerDetached(Entity<OE14NightVisionComponent> ent, ref PlayerDetachedEvent args)
    {
        NightVisionOff(ent);
    }

    private void OnToggleNightVision(Entity<OE14NightVisionComponent> ent, ref OE14ToggleNightVisionEvent args)
    {
        NightVisionToggle(ent);
    }

    private void NightVisionOn(Entity<OE14NightVisionComponent> ent)
    {
        if (_playerManager.LocalSession?.AttachedEntity != ent)
            return;

        var nightVisionLight = Spawn(ent.Comp.LightPrototype, Transform(ent).Coordinates);
        _transform.SetParent(nightVisionLight, ent);
        _transform.SetWorldRotation(nightVisionLight, _transform.GetWorldRotation(ent));
        ent.Comp.LocalLightEntity = nightVisionLight;
    }

    private void NightVisionOff(Entity<OE14NightVisionComponent> ent)
    {
        QueueDel(ent.Comp.LocalLightEntity);
        ent.Comp.LocalLightEntity = null;
    }

    private void NightVisionToggle(Entity<OE14NightVisionComponent> ent)
    {
        if (ent.Comp.LocalLightEntity == null)
        {
            NightVisionOn(ent);
        }
        else
        {
            NightVisionOff(ent);
        }
    }
}
