using Content.Shared._OE14.MagicSpell.Spells;
using Robust.Shared.GameStates;

namespace Content.Shared._OE14.StatusEffect;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentPause]
[Access(typeof(OE14ApplySpellStatusEffectSystem))]
public sealed partial class OE14ApplySpellStatusEffectComponent : Component
{
    [DataField(serverOnly: true)]
    public List<OE14SpellEffect> StartEffect = new();

    [DataField(serverOnly: true)]
    public List<OE14SpellEffect> EndEffect = new();

    [DataField(serverOnly: true)]
    public List<OE14SpellEffect> UpdateEffect = new();

    [DataField]
    public TimeSpan UpdateFrequency = TimeSpan.FromSeconds(1f);

    [DataField, AutoPausedField]
    public TimeSpan NextUpdateTime = TimeSpan.Zero;
}
