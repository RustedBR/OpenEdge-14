using Content.Shared.Beam;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellCreateBeam : OE14SpellEffect
{
    [DataField(required: true)]
    public EntProtoId BeamProto = default!;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null || args.User is null)
            return;

        var beamSys = entManager.System<SharedBeamSystem>();

        beamSys.TryCreateBeam(args.User.Value, args.Target.Value, BeamProto);
    }
}
