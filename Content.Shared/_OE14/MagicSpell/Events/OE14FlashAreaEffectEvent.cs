namespace Content.Shared._OE14.MagicSpell.Events;

/// <summary>
/// Raised on the caster entity when OE14SpellFlashArea executes.
/// Handled server-side by OE14MagicSystem to call FlashSystem.FlashArea.
/// </summary>
[ByRefEvent]
public record struct OE14FlashAreaEffectEvent(EntityUid Source, EntityUid? Instigator, float Range, TimeSpan Duration, float SlowTo);
