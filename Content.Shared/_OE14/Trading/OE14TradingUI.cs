using Robust.Shared.Serialization;

namespace Content.Shared._OE14.Trading;

[Serializable, NetSerializable]
public enum OE14TradingUiKey
{
    Buy,
    Sell,
}

[Serializable, NetSerializable]
public sealed class OE14TradingPlatformUiState(NetEntity platform) : BoundUserInterfaceState
{
    public NetEntity Platform = platform;
}

[Serializable, NetSerializable]
public sealed class OE14SellingPlatformUiState(NetEntity platform, int price) : BoundUserInterfaceState
{
    public NetEntity Platform = platform;
    public int Price = price;
}

[Serializable, NetSerializable]
public readonly struct OE14TradingProductEntry
{
}
