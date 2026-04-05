using Content.Shared.Actions;

namespace Content.Shared._OE14.MagicSpell.Events;

public interface IOE14MagicEffect
{
    public TimeSpan Cooldown { get; }
}

public sealed partial class OE14WorldTargetActionEvent : WorldTargetActionEvent, IOE14MagicEffect
{
    [DataField]
    public TimeSpan Cooldown { get; private set; } = TimeSpan.FromSeconds(1f);
}

public sealed partial class OE14EntityTargetActionEvent : EntityTargetActionEvent, IOE14MagicEffect
{
    [DataField]
    public TimeSpan Cooldown { get; private set; } = TimeSpan.FromSeconds(1f);
}

public sealed partial class OE14InstantActionEvent : InstantActionEvent, IOE14MagicEffect
{
    [DataField]
    public TimeSpan Cooldown { get; private set; } = TimeSpan.FromSeconds(1f);
}

