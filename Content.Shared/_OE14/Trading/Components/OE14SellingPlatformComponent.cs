using Content.Shared._OE14.Trading.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Trading.Components;

/// <summary>
/// Allows you to sell items by overloading the platform with energy
/// </summary>
[RegisterComponent, Access(typeof(OE14SharedTradingPlatformSystem))]
public sealed partial class OE14SellingPlatformComponent : Component
{
    [DataField]
    public SoundSpecifier SellSound = new SoundPathSpecifier("/Audio/_OE14/Effects/cash.ogg")
    {
        Params = AudioParams.Default.WithVariation(0.1f),
    };

    [DataField]
    public EntProtoId SellVisual = "OE14CashImpact";

    [DataField]
    public float PlatformMarkupProcent = 1f;
}
