using Content.Shared.Actions;

namespace Content.Shared._OE14.NightVision;

public abstract class OE14SharedNightVisionSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14NightVisionComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<OE14NightVisionComponent, ComponentRemove>(OnRemove);
    }

    private void OnMapInit(Entity<OE14NightVisionComponent> ent, ref MapInitEvent args)
    {
        _actions.AddAction(ent, ref ent.Comp.ActionEntity, ent.Comp.ActionPrototype);
    }

    protected virtual void OnRemove(Entity<OE14NightVisionComponent> ent, ref ComponentRemove args)
    {
        _actions.RemoveAction(ent.Owner, ent.Comp.ActionEntity);
    }
}

public sealed partial class OE14ToggleNightVisionEvent : InstantActionEvent { }
