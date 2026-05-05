using Content.Server._OE14.GameTicking.Rules.Components;
using Content.Server.GameTicking.Rules;
using Content.Shared._OE14.DayCycle;
using Content.Shared.GameTicking.Components;
using Content.Shared.Light.Components;
using Robust.Shared.Map.Components;

namespace Content.Server._OE14.GameTicking.Rules;

public sealed class OE14VampireNightRule : GameRuleSystem<OE14VampireNightRuleComponent>
{
    private float _elapsed;

    protected override void Started(EntityUid uid,
        OE14VampireNightRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);
        _elapsed = 0;

        // Start at midnight (50% of cycle)
        var mapQuery = EntityQueryEnumerator<MapComponent, LightCycleComponent>();
        while (mapQuery.MoveNext(out var mapUid, out _, out var lightCycle))
        {
            lightCycle.Offset = lightCycle.Duration.Multiply(0.5);
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = QueryActiveRules();
        while (query.MoveNext(out var uid, out _, out _, out _))
        {
            _elapsed += frameTime;

            if (_elapsed >= 300) // 5 minutes
            {
                var mapQuery = EntityQueryEnumerator<MapComponent, LightCycleComponent>();
                while (mapQuery.MoveNext(out var mapUid, out _, out _))
                {
                    var ev = new OE14StartNightEvent(mapUid);
                    RaiseLocalEvent(ev);
                }

                ForceEndSelf(uid);
            }
        }
    }
}

public sealed partial class OE14VampireNightRuleComponent : Component;
