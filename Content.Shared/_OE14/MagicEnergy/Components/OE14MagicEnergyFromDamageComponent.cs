using Content.Shared.Damage.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MagicEnergy.Components;

/// <summary>
/// Restores or expends magical energy when taking damage of certain types.
/// </summary>
[RegisterComponent, Access(typeof(OE14SharedMagicEnergySystem))]
public sealed partial class OE14MagicEnergyFromDamageComponent : Component
{
    [DataField]
    public Dictionary<ProtoId<DamageTypePrototype>, float> Damage = new();
}
