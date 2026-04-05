using Content.Shared.StatusIcon;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Vampire;

[Prototype("oe14VampireFaction")]
public sealed partial class OE14VampireFactionPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public LocId Name = string.Empty;

    [DataField(required: true)]
    public ProtoId<FactionIconPrototype> FactionIcon;

    [DataField(required: true)]
    public string SingletonTeleportKey = string.Empty;
}
