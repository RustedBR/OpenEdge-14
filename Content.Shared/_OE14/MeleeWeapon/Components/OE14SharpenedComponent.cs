using Content.Shared._OE14.MeleeWeapon.EntitySystems;
using Robust.Shared.GameStates;

namespace Content.Shared._OE14.MeleeWeapon.Components;

/// <summary>
/// allows the object to become blunt with use
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class OE14SharpenedComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Sharpness = 1f;

    [DataField]
    public Dictionary<float, string> SharpnessExamineThresholds { get; private set; } = new()
    {
        { 0.95f, "sharpening-examined-95" },
        { 0.75f, "sharpening-examined-75" },
        { 0.5f, "sharpening-examined-50" },
        { 0, "sharpening-examined-25" },
    };

    [DataField]
    public float SharpnessDamageBy1Damage = 0.0005f; //2000 damage
}
