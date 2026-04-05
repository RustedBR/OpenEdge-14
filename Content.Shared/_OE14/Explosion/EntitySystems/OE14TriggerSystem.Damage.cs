using Content.Shared._OE14.Explosion.Components;
using Content.Shared.Damage;

namespace Content.Shared.Trigger.Systems;

public sealed partial class TriggerSystem
{
    private void InitializeDamageReceived()
    {
        SubscribeLocalEvent<OE14TriggerOnDamageReceivedComponent, DamageChangedEvent>(OnDamageReceived);
    }

    private void OnDamageReceived(EntityUid uid, OE14TriggerOnDamageReceivedComponent component, DamageChangedEvent args)
    {
        if (!args.DamageIncreased)
            return;

        Trigger(uid);
    }
}
