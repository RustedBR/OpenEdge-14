using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Vampire.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class OE14ShowVampireFactionComponent : Component
{
    [DataField, AutoNetworkedField]
    public ProtoId<OE14VampireFactionPrototype>? Faction;
}
