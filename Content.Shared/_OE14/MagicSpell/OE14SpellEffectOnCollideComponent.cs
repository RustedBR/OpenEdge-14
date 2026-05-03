using Content.Shared._OE14.MagicSpell.Spells;
using Content.Shared.Whitelist;

namespace Content.Shared._OE14.MagicSpell;

/// <summary>
/// Component that allows an entity to apply spell effects to other entities on collision.
/// </summary>
[RegisterComponent]
public sealed partial class OE14SpellEffectOnCollideComponent : Component
{
    [DataField(required: true, serverOnly: true)]
    public List<OE14SpellEffect> Effects = new();

    [DataField]
    public EntityWhitelist? Whitelist;

    [DataField]
    public float Prob = 1f;
}
