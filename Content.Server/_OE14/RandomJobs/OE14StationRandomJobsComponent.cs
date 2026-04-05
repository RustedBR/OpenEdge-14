
using Content.Shared.Destructible.Thresholds;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.RandomJobs;

[RegisterComponent, Access(typeof(OE14StationRandomJobsSystem))]
public sealed partial class OE14StationRandomJobsComponent : Component
{
    [DataField]
    public List<OE14RandomJobEntry> Entries = new();
}

[Serializable, DataDefinition]
public sealed partial class OE14RandomJobEntry
{
    [DataField(required: true)]
    public List<ProtoId<JobPrototype>> Jobs = new();

    [DataField(required: true)]
    public MinMax Count = new(1, 1);

    [DataField]
    public float Prob = 1f;
}
