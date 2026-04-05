using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.StatusEffect;

[RegisterComponent, NetworkedComponent]
[Access(typeof(OE14ApplySpellStatusEffectSystem))]
public sealed partial class OE14DamageModifierStatusEffectComponent : Component
{
    [DataField]
    public Dictionary<ProtoId<DamageTypePrototype>, float>? Defence = null;

    [DataField]
    public float GlobalDefence = 1f;
}
