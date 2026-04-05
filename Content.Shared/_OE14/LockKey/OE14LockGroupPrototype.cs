using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.LockKey;

/// <summary>
/// Group Affiliation. Used for “abstract key” mechanics,
/// where the key takes one of the free forms from identical rooms (10 different kinds of tavern rooms for example).
/// </summary>
[Prototype("OE14LockGroup")]
public sealed partial class OE14LockGroupPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;
}
