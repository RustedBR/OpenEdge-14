using Content.Shared._OE14.Religion.Prototypes;
using Content.Shared._OE14.Religion.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Religion.Components;

/// <summary>
/// Allows the god of a particular religion to see within a radius around the observer.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true), Access(typeof(OE14SharedReligionGodSystem))]
public sealed partial class OE14ReligionObserverComponent : Component
{
    [DataField, AutoNetworkedField]
    public ProtoId<OE14ReligionPrototype>? Religion;

    [DataField, AutoNetworkedField]
    public float Radius = 5f;

    [DataField, AutoNetworkedField]
    public bool Active = true;
}
