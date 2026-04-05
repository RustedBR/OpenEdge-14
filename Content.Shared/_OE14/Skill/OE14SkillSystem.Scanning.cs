using System.Text;
using Content.Shared._OE14.Skill.Components;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Shared._OE14.Skill;

public abstract partial class OE14SharedSkillSystem
{
    [Dependency] private readonly ExamineSystemShared _examine = default!;

    private void InitializeScanning()
    {
        SubscribeLocalEvent<OE14SkillScannerComponent, OE14SkillScanEvent>(OnSkillScan);
        SubscribeLocalEvent<OE14SkillScannerComponent, InventoryRelayedEvent<OE14SkillScanEvent>>((e, c, ev) => OnSkillScan(e, c, ev.Args));

        SubscribeLocalEvent<OE14SkillStorageComponent, GetVerbsEvent<ExamineVerb>>(OnExamined);
    }

    private void OnExamined(Entity<OE14SkillStorageComponent> ent, ref GetVerbsEvent<ExamineVerb> args)
    {
        var scanEvent = new OE14SkillScanEvent();
        RaiseLocalEvent(args.User, scanEvent);

        if (!scanEvent.CanScan)
            return;

        var markup = GetSkillExamine(ent);

        _examine.AddDetailedExamineVerb(
            args,
            ent.Comp,
            markup,
            Loc.GetString("oe14-skill-info-title"),
            "/Textures/Interface/students-cap.svg.192dpi.png");
    }

    private FormattedMessage GetSkillExamine(Entity<OE14SkillStorageComponent> ent)
    {
        var msg = new FormattedMessage();

        var sb = new StringBuilder();

        sb.Append(Loc.GetString("oe14-skill-examine-title") + "\n");

        foreach (var skill in ent.Comp.LearnedSkills)
        {
            if (!_proto.TryIndex(skill, out var indexedSkill))
                continue;

            if(!_proto.TryIndex(indexedSkill.Tree, out var indexedTree))
                continue;

            var skillName = GetSkillName(skill);
            sb.Append($"• [color={indexedTree.Color.ToHex()}]{skillName}[/color]\n");
        }

        //sb.Append($"\n{Loc.GetString("oe14-skill-menu-level")} {ent.Comp.SkillsSumExperience}/{ent.Comp.ExperienceMaxCap}\n");
        msg.AddMarkupOrThrow(sb.ToString());
        return msg;
    }

    private void OnSkillScan(EntityUid uid, OE14SkillScannerComponent component, OE14SkillScanEvent args)
    {
        args.CanScan = true;
    }
}

public sealed class OE14SkillScanEvent : EntityEventArgs, IInventoryRelayEvent
{
    public bool CanScan;
    public SlotFlags TargetSlots { get; } = SlotFlags.EYES;
}
