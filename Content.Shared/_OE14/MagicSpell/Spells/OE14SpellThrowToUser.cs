using Content.Shared.Projectiles;
using Content.Shared.Throwing;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellThrowToUser : OE14SpellEffect
{
    [DataField]
    public float ThrowPower = 10f;
    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        var targetEntity = args.Target.Value;

        var throwing = entManager.System<ThrowingSystem>();

        if (!entManager.TryGetComponent<TransformComponent>(args.User, out var xform))
            return;

        if (entManager.TryGetComponent<EmbeddableProjectileComponent>(targetEntity, out var embeddable))
        {
            var projectile = entManager.System<SharedProjectileSystem>();

            projectile.EmbedDetach(targetEntity, embeddable);
        }

        throwing.TryThrow(targetEntity, xform.Coordinates, ThrowPower);
    }
}
