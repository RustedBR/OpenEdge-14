using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._OE14.MagicWeakness;

/// <summary>
/// imposes damage on excessive use of magic
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(OE14SharedMagicWeaknessSystem))]
public sealed partial class OE14MagicUnsafeDamageComponent : Component
{
    [DataField]
    public DamageSpecifier DamagePerEnergy = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            {"OE14ManaDepletion", 0.8},
        },
    };
}
