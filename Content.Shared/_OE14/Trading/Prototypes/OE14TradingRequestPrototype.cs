using Content.Shared._OE14.Workbench;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Trading.Prototypes;

[Prototype("oe14TradingRequest")]
public sealed partial class OE14TradingRequestPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;

    [DataField]
    public HashSet<ProtoId<OE14TradingFactionPrototype>> PossibleFactions = [];

    [DataField]
    public float GenerationWeight = 1f;

    [DataField]
    public int FromMinutes = 0;

    [DataField]
    public int? ToMinutes;

    [DataField]
    public int AdditionalReward = 10;

    [DataField]
    public float ReputationCashback = 0.015f;

    [DataField(required: true)]
    public List<OE14WorkbenchCraftRequirement> Requirements = new();
}
