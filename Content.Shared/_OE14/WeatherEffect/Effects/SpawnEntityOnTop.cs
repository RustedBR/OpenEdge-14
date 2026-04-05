using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._OE14.WeatherEffect.Effects;

public sealed partial class SpawnEntityOnTop : OE14WeatherEffect
{
    [DataField(required: true)]
    public EntProtoId Entity;

    public override void ApplyEffect(IEntityManager entManager, IRobustRandom random, EntityUid target)
    {
        if (!random.Prob(Prob))
            return;

        entManager.SpawnAtPosition(Entity, entManager.GetComponent<TransformComponent>(target).Coordinates);
    }
}
