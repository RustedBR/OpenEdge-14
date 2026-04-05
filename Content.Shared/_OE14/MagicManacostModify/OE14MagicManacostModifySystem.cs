
using Content.Shared._OE14.MagicRitual.Prototypes;
using Content.Shared._OE14.MagicSpell.Events;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Verbs;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._OE14.MagicManacostModify;

public sealed partial class OE14MagicManacostModifySystem : EntitySystem
{
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14MagicManacostModifyComponent, InventoryRelayedEvent<OE14CalculateManacostEvent>>(OnCalculateManacost);
        SubscribeLocalEvent<OE14MagicManacostModifyComponent, OE14CalculateManacostEvent>(OnCalculateManacost);
        SubscribeLocalEvent<OE14MagicManacostModifyComponent, GetVerbsEvent<ExamineVerb>>(OnVerbExamine);
    }

    private void OnVerbExamine(Entity<OE14MagicManacostModifyComponent> ent, ref GetVerbsEvent<ExamineVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess || !ent.Comp.Examinable)
            return;

        var markup = GetManacostModifyMessage(ent.Comp.GlobalModifier);
        _examine.AddDetailedExamineVerb(
            args,
            ent.Comp,
            markup,
            Loc.GetString("oe14-magic-examinable-verb-text"),
            "/Textures/Interface/VerbIcons/bubbles.svg.192dpi.png",
            Loc.GetString("oe14-magic-examinable-verb-message"));
    }

    public FormattedMessage GetManacostModifyMessage(FixedPoint2 global)
    {
        var msg = new FormattedMessage();
        msg.AddMarkupOrThrow(Loc.GetString("oe14-clothing-magic-examine"));

        if (global != 1)
        {
            msg.PushNewline();

            var plus = (float)global > 1 ? "+" : "";
            msg.AddMarkupOrThrow(
                $"{Loc.GetString("oe14-clothing-magic-global")}: {plus}{MathF.Round((float)(global - 1) * 100, MidpointRounding.AwayFromZero)}%");
        }

        return msg;
    }

    private void OnCalculateManacost(Entity<OE14MagicManacostModifyComponent> ent, ref InventoryRelayedEvent<OE14CalculateManacostEvent> args)
    {
        OnCalculateManacost(ent, ref args.Args);
    }

    private void OnCalculateManacost(Entity<OE14MagicManacostModifyComponent> ent, ref OE14CalculateManacostEvent args)
    {
        args.Multiplier *= (float)ent.Comp.GlobalModifier;
    }
}
