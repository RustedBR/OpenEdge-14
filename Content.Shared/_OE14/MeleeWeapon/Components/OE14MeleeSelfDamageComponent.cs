using Content.Shared.Damage;

namespace Content.Shared._OE14.MeleeWeapon.Components;

[RegisterComponent]
public sealed partial class OE14MeleeSelfDamageComponent : Component
{
    [DataField]
    public DamageSpecifier DamageToSelf = new()
    {
        DamageDict = new()
        {
            { "Blunt", 1 },
        }
    };
}
