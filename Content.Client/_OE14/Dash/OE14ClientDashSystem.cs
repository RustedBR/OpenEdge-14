using Content.Shared._OE14.Dash;

namespace Content.Client._OE14.Dash;

public sealed partial class OE14ClientDashSystem : EntitySystem
{
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<OE14DashComponent>();
        while (query.MoveNext(out var uid, out var dash))
        {
            SpawnAtPosition(dash.DashEffect, Transform(uid).Coordinates);
        }
    }
}
