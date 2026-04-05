using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.BloodMoon;


[RegisterComponent, NetworkedComponent]
public sealed partial class OE14BloodMoonCurseComponent : Component
{
    [DataField]
    public EntityUid? CurseRule;

    [DataField]
    public EntProtoId CurseEffect = "OE14BloodMoonCurseEffect";

    [DataField]
    public EntityUid? SpawnedEffect;

    [DataField]
    public TimeSpan EndStunDuration = TimeSpan.FromSeconds(60f);

    [DataField]
    public EntProtoId Action = "OE14ActionSpellBloodlust";

    [DataField]
    public EntityUid? ActionEntity;
}
