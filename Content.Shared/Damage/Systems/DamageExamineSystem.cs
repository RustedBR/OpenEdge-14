using Content.Shared.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Damage.Systems;

public sealed class DamageExamineSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DamageExaminableComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(EntityUid uid, DamageExaminableComponent component, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        var ev = new DamageExamineEvent(new FormattedMessage(), args.Examiner);
        RaiseLocalEvent(uid, ref ev);
        if (!ev.Message.IsEmpty)
        {
            args.PushMessage(ev.Message, -1);
        }
    }

    public void AddDamageExamine(FormattedMessage message, DamageSpecifier damageSpecifier, string? type = null)
    {
        var markup = GetDamageExamine(damageSpecifier, type);
        if (!message.IsEmpty)
        {
            message.PushNewline();
        }
        message.AddMessage(markup);
    }

    /// <summary>
    /// Retrieves the damage examine values.
    /// </summary>
    private FormattedMessage GetDamageExamine(DamageSpecifier damageSpecifier, string? type = null)
    {
        var msg = new FormattedMessage();

        if (string.IsNullOrEmpty(type))
        {
            msg.AddMarkupOrThrow(Loc.GetString("damage-examine"));
        }
        else
        {
            if (damageSpecifier.GetTotal() == FixedPoint2.Zero && !damageSpecifier.AnyPositive())
            {
                msg.AddMarkupOrThrow(Loc.GetString("damage-none"));
                return msg;
            }

            msg.AddMarkupOrThrow(Loc.GetString("damage-examine-type", ("type", type)));
        }

        foreach (var damage in damageSpecifier.DamageDict)
        {
            if (damage.Value != FixedPoint2.Zero)
            {
                msg.PushNewline();
                msg.AddMarkupOrThrow(Loc.GetString("damage-value", ("type", _prototype.Index<DamageTypePrototype>(damage.Key).LocalizedName), ("amount", damage.Value)));
            }
        }

        return msg;
    }
}
