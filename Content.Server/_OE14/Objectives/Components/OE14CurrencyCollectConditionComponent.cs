using Content.Server._OE14.Objectives.Systems;
using Robust.Shared.Utility;

namespace Content.Server._OE14.Objectives.Components;

[RegisterComponent, Access(typeof(OE14CurrencyCollectConditionSystem))]
public sealed partial class OE14CurrencyCollectConditionComponent : Component
{
    [DataField]
    public int Currency = 1000;

    [DataField(required: true)]
    public LocId ObjectiveText;

    [DataField(required: true)]
    public LocId ObjectiveDescription;

    [DataField(required: true)]
    public SpriteSpecifier ObjectiveSprite;
}
