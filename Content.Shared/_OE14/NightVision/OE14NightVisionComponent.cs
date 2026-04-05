using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.NightVision;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class OE14NightVisionComponent : Component
{
    [DataField]
    public EntityUid? LocalLightEntity = null;

    [DataField, AutoNetworkedField]
    public EntProtoId LightPrototype = "OE14NightVisionLight";

    [DataField, AutoNetworkedField]
    public EntProtoId ActionPrototype = "OE14ActionToggleNightVision";

    [DataField]
    public EntityUid? ActionEntity = null;
}
