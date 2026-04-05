using Content.Server.Cargo.Systems;
using Content.Shared._OE14.Currency;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Tag;
using Content.Shared._OE14.Trading.Components;
using Content.Shared.Mobs.Components;

namespace Content.Server._OE14.Trading;

public sealed class OE14PriceScannerSystem : EntitySystem
{
    [Dependency] private readonly PricingSystem _price = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly InventorySystem _invSystem = default!;
    [Dependency] private readonly OE14SharedCurrencySystem _currency = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<MetaDataComponent, ExaminedEvent>(OnExamined);
    }

    private bool IsAbleExamine(EntityUid uid)
    {
        if (HasComp<OE14PriceScannerComponent>(uid))
            return true;
        if (_invSystem.TryGetSlotEntity(uid, "eyes", out var huds) && HasComp<OE14PriceScannerComponent>(huds))
            return true;

        return false;
    }

    private void OnExamined(EntityUid uid, MetaDataComponent component, ExaminedEvent args)
    {
        if (!IsAbleExamine(args.Examiner))
            return;
        if (_tag.HasTag(args.Examined, "OE14Coin"))
            return;
        if (HasComp<MobStateComponent>(uid))
            return;

        var price = Math.Round(_price.GetPrice(args.Examined));

        if (price <= 0)
            return;

        var priceMsg = Loc.GetString("oe14-currency-examine-title");

        priceMsg += _currency.GetCurrencyPrettyString((int)price);

        args.PushMarkup(priceMsg);
    }
}
