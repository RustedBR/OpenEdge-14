using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.StationCommonObjectives;

[RegisterComponent]
public sealed partial class OE14StationCommonObjectivesComponent : Component
{
    public Dictionary<EntityUid, ProtoId<JobPrototype>> JobObjectives = new();
    public Dictionary<EntityUid, ProtoId<DepartmentPrototype>> DepartmentObjectives = new();
}
