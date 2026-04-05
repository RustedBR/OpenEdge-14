using Content.Shared._OE14.Trading.Systems;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Trading.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(OE14SharedTradingPlatformSystem))]
public sealed partial class OE14TradingPlatformComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan NextBuyTime = TimeSpan.Zero;

    [DataField]
    public SoundSpecifier BuySound = new SoundPathSpecifier("/Audio/_OE14/Effects/cash.ogg")
    {
        Params = AudioParams.Default.WithVariation(0.1f)
    };

    [DataField]
    public ProtoId<TagPrototype> CoinTag = "OE14Coin";

    [DataField]
    public EntProtoId BuyVisual = "OE14CashImpact";


    [DataField]
    public float PlatformMarkupProcent = 1f;
}
