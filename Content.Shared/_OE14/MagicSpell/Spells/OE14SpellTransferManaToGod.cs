using Content.Shared._OE14.MagicEnergy;
using Content.Shared._OE14.Religion.Components;
using Content.Shared._OE14.Religion.Systems;
using Content.Shared.FixedPoint;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellTransferManaToGod : OE14SpellEffect
{
    [DataField]
    public FixedPoint2 Amount = 10f;

    [DataField]
    public bool Safe = false;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.User is null)
            return;

        if (!entManager.TryGetComponent<OE14ReligionFollowerComponent>(args.User, out var follower))
            return;

        if (follower.Religion is null)
            return;

        var religionSys = entManager.System<OE14SharedReligionGodSystem>();
        var magicEnergySys = entManager.System<OE14SharedMagicEnergySystem>();

        var gods = religionSys.GetGods(follower.Religion.Value);
        var manaAmount = Amount / gods.Count;
        foreach (var god in gods)
        {
            magicEnergySys.TransferEnergy(args.User.Value, god.Owner, manaAmount, out _, out _, safe: Safe);
        }
    }
}
