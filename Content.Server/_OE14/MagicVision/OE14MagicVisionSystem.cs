using Content.Shared._OE14.MagicVision;
using Robust.Shared.Timing;
using Content.Shared.StatusEffectNew;

namespace Content.Server._OE14.MagicVision;

public sealed class OE14MagicVisionSystem : OE14SharedMagicVisionSystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedEyeSystem _eye = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14MagicVisionStatusEffectComponent, StatusEffectRelayedEvent<GetVisMaskEvent>>(OnGetVisMask);

        SubscribeLocalEvent<OE14MagicVisionStatusEffectComponent, StatusEffectAppliedEvent>(OnApplied);
        SubscribeLocalEvent<OE14MagicVisionStatusEffectComponent, StatusEffectRemovedEvent>(OnRemoved);
    }

    private void OnGetVisMask(Entity<OE14MagicVisionStatusEffectComponent> ent, ref StatusEffectRelayedEvent<GetVisMaskEvent> args)
    {
        var appliedMask = (int)OE14MagicVisionStatusEffectComponent.VisibilityMask;
        var newArgs = args.Args;

        newArgs.VisibilityMask |= appliedMask;
        args = args with { Args = newArgs };
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<OE14MagicVisionMarkerComponent>();
        while (query.MoveNext(out var uid, out var marker))
        {
            if (marker.EndTime == TimeSpan.Zero)
                continue;

            if (_timing.CurTime < marker.EndTime)
                continue;

            QueueDel(uid);
        }
    }

    private void OnApplied(Entity<OE14MagicVisionStatusEffectComponent> ent, ref StatusEffectAppliedEvent args)
    {
        _eye.RefreshVisibilityMask(args.Target);
    }

    private void OnRemoved(Entity<OE14MagicVisionStatusEffectComponent> ent, ref StatusEffectRemovedEvent args)
    {
        _eye.RefreshVisibilityMask(args.Target);
    }
}
