using System.Linq;
using Content.Server.Chat.Systems;
using Content.Server.Chat.Managers;
using Content.Server.UserInterface;
using Content.Shared._OE14.CommunicationCrystal;
using Content.Shared._OE14.CommunicationCrystal.Components;
using Content.Shared._OE14.MagicEnergy.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Timing;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using Content.Shared.UserInterface;
using Robust.Server.GameObjects;
using Content.Shared.Popups;
using Content.Shared.Interaction;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Verbs;
using Content.Server._OE14.MagicEnergy;
using Content.Shared.Chat;

namespace Content.Server._OE14.CommunicationCrystal;

public sealed partial class OE14CommunicationCrystalSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly UserInterfaceSystem _userInterface = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly OE14MagicEnergySystem _magicEnergy = default!;
    [Dependency] private readonly Robust.Shared.Timing.IGameTiming _timing = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<OE14CommunicationCrystalComponent, BeforeActivatableUIOpenEvent>(OnBeforeUIOpen);
        SubscribeLocalEvent<OE14CommunicationCrystalComponent, OE14CommunicationCrystalSendMessage>(OnSendMessage);
        SubscribeLocalEvent<OE14CommunicationCrystalComponent, OE14CommunicationCrystalRemoveCrystal>(OnRemoveCrystal);
        SubscribeLocalEvent<OE14CommunicationCrystalComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<OE14CommunicationCrystalComponent, GetVerbsEvent<AlternativeVerb>>(OnGetAlternativeVerbs);
    }

    private void OnGetAlternativeVerbs(Entity<OE14CommunicationCrystalComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!TryComp<ContainerManagerComponent>(ent, out var containerManager))
            return;

        if (!_container.TryGetContainer(ent, "crystal_slot", out var container, containerManager))
            return;

        if (container.ContainedEntities.Count == 0)
            return;

        var user = args.User;
        AlternativeVerb verb = new()
        {
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/eject.svg.192dpi.png")),
            Act = () => TryEjectCrystal(ent, user),
            Priority = 0
        };

        args.Verbs.Add(verb);
    }

    private void TryEjectCrystal(Entity<OE14CommunicationCrystalComponent> ent, EntityUid user)
    {
        if (!TryComp<ContainerManagerComponent>(ent, out var containerManager))
            return;

        if (!_container.TryGetContainer(ent, "crystal_slot", out var container, containerManager))
            return;

        if (container.ContainedEntities.Count == 0)
            return;

        var crystal = container.ContainedEntities[0];
        _container.Remove(crystal, container);

        if (_hands.TryPickupAnyHand(user, crystal))
        {
            _audio.PlayPvs("/Audio/_OE14/Items/crystal_eject.ogg", Transform(ent).Coordinates);
        }
        else
        {
            _audio.PlayPvs("/Audio/_OE14/Items/crystal_eject.ogg", Transform(ent).Coordinates);
        }

        UpdateUIState(ent);
    }

    private void OnRemoveCrystal(Entity<OE14CommunicationCrystalComponent> ent, ref OE14CommunicationCrystalRemoveCrystal args)
    {
        if (!TryComp<ContainerManagerComponent>(ent, out var containerManager))
            return;

        if (!_container.TryGetContainer(ent, "crystal_slot", out var container, containerManager))
            return;

        if (container.ContainedEntities.Count == 0)
            return;

        var crystal = container.ContainedEntities[0];
        _container.Remove(crystal, container);

        if (_hands.TryPickupAnyHand(ent.Owner, crystal))
        {
            _audio.PlayPvs("/Audio/_OE14/Items/crystal_eject.ogg", Transform(ent).Coordinates);
        }
        else
        {
            _audio.PlayPvs("/Audio/_OE14/Items/crystal_eject.ogg", Transform(ent).Coordinates);
        }

        UpdateUIState(ent);
    }

    private void OnInteractUsing(Entity<OE14CommunicationCrystalComponent> ent, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp<ContainerManagerComponent>(ent, out var containerManager))
            return;

        if (!_container.TryGetContainer(ent, "crystal_slot", out var container, containerManager))
            return;

        if (container.ContainedEntities.Count > 0)
        {
            _popup.PopupEntity(Loc.GetString("oe14-comm-crystal-already-has"), ent);
            return;
        }

        if (!HasComp<OE14MagicEnergyCrystalComponent>(args.Used))
        {
            _popup.PopupEntity(Loc.GetString("oe14-comm-crystal-need-energy"), ent, args.User);
            return;
        }

        _container.Insert(args.Used, container);
        _audio.PlayPvs("/Audio/_OE14/Items/crystal_insert.ogg", Transform(ent).Coordinates);

        UpdateUIState(ent);
        args.Handled = true;
    }

    private void OnBeforeUIOpen(Entity<OE14CommunicationCrystalComponent> ent, ref BeforeActivatableUIOpenEvent args)
    {
        UpdateUIState(ent);
    }

    private void OnSendMessage(Entity<OE14CommunicationCrystalComponent> ent, ref OE14CommunicationCrystalSendMessage args)
    {
        var cost = args.IsGlobal
            ? OE14CommunicationCrystalComponent.GlobalCost
            : OE14CommunicationCrystalComponent.LocalCost;

        if (string.IsNullOrWhiteSpace(args.Message))
            return;

        if (args.Message.Length > 500)
        {
            return;
        }

        if (!TryComp<ContainerManagerComponent>(ent, out var containerManager))
            return;

        if (!_container.TryGetContainer(ent, "crystal_slot", out var container, containerManager))
            return;

        if (container.ContainedEntities.Count == 0)
        {
            UpdateUIState(ent);
            return;
        }

        var crystal = container.ContainedEntities[0];
        if (!TryComp<OE14MagicEnergyContainerComponent>(crystal, out var crystalEnergy))
        {
            UpdateUIState(ent);
            return;
        }

        if (crystalEnergy.Energy < cost)
        {
            _popup.PopupEntity(Loc.GetString("oe14-comm-crystal-insufficient-energy",
                ("current", (int)crystalEnergy.Energy),
                ("required", cost)), ent);
            UpdateUIState(ent);
            return;
        }

        if (args.IsGlobal)
        {
            var cooldown = ent.Comp.LastGlobalAnnouncement;
            if (cooldown != null)
            {
                var elapsed = _timing.CurTime - cooldown.Value;
                if (elapsed.TotalSeconds < OE14CommunicationCrystalComponent.GlobalCooldownSeconds)
                {
                    var remaining = OE14CommunicationCrystalComponent.GlobalCooldownSeconds - elapsed.TotalSeconds;
                    _popup.PopupEntity(Loc.GetString("oe14-comm-crystal-global-cooldown",
                        ("seconds", (int)remaining)), ent);
                    return;
                }
            }

            ent.Comp.LastGlobalAnnouncement = _timing.CurTime;
            Dirty(ent);
        }

        _magicEnergy.ChangeEnergy(crystal, -cost, out var delta, out var overload);

        var sender = ent.Comp.AnnouncementTitle;
        if (args.IsGlobal)
        {
            var message = FormattedMessage.RemoveMarkupPermissive(args.Message.Trim());
            var senderText = Loc.GetString("oe14-comm-crystal-announcer", ("sender", sender));
            var formatted =
                $"\n{message}\n";
            _chat.DispatchGlobalAnnouncement(formatted, $"\n{senderText}", playSound: true, colorOverride: ent.Comp.AnnouncementColor);
        }
        else
        {
            var message = FormattedMessage.RemoveMarkupPermissive(args.Message.Trim());
            var senderHex = ent.Comp.SenderColor.ToHex();
            var localHex = ent.Comp.LocalMessageColor.ToHex();
            var formatted = $"[color={senderHex}]{sender}:[/color] [color={localHex}]{message}[/color]";

            var mapId = Transform(ent).MapID;
            if (mapId == MapId.Nullspace)
            {
                _chatManager.ChatMessageToAll(ChatChannel.Local, formatted, formatted, ent, false, true);
            }
            else
            {
                var filter = Filter.BroadcastMap(mapId);
                _chatManager.ChatMessageToManyFiltered(filter, ChatChannel.Local, formatted, formatted, ent, false, true, null);
            }
        }

        UpdateUIState(ent);
    }

    private void UpdateUIState(Entity<OE14CommunicationCrystalComponent> ent)
    {
        int currentEnergy = 0;
        int maxEnergy = 50;
        var hasCrystal = false;
        var canSendGlobal = false;

        if (TryComp<ContainerManagerComponent>(ent, out var containerManager))
        {
            if (_container.TryGetContainer(ent, "crystal_slot", out var container, containerManager))
            {
                if (container.ContainedEntities.Count > 0)
                {
                    var crystal = container.ContainedEntities[0];
                    if (TryComp<OE14MagicEnergyContainerComponent>(crystal, out var crystalEnergy))
                    {
                        hasCrystal = true;
                        currentEnergy = (int)crystalEnergy.Energy;
                        maxEnergy = (int)crystalEnergy.MaxEnergy;
                        canSendGlobal = currentEnergy >= OE14CommunicationCrystalComponent.GlobalCost;
                    }
                }
            }
        }

        var cooldown = ent.Comp.LastGlobalAnnouncement;
        TimeSpan? remainingCooldown = null;
        if (cooldown != null)
        {
            var elapsed = _timing.CurTime - cooldown.Value;
            if (elapsed.TotalSeconds < OE14CommunicationCrystalComponent.GlobalCooldownSeconds)
            {
                remainingCooldown = TimeSpan.FromSeconds(
                    OE14CommunicationCrystalComponent.GlobalCooldownSeconds - elapsed.TotalSeconds);
            }
        }

        var state = new OE14CommunicationCrystalUiState(
            currentEnergy,
            maxEnergy,
            hasCrystal,
            canSendGlobal,
            remainingCooldown);

        _userInterface.SetUiState(ent.Owner, OE14CommunicationCrystalUiKey.Board, state);
    }
}