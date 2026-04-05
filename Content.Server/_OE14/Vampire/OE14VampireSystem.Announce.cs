using Content.Server.Administration.Managers;
using Content.Server.Chat.Systems;
using Content.Shared._OE14.Vampire;
using Content.Shared._OE14.Vampire.Components;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.Vampire;

public sealed partial class OE14VampireSystem
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly IAdminManager _admin = default!;

    private void InitializeAnnounces()
    {
        SubscribeLocalEvent<OE14VampireClanHeartComponent, MapInitEvent>(OnHeartCreate);
        SubscribeLocalEvent<OE14VampireClanHeartComponent, DamageChangedEvent>(OnHeartDamaged);
        SubscribeLocalEvent<OE14VampireClanHeartComponent, ComponentRemove>(OnHeartDestructed);
    }

    private void OnHeartCreate(Entity<OE14VampireClanHeartComponent> ent, ref MapInitEvent args)
    {
        if (ent.Comp.Faction is null)
            return;

        if (!Proto.TryIndex(ent.Comp.Faction, out var indexedFaction))
            return;

        AnnounceToFaction(ent.Comp.Faction.Value, Loc.GetString("oe14-vampire-tree-created", ("name", Loc.GetString(indexedFaction.Name))));
        AnnounceToOpposingFactions(ent.Comp.Faction.Value, Loc.GetString("oe14-vampire-tree-created", ("name", Loc.GetString(indexedFaction.Name))));
    }

    private void OnHeartDamaged(Entity<OE14VampireClanHeartComponent> ent, ref DamageChangedEvent args)
    {
        if (ent.Comp.Faction is null)
            return;

        if (!args.DamageIncreased)
            return;

        if (_timing.CurTime < ent.Comp.NextAnnounceTime)
            return;

        ent.Comp.NextAnnounceTime = _timing.CurTime + ent.Comp.MaxAnnounceFreq;

        AnnounceToFaction(ent.Comp.Faction.Value, Loc.GetString("oe14-vampire-tree-damaged"));
    }

    private void OnHeartDestructed(Entity<OE14VampireClanHeartComponent> ent, ref ComponentRemove args)
    {
        if (ent.Comp.Faction is null)
            return;

        if (!Proto.TryIndex(ent.Comp.Faction, out var indexedFaction))
            return;

        AnnounceToFaction(ent.Comp.Faction.Value, Loc.GetString("oe14-vampire-tree-destroyed-self"));
        AnnounceToOpposingFactions(ent.Comp.Faction.Value, Loc.GetString("oe14-vampire-tree-destroyed", ("name", Loc.GetString(indexedFaction.Name))));
    }

    public void AnnounceToFaction(ProtoId<OE14VampireFactionPrototype> faction, string message)
    {
        var filter = Filter.Empty();
        var query = EntityQueryEnumerator<OE14VampireComponent, ActorComponent>();

        while (query.MoveNext(out var uid, out var vampire, out var actor))
        {
            if (vampire.Faction != faction)
                continue;

            filter.AddPlayer(actor.PlayerSession);
        }

        if (filter.Count == 0)
            return;

        VampireAnnounce(filter, message);
    }

    public void AnnounceToOpposingFactions(ProtoId<OE14VampireFactionPrototype> faction, string message)
    {
        var filter = Filter.Empty();
        var query = EntityQueryEnumerator<OE14VampireComponent, ActorComponent>();

        while (query.MoveNext(out var uid, out var vampire, out var actor))
        {
            if (vampire.Faction == faction)
                continue;

            filter.AddPlayer(actor.PlayerSession);
        }

        filter.AddPlayers(_admin.ActiveAdmins);

        if (filter.Count == 0)
            return;

        VampireAnnounce(filter, message);
    }

    private void VampireAnnounce(Filter players, string message)
    {
        _chat.DispatchFilteredAnnouncement(
            players,
            message,
            sender: Loc.GetString("oe14-vampire-sender"),
            announcementSound: new SoundPathSpecifier("/Audio/_OE14/Announce/vampire.ogg"),
            colorOverride: Color.FromHex("#820e22"));
    }
}
