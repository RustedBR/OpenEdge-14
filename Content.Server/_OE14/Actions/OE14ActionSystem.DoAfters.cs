using Content.Server.Chat.Systems;
using Content.Shared._OE14.Actions;
using Content.Shared._OE14.Actions.Components;
using Content.Shared.Actions.Events;

namespace Content.Server._OE14.Actions;

public sealed partial class OE14ActionSystem
{
    private void InitializeDoAfter()
    {
        SubscribeLocalEvent<OE14ActionSpeakingComponent, OE14ActionStartDoAfterEvent>(OnVerbalActionStarted);
        SubscribeLocalEvent<OE14ActionSpeakingComponent, ActionDoAfterEvent>(OnVerbalActionPerformed);

        SubscribeLocalEvent<OE14ActionEmotingComponent, OE14ActionStartDoAfterEvent>(OnEmoteActionStarted);
        SubscribeLocalEvent<OE14ActionEmotingComponent, ActionDoAfterEvent>(OnEmoteActionPerformed);

        SubscribeLocalEvent<OE14ActionDoAfterVisualsComponent, OE14ActionStartDoAfterEvent>(OnSpawnMagicVisualEffect);
        SubscribeLocalEvent<OE14ActionDoAfterVisualsComponent, ActionDoAfterEvent>(OnDespawnMagicVisualEffect);
    }

    private void OnVerbalActionStarted(Entity<OE14ActionSpeakingComponent> ent, ref OE14ActionStartDoAfterEvent args)
    {
        var performer = GetEntity(args.Performer);
        _chat.TrySendInGameICMessage(performer, ent.Comp.StartSpeech, ent.Comp.Whisper ? InGameICChatType.Whisper : InGameICChatType.Speak, true);
    }

    private void OnEmoteActionStarted(Entity<OE14ActionEmotingComponent> ent, ref OE14ActionStartDoAfterEvent args)
    {
        var performer = GetEntity(args.Performer);
        _chat.TrySendInGameICMessage(performer, Loc.GetString(ent.Comp.StartEmote), InGameICChatType.Emote, true);
    }

    private void OnVerbalActionPerformed(Entity<OE14ActionSpeakingComponent> ent, ref ActionDoAfterEvent args)
    {
        if (args.Cancelled)
            return;

        if (!args.Handled)
            return;

        var performer = GetEntity(args.Performer);
        _chat.TrySendInGameICMessage(performer, ent.Comp.EndSpeech, ent.Comp.Whisper ? InGameICChatType.Whisper : InGameICChatType.Speak, true);
    }

    private void OnEmoteActionPerformed(Entity<OE14ActionEmotingComponent> ent, ref ActionDoAfterEvent args)
    {
        if (args.Cancelled)
            return;

        if (!args.Handled)
            return;

        var performer = GetEntity(args.Performer);
        _chat.TrySendInGameICMessage(performer, Loc.GetString(ent.Comp.EndEmote), InGameICChatType.Emote, true);
    }

    private void OnSpawnMagicVisualEffect(Entity<OE14ActionDoAfterVisualsComponent> ent, ref OE14ActionStartDoAfterEvent args)
    {
        QueueDel(ent.Comp.SpawnedEntity);

        var performer = GetEntity(args.Performer);
        var vfx = SpawnAttachedTo(ent.Comp.Proto, Transform(performer).Coordinates);
        _transform.SetParent(vfx, performer);
        ent.Comp.SpawnedEntity = vfx;
    }

    private void OnDespawnMagicVisualEffect(Entity<OE14ActionDoAfterVisualsComponent> ent, ref ActionDoAfterEvent args)
    {
        if (args.Repeat)
            return;

        QueueDel(ent.Comp.SpawnedEntity);
        ent.Comp.SpawnedEntity = null;
    }
}
