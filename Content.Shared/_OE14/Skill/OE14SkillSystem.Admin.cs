using System.Linq;
using Content.Shared._OE14.Skill.Components;
using Content.Shared._OE14.Skill.Prototypes;
using Content.Shared.Administration;
using Content.Shared.Administration.Managers;
using Content.Shared.FixedPoint;
using Content.Shared.Verbs;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._OE14.Skill;

public abstract partial class OE14SharedSkillSystem
{
    [Dependency] private readonly ISharedAdminManager _admin = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    private IEnumerable<OE14SkillPrototype>? _allSkills;
    private IEnumerable<OE14SkillTreePrototype>? _allTrees;
    private void InitializeAdmin()
    {
        SubscribeLocalEvent<OE14SkillStorageComponent, GetVerbsEvent<Verb>>(OnGetAdminVerbs);

        SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnPrototypeReloaded);

        UpdateCachedSkill();
    }

    private void OnPrototypeReloaded(PrototypesReloadedEventArgs ev)
    {
        if (!ev.WasModified<OE14SkillPrototype>())
            return;

        UpdateCachedSkill();
    }

    private void UpdateCachedSkill()
    {
        _allSkills = _proto.EnumeratePrototypes<OE14SkillPrototype>();
        _allTrees = _proto.EnumeratePrototypes<OE14SkillTreePrototype>();
    }


    private void OnGetAdminVerbs(Entity<OE14SkillStorageComponent> ent, ref GetVerbsEvent<Verb> args)
    {
        if (!_admin.HasAdminFlag(args.User, AdminFlags.Admin))
            return;

        if (_allSkills is null || _allTrees is null)
            return;

        var target = args.Target;

        //Reset/Remove All Skills
        args.Verbs.Add(new Verb
        {
            Text = "Reset skills",
            Message = "Remove all learned skills",
            Category = VerbCategory.Debug,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/_OE14/Interface/Misc/reroll.rsi"), "reroll"),
            Act = () =>
            {
                TryResetSkills(target);
            },
        });
    }
}
