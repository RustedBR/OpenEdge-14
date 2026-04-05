using Content.Shared._OE14.Religion.Prototypes;
using Content.Shared._OE14.Religion.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Religion.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(OE14SharedReligionGodSystem))]
public sealed partial class OE14ReligionAltarComponent : Component
{
    [DataField, AutoNetworkedField]
    public ProtoId<OE14ReligionPrototype>? Religion;

    [DataField, AutoNetworkedField]
    public bool CanBeConverted = true;
}
