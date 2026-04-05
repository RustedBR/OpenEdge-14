using Content.Shared._OE14.Vampire;
using Content.Shared._OE14.Vampire.Components;
using Content.Shared.FixedPoint;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellVampireGatherEssence : OE14SpellEffect
{
    [DataField]
    public FixedPoint2 Amount = 0.2f;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        if (args.User is null)
            return;

        if (entManager.HasComponent<OE14VampireComponent>(args.Target.Value))
            return;

        if (!entManager.TryGetComponent<OE14VampireEssenceHolderComponent>(args.Target.Value, out var essenceHolder))
            return;

        var vamp = entManager.System<OE14SharedVampireSystem>();
        vamp.GatherEssence(args.User.Value, args.Target.Value, Amount);
    }
}
