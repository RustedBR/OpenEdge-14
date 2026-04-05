using System.Numerics;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Currency;

/// <summary>
/// Reflects the market value of an item, to guide players through the economy.
/// </summary>

[RegisterComponent]
public sealed partial class OE14CurrencyConverterComponent : Component
{
    [DataField]
    public int Balance;

    [DataField]
    public EntityWhitelist? Whitelist;

    [DataField]
    public Vector2 SpawnOffset = new Vector2(0, -0.4f);

    [DataField]
    public SoundSpecifier InsertSound = new SoundCollectionSpecifier("OE14Coins");

    [DataField]
    public ProtoId<TagPrototype> CoinTag = "OE14Coin";
}
