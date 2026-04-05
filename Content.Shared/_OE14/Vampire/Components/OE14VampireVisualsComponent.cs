using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Vampire;

[RegisterComponent, NetworkedComponent]
public sealed partial class OE14VampireVisualsComponent : Component
{
    [DataField]
    public Color EyesColor = Color.Red;

    [DataField]
    public Color OriginalEyesColor = Color.White;

    [DataField]
    public string FangsMap = "vampire_fangs";

    [DataField]
    public EntProtoId EnableVFX = "OE14ImpactEffectBloodEssence2";

    [DataField]
    public EntProtoId DisableVFX = "OE14ImpactEffectBloodEssenceInverse";
}
