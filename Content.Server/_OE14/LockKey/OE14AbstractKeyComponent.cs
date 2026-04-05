
using Content.Shared._OE14.LockKey;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.LockKey;

[RegisterComponent]
public sealed partial class OE14AbstractKeyComponent : Component
{
    [DataField(required: true)]
    public ProtoId<OE14LockGroupPrototype> Group = default;

    [DataField]
    public bool DeleteOnFailure = true;
}
