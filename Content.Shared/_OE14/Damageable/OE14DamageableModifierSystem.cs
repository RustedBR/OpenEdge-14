using Content.Shared.Damage;

namespace Content.Shared._OE14.Damageable;

public sealed class OE14DamageableModifierSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14DamageableModifierComponent, DamageModifyEvent>(OnDamageModify);
    }

    private void OnDamageModify(Entity<OE14DamageableModifierComponent> ent, ref DamageModifyEvent args)
    {
        args.Damage *= ent.Comp.Modifier;
    }
}
