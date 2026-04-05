using Content.Shared._OE14.MagicEnergy;
using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared.FixedPoint;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellConsumeManaEffect : OE14SpellEffect
{
    [DataField]
    public FixedPoint2 Mana = 0;

    [DataField]
    public bool Safe = false;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        var targetEntity = args.Target.Value;

        if (!entManager.HasComponent<OE14MagicEnergyContainerComponent>(targetEntity))
            return;

        var magicEnergy = entManager.System<OE14SharedMagicEnergySystem>();

        //First - used object
        if (args.Used is not null)
        {
            magicEnergy.TransferEnergy(targetEntity,
                args.Used.Value,
                Mana,
                out _,
                out _,
                safe: Safe);
            return;
        }

        //Second - player
        if (args.User is not null)
        {
            magicEnergy.TransferEnergy(targetEntity,
                args.User.Value,
                Mana,
                out _,
                out _,
                safe: Safe);
            return;
        }
    }
}
