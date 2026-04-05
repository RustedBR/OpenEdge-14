using Content.Shared._OE14.Religion.Components;
using Content.Shared._OE14.Religion.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellGodTouch : OE14SpellEffect
{
    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (args.Target is null)
            return;

        if (!entManager.TryGetComponent<OE14ReligionEntityComponent>(args.User, out var god) || god.Religion is null)
            return;

        var ev = new OE14GodTouchEvent(god.Religion.Value);
        entManager.EventBus.RaiseLocalEvent(args.Target.Value, ev);
    }
}

public sealed class OE14GodTouchEvent(ProtoId<OE14ReligionPrototype> religion) : EntityEventArgs
{
    public ProtoId<OE14ReligionPrototype> Religion = religion;
}
