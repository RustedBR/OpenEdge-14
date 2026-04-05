using Content.Shared._OE14.Religion.Components;
using Content.Shared._OE14.Religion.Systems;

namespace Content.Shared._OE14.MagicSpell.Spells;

public sealed partial class OE14SpellSendMessageToGod : OE14SpellEffect
{
    [DataField]
    public LocId? Message;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        if (!entManager.TryGetComponent<OE14ReligionFollowerComponent>(args.User, out var follower))
            return;
        if (!entManager.TryGetComponent<MetaDataComponent>(args.User, out var metaData))
            return;

        if (follower.Religion is null)
            return;

        var religionSys = entManager.System<OE14SharedReligionGodSystem>();

        religionSys.SendMessageToGods(follower.Religion.Value, Loc.GetString("oe14-call-follower-message", ("name", metaData.EntityName)) + " " + Loc.GetString(Message?? ""), args.User.Value);
    }
}
