using System.Linq;
using Content.Shared._OE14.Actions.Components;
using Content.Shared.Examine;
using Content.Shared.Mobs;

namespace Content.Shared._OE14.Actions;

public abstract partial class OE14SharedActionSystem
{
    private void InitializeExamine()
    {
        SubscribeLocalEvent<OE14ActionManaCostComponent, ExaminedEvent>(OnManacostExamined);
        SubscribeLocalEvent<OE14ActionStaminaCostComponent, ExaminedEvent>(OnStaminaCostExamined);
        SubscribeLocalEvent<OE14ActionSkillPointCostComponent, ExaminedEvent>(OnSkillPointCostExamined);

        SubscribeLocalEvent<OE14ActionSpeakingComponent, ExaminedEvent>(OnVerbalExamined);
        SubscribeLocalEvent<OE14ActionFreeHandsRequiredComponent, ExaminedEvent>(OnSomaticExamined);
        SubscribeLocalEvent<OE14ActionMaterialCostComponent, ExaminedEvent>(OnMaterialExamined);
        SubscribeLocalEvent<OE14ActionRequiredMusicToolComponent, ExaminedEvent>(OnMusicExamined);
        SubscribeLocalEvent<OE14ActionTargetMobStatusRequiredComponent, ExaminedEvent>(OnMobStateExamined);
    }

    private void OnManacostExamined(Entity<OE14ActionManaCostComponent> ent, ref ExaminedEvent args)
    {
        args.PushMarkup($"{Loc.GetString("oe14-magic-manacost")}: [color=#5da9e8]{ent.Comp.ManaCost}[/color]", priority: 9);
    }

    private void OnStaminaCostExamined(Entity<OE14ActionStaminaCostComponent> ent, ref ExaminedEvent args)
    {
        args.PushMarkup($"{Loc.GetString("oe14-magic-staminacost")}: [color=#3fba54]{ent.Comp.Stamina}[/color]", priority: 9);
    }

    private void OnSkillPointCostExamined(Entity<OE14ActionSkillPointCostComponent> ent, ref ExaminedEvent args)
    {
        if (!_proto.TryIndex(ent.Comp.SkillPoint, out var indexedSkillPoint))
            return;

        args.PushMarkup($"{Loc.GetString("oe14-magic-skillpointcost", ("name", Loc.GetString(indexedSkillPoint.Name)), ("count", ent.Comp.Count))}", priority: 9);
    }

    private void OnVerbalExamined(Entity<OE14ActionSpeakingComponent> ent, ref ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString("oe14-magic-verbal-aspect"), 8);
    }

    private void OnSomaticExamined(Entity<OE14ActionFreeHandsRequiredComponent> ent, ref ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString("oe14-magic-somatic-aspect") + " " + ent.Comp.FreeHandRequired, 8);
    }

    private void OnMaterialExamined(Entity<OE14ActionMaterialCostComponent> ent, ref ExaminedEvent args)
    {
        if (ent.Comp.Requirement is not null)
            args.PushMarkup(Loc.GetString("oe14-magic-material-aspect") + " " + ent.Comp.Requirement.GetRequirementTitle(_proto));
    }
    private void OnMusicExamined(Entity<OE14ActionRequiredMusicToolComponent> ent, ref ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString("oe14-magic-music-aspect"));
    }

    private void OnMobStateExamined(Entity<OE14ActionTargetMobStatusRequiredComponent> ent, ref ExaminedEvent args)
    {
        var states = string.Join(", ",
            ent.Comp.AllowedStates.Select(state => state switch
        {
            MobState.Alive => Loc.GetString("oe14-magic-spell-target-mob-state-live"),
            MobState.Dead => Loc.GetString("oe14-magic-spell-target-mob-state-dead"),
            MobState.Critical => Loc.GetString("oe14-magic-spell-target-mob-state-critical")
        }));

        args.PushMarkup(Loc.GetString("oe14-magic-spell-target-mob-state", ("state", states)));
    }
}
