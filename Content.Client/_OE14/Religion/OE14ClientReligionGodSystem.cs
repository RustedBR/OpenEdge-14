using Content.Shared._OE14.Religion.Components;
using Content.Shared._OE14.Religion.Prototypes;
using Content.Shared._OE14.Religion.Systems;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client._OE14.Religion;

public sealed partial class OE14ClientReligionGodSystem : OE14SharedReligionGodSystem
{
    [Dependency] private readonly IOverlayManager _overlayMgr = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    private OE14ReligionVisionOverlay? _overlay;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14ReligionVisionComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<OE14ReligionVisionComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);
        SubscribeLocalEvent<OE14ReligionVisionComponent, ComponentInit>(OnOverlayInit);
        SubscribeLocalEvent<OE14ReligionVisionComponent, ComponentRemove>(OnOverlayRemove);
    }

    public override void SendMessageToGods(ProtoId<OE14ReligionPrototype> religion, string msg, EntityUid source) { }

    public override void Shutdown()
    {
        base.Shutdown();
        _overlayMgr.RemoveOverlay<OE14ReligionVisionOverlay>();
    }

    private void OnPlayerAttached(Entity<OE14ReligionVisionComponent> ent, ref LocalPlayerAttachedEvent args)
    {
        AddOverlay();
    }

    private void OnPlayerDetached(Entity<OE14ReligionVisionComponent> ent, ref LocalPlayerDetachedEvent args)
    {
        RemoveOverlay();
    }

    private void OnOverlayInit(Entity<OE14ReligionVisionComponent> ent, ref ComponentInit args)
    {
        var attachedEnt = _player.LocalEntity;

        if (attachedEnt != ent.Owner)
            return;

        AddOverlay();
    }

    private void OnOverlayRemove(Entity<OE14ReligionVisionComponent> ent, ref ComponentRemove args)
    {
        var attachedEnt = _player.LocalEntity;

        if (attachedEnt != ent.Owner)
            return;

        RemoveOverlay();
    }

    private void AddOverlay()
    {
        if (_overlay != null)
            return;

        _overlay = new OE14ReligionVisionOverlay();
        _overlayMgr.AddOverlay(_overlay);
    }

    private void RemoveOverlay()
    {
        if (_overlay == null)
            return;

        _overlayMgr.RemoveOverlay(_overlay);
        _overlay = null;
    }
}
