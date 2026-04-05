using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._OE14.Vampire.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(typeof(OE14SharedVampireSystem))]
public sealed partial class OE14VampireTreeCollectableComponent : Component
{
    [DataField]
    public FixedPoint2 Essence = 1f;

    [DataField]
    public SoundSpecifier CollectSound = new SoundPathSpecifier("/Audio/_OE14/Effects/essence_consume.ogg");
}
