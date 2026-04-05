using Robust.Shared.GameStates;

namespace Content.Shared._OE14.Vampire.Components;

/// <summary>
/// increases the amount of blood essence extracted if the victim is strapped to the altar
/// </summary>
[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState]
[Access(typeof(OE14SharedVampireSystem))]
public sealed partial class OE14VampireAltarComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Multiplier = 2f;
}
