using Content.Shared._OE14.ZLevel;
using Content.Shared.Actions;
using Robust.Shared.Map;

namespace Content.Server._OE14.ZLevels.EntitySystems;

public sealed partial class OE14StationZLevelsSystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    private void InitActions()
    {
        SubscribeLocalEvent<OE14ZLevelMoverComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<OE14ZLevelMoverComponent, ComponentRemove>(OnRemove);
        SubscribeLocalEvent<OE14ZLevelMoverComponent, OE14ZLevelActionUp>(OnZLevelUpGhost);
        SubscribeLocalEvent<OE14ZLevelMoverComponent, OE14ZLevelActionDown>(OnZLevelDownGhost);
    }

    private void OnMapInit(Entity<OE14ZLevelMoverComponent> ent, ref MapInitEvent args)
    {
        _actions.AddAction(ent, ref ent.Comp.OE14ZLevelUpActionEntity, ent.Comp.UpActionProto);
        _actions.AddAction(ent, ref ent.Comp.OE14ZLevelDownActionEntity, ent.Comp.DownActionProto);
    }

    private void OnRemove(Entity<OE14ZLevelMoverComponent> ent, ref ComponentRemove args)
    {
        _actions.RemoveAction(ent.Comp.OE14ZLevelUpActionEntity);
        _actions.RemoveAction(ent.Comp.OE14ZLevelDownActionEntity);
    }

    private void OnZLevelDownGhost(Entity<OE14ZLevelMoverComponent> ent, ref OE14ZLevelActionDown args)
    {
        if (args.Handled)
            return;

        ZLevelMove(ent, -1);

        args.Handled = true;
    }

    private void OnZLevelUpGhost(Entity<OE14ZLevelMoverComponent> ent, ref OE14ZLevelActionUp args)
    {
        if (args.Handled)
            return;

        ZLevelMove(ent, 1);

        args.Handled = true;
    }

    private void ZLevelMove(EntityUid ent, int offset)
    {
        var xform = Transform(ent);
        var map = xform.MapUid;

        if (map is null)
            return;

        var targetMap = GetMapOffset(map.Value, offset);

        if (targetMap is null)
            return;

        _transform.SetMapCoordinates(ent, new MapCoordinates(_transform.GetWorldPosition(ent), targetMap.Value));
    }
}
