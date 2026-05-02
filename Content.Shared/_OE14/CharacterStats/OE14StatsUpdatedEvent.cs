namespace Content.Shared._OE14.CharacterStats;

/// <summary>
/// Raised as a directed local event on the player entity whenever their character
/// stats are recalculated (on init, startup, or after any modifier change).
/// Used by systems that need to react to stat changes, e.g. updating spell descriptions.
/// </summary>
public sealed class OE14StatsUpdatedEvent : EntityEventArgs { }
