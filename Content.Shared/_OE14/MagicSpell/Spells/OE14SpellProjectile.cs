using System.Numerics;
using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Map;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellProjectile : OE14SpellEffect
{
    [DataField(required: true)]
    public EntProtoId Prototype;

    [DataField]
    public float ProjectileSpeed = 20f;

    [DataField]
    public float Spread = 0f;

    [DataField]
    public int ProjectileCount = 1;

    [DataField]
    public bool SaveVelocity = false;

    /// <summary>
    /// When true, the spawned projectile's damage is multiplied by the caster's INT.
    /// Formula: INT 5 = 1.0x, INT 10 = 1.5x, INT 1 = 0.5x.
    /// </summary>
    [DataField]
    public bool ScaleWithCasterInt = false;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        EntityCoordinates? targetPoint = null;

        if (args.Target is not null &&
            entManager.TryGetComponent<TransformComponent>(args.Target.Value, out var transformComponent))
            targetPoint = transformComponent.Coordinates;
        else if (args.Position is not null)
            targetPoint = args.Position;

        if (targetPoint is null)
            return;

        var transform = entManager.System<SharedTransformSystem>();
        var physics = entManager.System<SharedPhysicsSystem>();
        var gunSystem = entManager.System<SharedGunSystem>();
        var mapManager = IoCManager.Resolve<IMapManager>();
        var random = IoCManager.Resolve<IRobustRandom>();

        if (!entManager.TryGetComponent<TransformComponent>(args.User, out var xform))
            return;

        var fromCoords = xform.Coordinates;


        var userVelocity = physics.GetMapLinearVelocity(args.User.Value);

        // If applicable, this ensures the projectile is parented to grid on spawn, instead of the map.
        var fromMap = transform.ToMapCoordinates(fromCoords);

        var spawnCoords = mapManager.TryFindGridAt(fromMap, out var gridUid, out _)
            ? transform.WithEntityId(fromCoords, gridUid)
            : new(mapManager.GetMapEntityId(fromMap.MapId), fromMap.Position);

        for (var i = 0; i < ProjectileCount; i++)
        {
            //Apply spread to target point
            var offsetedTargetPoint = targetPoint.Value.Offset(new Vector2(
                (float) (random.NextDouble() * 2 - 1) * Spread,
                (float) (random.NextDouble() * 2 - 1) * Spread));

            if (fromCoords == offsetedTargetPoint)
                continue;

            var ent = entManager.PredictedSpawnAtPosition(Prototype, spawnCoords);

            // Scale projectile damage by caster Intelligence
            if (ScaleWithCasterInt &&
                args.User is not null &&
                entManager.TryGetComponent<OE14CharacterStatsComponent>(args.User.Value, out var casterStats) &&
                entManager.TryGetComponent<ProjectileComponent>(ent, out var proj))
            {
                var effInt = Math.Clamp(
                    casterStats.Intelligence + casterStats.IntelligenceModifier,
                    1,
                    casterStats.MaxStatValue);
                var multiplier = effInt >= 5
                    ? 1.0f + (effInt - 5) * 0.10f
                    : 1.0f + (effInt - 5) * 0.125f;
                proj.Damage *= multiplier;
            }

            var direction = offsetedTargetPoint.ToMapPos(entManager, transform) -
                            spawnCoords.ToMapPos(entManager, transform);

            gunSystem.ShootProjectile(ent, direction, SaveVelocity ? userVelocity : new Vector2(), args.User!.Value, args.User, ProjectileSpeed);
        }
    }
}
