using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.GameTicking.Rules.Components;

[RegisterComponent, Access(typeof(OE14BloodMoonRule))]
public sealed partial class OE14BloodMoonRuleComponent : Component
{
    [DataField]
    public EntProtoId CurseRule = "OE14BloodMoonCurseRule";

    [DataField]
    public LocId StartAnnouncement = "oe14-bloodmoon-raising";

    [DataField]
    public Color? AnnouncementColor = Color.FromHex("#e32759");

    [DataField]
    public SoundSpecifier? AnnounceSound;
}
