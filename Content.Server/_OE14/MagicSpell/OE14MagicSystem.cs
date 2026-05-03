using Content.Server.Flash;
using Content.Shared._OE14.MagicSpell;
using Content.Shared._OE14.MagicSpell.Events;
using Content.Shared._OE14.MagicSpell.Spells;
using Content.Shared.Audio;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Physics.Events;
using Robust.Shared.Random;

namespace Content.Server._OE14.MagicSpell;

public sealed  class OE14MagicSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly FlashSystem _flash = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14AreaEntityEffectComponent, MapInitEvent>(OnAoEMapInit);

        SubscribeLocalEvent<OE14SpellEffectOnHitComponent, MeleeHitEvent>(OnMeleeHit);
        SubscribeLocalEvent<OE14SpellEffectOnHitComponent, ThrowDoHitEvent>(OnProjectileHit);
        SubscribeLocalEvent<OE14SpellEffectOnCollideComponent, StartCollideEvent>(OnStartCollide);
        SubscribeLocalEvent<OE14TrapCollideComponent, StartCollideEvent>(OnTrapCollide);
        SubscribeLocalEvent<OE14FlashAreaEffectEvent>(OnFlashAreaEffect);
    }

    private void OnFlashAreaEffect(ref OE14FlashAreaEffectEvent ev)
    {
        _flash.FlashArea(ev.Source, ev.Instigator, ev.Range, ev.Duration, ev.SlowTo);
    }

    private void OnTrapCollide(Entity<OE14TrapCollideComponent> ent, ref StartCollideEvent args)
    {
        if (ent.Comp.Owner == args.OtherEntity)
            return;

        if (ent.Comp.Whitelist is not null && !_whitelist.IsValid(ent.Comp.Whitelist, args.OtherEntity))
            return;

        if (ent.Comp.TriggerSound is not null)
            _audio.PlayPvs(ent.Comp.TriggerSound, ent);

        var triggerCoords = Transform(ent).Coordinates;

        foreach (var effect in ent.Comp.Effects)
        {
            effect.Effect(EntityManager, new OE14SpellEffectBaseArgs(ent.Comp.Owner, ent, args.OtherEntity, triggerCoords));
        }

        QueueDel(ent);
    }

    private void OnStartCollide(Entity<OE14SpellEffectOnCollideComponent> ent, ref StartCollideEvent args)
    {
        if (!_random.Prob(ent.Comp.Prob))
            return;

        if (ent.Comp.Whitelist is not null && !_whitelist.IsValid(ent.Comp.Whitelist, args.OtherEntity))
            return;

        foreach (var effect in ent.Comp.Effects)
        {
            effect.Effect(EntityManager, new OE14SpellEffectBaseArgs(null, ent, args.OtherEntity, Transform(args.OtherEntity).Coordinates));
        }
    }

    private void OnProjectileHit(Entity<OE14SpellEffectOnHitComponent> ent, ref ThrowDoHitEvent args)
    {
        if (!_random.Prob(ent.Comp.Prob))
            return;

        if (ent.Comp.Whitelist is not null && !_whitelist.IsValid(ent.Comp.Whitelist, args.Target))
            return;

        foreach (var effect in ent.Comp.Effects)
        {
            effect.Effect(EntityManager, new OE14SpellEffectBaseArgs(args.Thrown, ent, args.Target, Transform(args.Target).Coordinates));
        }
    }

    private void OnMeleeHit(Entity<OE14SpellEffectOnHitComponent> ent, ref MeleeHitEvent args)
    {
        if (HasComp<PacifiedComponent>(args.User)) //IDK how to check if the user is pacified in a better way
            return;

        if (!args.IsHit)
            return;

        if (!_random.Prob(ent.Comp.Prob))
            return;

        foreach (var entity in args.HitEntities)
        {
            if (ent.Comp.Whitelist is not null && !_whitelist.IsValid(ent.Comp.Whitelist, entity))
                continue;

            foreach (var effect in ent.Comp.Effects)
            {
                effect.Effect(EntityManager, new OE14SpellEffectBaseArgs(args.User, ent, entity, Transform(entity).Coordinates));
            }
        }
    }

    private void OnAoEMapInit(Entity<OE14AreaEntityEffectComponent> ent, ref MapInitEvent args)
    {
        var entitiesAround = _lookup.GetEntitiesInRange(ent, ent.Comp.Range, LookupFlags.Uncontained);

        var count = 0;
        foreach (var entity in entitiesAround)
        {
            if (ent.Comp.Whitelist is not null && !_whitelist.IsValid(ent.Comp.Whitelist, entity))
                continue;

            foreach (var effect in ent.Comp.Effects)
            {
                effect.Effect(EntityManager, new OE14SpellEffectBaseArgs(ent, null, entity, Transform(entity).Coordinates));
            }

            count++;

            if (ent.Comp.MaxTargets > 0 && count >= ent.Comp.MaxTargets)
                break;
        }
    }
}
