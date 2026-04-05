using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._OE14.Vampire.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState]
[Access(typeof(OE14SharedVampireSystem))]
public sealed partial class OE14VampireEssenceHolderComponent : Component
{
    [DataField, AutoNetworkedField]
    public FixedPoint2 Essence = 1f;
}
