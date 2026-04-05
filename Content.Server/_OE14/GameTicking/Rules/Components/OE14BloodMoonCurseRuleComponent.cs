using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.GameTicking.Rules.Components;

[RegisterComponent, Access(typeof(OE14BloodMoonCurseRule))]
public sealed partial class OE14BloodMoonCurseRuleComponent : Component
{
    [DataField]
    public LocId StartAnnouncement = "oe14-bloodmoon-start";

    [DataField]
    public LocId EndAnnouncement = "oe14-bloodmoon-end";

    [DataField]
    public Color? AnnouncementColor;

    [DataField]
    public EntProtoId CurseEffect = "OE14ImpactEffectMagicSplitting";

    [DataField]
    public SoundSpecifier GlobalSound = new SoundPathSpecifier("/Audio/_OE14/Ambience/blood_moon_raise.ogg")
    {
        Params = AudioParams.Default.WithVolume(-2f)
    };
}
