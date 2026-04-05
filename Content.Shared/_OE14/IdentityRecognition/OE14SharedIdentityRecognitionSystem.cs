using Content.Shared.Examine;
using Content.Shared.Ghost;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Player;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._OE14.IdentityRecognition;

public abstract class OE14SharedIdentityRecognitionSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14UnknownIdentityComponent, GetVerbsEvent<Verb>>(OnUnknownIdentityVerb);
        SubscribeLocalEvent<OE14UnknownIdentityComponent, ExaminedEvent>(OnExaminedEvent);

        SubscribeLocalEvent<MindContainerComponent, OE14RememberedNameChangedMessage>(OnRememberedNameChanged);

        SubscribeLocalEvent<OE14RememberedNamesComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<OE14RememberedNamesComponent> ent, ref MapInitEvent args)
    {
        if (!TryComp<MindComponent>(ent, out var mind))
            return;

        if (mind.OwnedEntity is null)
            return;

        if (mind.CharacterName is null)
            return;

        RememberCharacter(ent, GetNetEntity(mind.OwnedEntity.Value), mind.CharacterName);
    }

    private void OnUnknownIdentityVerb(Entity<OE14UnknownIdentityComponent> ent, ref GetVerbsEvent<Verb> args)
    {
        if (HasComp<GhostComponent>(args.User))
            return;

        if(!_mind.TryGetMind(args.User, out var mindId, out var mind))
            return;

        if (!TryComp<ActorComponent>(args.User, out var actor))
            return;

        if (args.User == ent.Owner)
            return;

        EnsureComp<OE14RememberedNamesComponent>(mindId);

        var seeAttemptEv = new SeeIdentityAttemptEvent();
        RaiseLocalEvent(ent.Owner, seeAttemptEv);

        var _args = args;
        var verb = new Verb
        {
            Priority = 2,
            Icon =  new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/sentient.svg.192dpi.png")),
            Text = Loc.GetString("oe14-remember-name-verb"),
            Act = () =>
            {
                if (seeAttemptEv.Cancelled)
                {
                    _popup.PopupClient(Loc.GetString("oe14-remember-fail-mask"), _args.Target, _args.User);
                }
                else
                {
                    _uiSystem.SetUiState(_args.User, OE14RememberNameUiKey.Key, new OE14RememberNameUiState(GetNetEntity(ent)));
                    _uiSystem.TryToggleUi(_args.User, OE14RememberNameUiKey.Key, actor.PlayerSession);
                }
            },
        };
        args.Verbs.Add(verb);
    }

    private void OnExaminedEvent(Entity<OE14UnknownIdentityComponent> ent, ref ExaminedEvent args)
    {
        var ev = new SeeIdentityAttemptEvent();
        RaiseLocalEvent(ent.Owner, ev);

        if (ev.Cancelled)
            return;

        if (!_mind.TryGetMind(args.Examiner, out var mindId, out var mind))
            return;

        if (!TryComp<OE14RememberedNamesComponent>(mindId, out var knownNames))
            return;

        if (knownNames.Names.TryGetValue(GetNetEntity(ent).Id, out var name))
        {
            args.PushMarkup(Loc.GetString("oe14-remember-name-examine", ("name", name)), priority: -1);
        }
    }

    private void OnRememberedNameChanged(Entity<MindContainerComponent> ent, ref OE14RememberedNameChangedMessage args)
    {
        var mindEntity = ent.Comp.Mind;

        if (mindEntity is null)
            return;

        RememberCharacter(mindEntity.Value, args.Target, args.Name);
    }

    private void RememberCharacter(EntityUid mindEntity, NetEntity targetId, string name)
    {
        var knownNames = EnsureComp<OE14RememberedNamesComponent>(mindEntity);

        knownNames.Names[targetId.Id] = name;
        Dirty(mindEntity, knownNames);
    }
}

[Serializable, NetSerializable]
public sealed class OE14RememberedNameChangedMessage(string name, NetEntity target) : BoundUserInterfaceMessage
{
    public string Name { get; } = name;
    public NetEntity Target { get; } = target;
}

[Serializable, NetSerializable]
public enum OE14RememberNameUiKey
{
    Key,
}

[Serializable, NetSerializable]
public sealed class OE14RememberNameUiState(NetEntity target) : BoundUserInterfaceState
{
    public NetEntity Target = target;
}
