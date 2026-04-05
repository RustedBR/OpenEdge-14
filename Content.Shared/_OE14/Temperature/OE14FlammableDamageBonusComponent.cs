using Content.Shared.Damage;

namespace Content.Shared._OE14.Temperature;

/// <summary>
/// Add bonus damage to melee attacks per flammable stack
/// </summary>
[RegisterComponent, Access(typeof(OE14SharedFireSpreadSystem))]
public sealed partial class OE14FlammableBonusDamageComponent : Component
{
    [DataField]
    public DamageSpecifier DamagePerStack = new()
    {
        DamageDict = new()
        {
            {"Heat", 0.3},
        }
    };
}
