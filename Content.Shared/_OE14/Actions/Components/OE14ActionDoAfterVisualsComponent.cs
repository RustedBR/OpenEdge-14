using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Actions.Components;

/// <summary>
/// Creates a temporary entity that exists while the spell is cast, and disappears at the end. For visual special effects.
/// </summary>
[RegisterComponent, Access(typeof(OE14SharedActionSystem))]
public sealed partial class OE14ActionDoAfterVisualsComponent : Component
{
    [DataField]
    public EntityUid? SpawnedEntity;

    [DataField(required: true)]
    public EntProtoId Proto = default!;
}
