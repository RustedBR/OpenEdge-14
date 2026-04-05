using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.MeleeWeapon.Components;

/// <summary>
/// After several wide attacks, a light attack deals additional damage.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class OE14ComboBonusMeleeDamageComponent : Component
{
    [DataField]
    public DamageSpecifier BonusDamage = new();

    [DataField]
    public int HeavyAttackNeed = 2;

    [DataField, AutoNetworkedField]
    public int CurrentHeavyAttacks = 0;

    [DataField, AutoNetworkedField]
    public HashSet<EntityUid> HitEntities = new();

    [DataField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/_OE14/Effects/critical_sword.ogg")
    {
        Params = AudioParams.Default.WithVariation(0.125f),
    };

    [DataField]
    public EntProtoId VFX = "OE14MeleeCritEffect";
}
