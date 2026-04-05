using Content.Shared._OE14.Skill;
using Content.Shared._OE14.Skill.Components;
using Content.Shared._OE14.Skill.Prototypes;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Prototypes;

namespace Content.Client._OE14.Skill;

public sealed partial class OE14ClientSkillSystem : OE14SharedSkillSystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public event Action<EntityUid>? OnSkillUpdate;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14SkillStorageComponent, AfterAutoHandleStateEvent>(OnAfterAutoHandleState);
    }

    private void OnAfterAutoHandleState(Entity<OE14SkillStorageComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        if (ent != _playerManager.LocalEntity)
            return;

        OnSkillUpdate?.Invoke(ent.Owner);
    }

    public void RequestSkillData()
    {
        var localPlayer = _playerManager.LocalEntity;

        if (!HasComp<OE14SkillStorageComponent>(localPlayer))
            return;

        OnSkillUpdate?.Invoke(localPlayer.Value);
    }

    public void RequestLearnSkill(EntityUid? target, OE14SkillPrototype? skill)
    {
        if (skill == null || target == null)
            return;

        var netEv = new OE14TryLearnSkillMessage(GetNetEntity(target.Value), skill.ID);
        RaiseNetworkEvent(netEv);

        if (_proto.TryIndex(skill.Tree, out var indexedTree))
        {
            _audio.PlayGlobal(indexedTree.LearnSound, target.Value, AudioParams.Default.WithVolume(6f));
        }
    }
}
