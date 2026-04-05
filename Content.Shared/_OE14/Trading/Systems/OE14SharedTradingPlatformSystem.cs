using Content.Shared._OE14.Trading.Components;
using Content.Shared._OE14.Trading.Prototypes;
using Content.Shared.Interaction.Events;
using Content.Shared.Placeable;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared._OE14.Trading.Systems;

public abstract partial class OE14SharedTradingPlatformSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _userInterface = default!;
    [Dependency] protected readonly IPrototypeManager Proto = default!;
    [Dependency] protected readonly IGameTiming Timing = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    public override void Initialize()
    {
        base.Initialize();
        InitializeUI();

        SubscribeLocalEvent<OE14TradingReputationComponent, MapInitEvent>(OnReputationMapInit);
        SubscribeLocalEvent<OE14TradingContractComponent, UseInHandEvent>(OnContractUse);
    }

    private void OnReputationMapInit(Entity<OE14TradingReputationComponent> ent, ref MapInitEvent args)
    {
        foreach (var faction in Proto.EnumeratePrototypes<OE14TradingFactionPrototype>())
        {
            if (faction.RoundStart is not null)
            {
                ent.Comp.Reputation[faction] = ent.Comp.Reputation.GetValueOrDefault(faction, 0f) + faction.RoundStart.Value;
            }
        }
        Dirty(ent);
    }

    private void OnContractUse(Entity<OE14TradingContractComponent> ent, ref UseInHandEvent args)
    {
        if (args.Handled)
            return;
        if (!Proto.TryIndex(ent.Comp.Faction, out var indexedFaction))
            return;

        args.Handled = true;

        var repComp = EnsureComp<OE14TradingReputationComponent>(args.User);
        repComp.Reputation.TryAdd(ent.Comp.Faction, 0);
        _audio.PlayLocal(new SoundCollectionSpecifier("OE14CoinImpact"), args.User, args.User);
        _popup.PopupClient(Loc.GetString("oe14-trading-contract-use", ("name", Loc.GetString(indexedFaction.Name))), args.User, args.User);

        if (_net.IsServer)
            QueueDel(ent);
    }

    public bool CanBuyPosition(Entity<OE14TradingReputationComponent?> user, ProtoId<OE14TradingPositionPrototype> position)
    {
        if (!Resolve(user.Owner, ref user.Comp, false))
            return false;
        if (!Proto.TryIndex(position, out var indexedPosition))
            return false;

        if (user.Comp.Reputation[indexedPosition.Faction] < indexedPosition.ReputationLevel)
            return false;

        return true;
    }

    public void AddReputation(Entity<OE14TradingReputationComponent?> user,
        ProtoId<OE14TradingFactionPrototype> faction, float rep)
    {
        if (!Resolve(user.Owner, ref user.Comp, false))
            return;

        if (!user.Comp.Reputation.ContainsKey(faction))
            user.Comp.Reputation.Add(faction, rep);
        else
            user.Comp.Reputation[faction] += rep;

        Dirty(user);
    }

    public bool CanFulfillRequest(EntityUid platform, ProtoId<OE14TradingRequestPrototype> request)
    {
        if (!TryComp<ItemPlacerComponent>(platform, out var itemPlacer))
            return false;

        if (!Proto.TryIndex(request, out var indexedRequest))
            return false;

        foreach (var requirement in indexedRequest.Requirements)
        {
            if (!requirement.CheckRequirement(EntityManager, Proto, itemPlacer.PlacedEntities))
                return false;
        }

        return true;
    }
}

[Serializable, NetSerializable]
public sealed class OE14TradingPositionBuyAttempt(ProtoId<OE14TradingPositionPrototype> position) : BoundUserInterfaceMessage
{
    public readonly ProtoId<OE14TradingPositionPrototype> Position = position;
}

[Serializable, NetSerializable]
public sealed class OE14TradingRequestSellAttempt(ProtoId<OE14TradingRequestPrototype> request, ProtoId<OE14TradingFactionPrototype> faction) : BoundUserInterfaceMessage
{
    public readonly ProtoId<OE14TradingRequestPrototype> Request = request;
    public readonly ProtoId<OE14TradingFactionPrototype> Faction = faction;
}


[Serializable, NetSerializable]
public sealed class OE14TradingSellAttempt : BoundUserInterfaceMessage
{
}
