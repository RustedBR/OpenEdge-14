using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Sponsor;

[Prototype("sponsorRole")]
public sealed partial class OE14SponsorRolePrototype : IPrototype
{
    [IdDataField] public string ID { get; } = string.Empty;

    [DataField(required: true)]
    public string Name = string.Empty;

    [DataField(required: true)]
    public string DiscordRoleId = string.Empty;

    [DataField]
    public Color? Color = null;

    [DataField]
    public float Priority = 0;

    [DataField]
    public bool Examinable = false;
}

[Prototype("sponsorFeature")]
public sealed partial class OE14SponsorFeaturePrototype : IPrototype
{
    [IdDataField] public string ID { get; } = string.Empty;

    [DataField]
    public float MinPriority = 1;
}
