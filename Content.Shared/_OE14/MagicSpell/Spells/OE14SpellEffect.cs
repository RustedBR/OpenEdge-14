using JetBrains.Annotations;
using Robust.Shared.Map;

namespace Content.Shared._OE14.MagicSpell.Spells;

[ImplicitDataDefinitionForInheritors]
[MeansImplicitUse]
public abstract partial class OE14SpellEffect
{
    public abstract void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args);
}

public record OE14SpellEffectBaseArgs
{
    public EntityUid? User;
    public EntityUid? Used;
    public EntityUid? Target;
    public EntityCoordinates? Position;

    public OE14SpellEffectBaseArgs(EntityUid? user, EntityUid? used, EntityUid? target, EntityCoordinates? position)
    {
        User = user;
        Used = used;
        Target = target;
        Position = position;
    }
}
