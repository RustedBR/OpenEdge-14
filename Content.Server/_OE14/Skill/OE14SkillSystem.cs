using Content.Shared._OE14.Skill;
using Content.Shared._OE14.Skill.Components;

namespace Content.Server._OE14.Skill;

public sealed partial class OE14SkillSystem : OE14SharedSkillSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<OE14TryLearnSkillMessage>(OnClientRequestLearnSkill);
    }

    private void OnClientRequestLearnSkill(OE14TryLearnSkillMessage ev, EntitySessionEventArgs args)
    {
        var entity = GetEntity(ev.Entity);

        if (args.SenderSession.AttachedEntity != entity)
            return;

        TryLearnSkill(entity, ev.Skill);
    }
}
