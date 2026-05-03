using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared._OE14.MagicSpell.Events;
using Robust.Shared.Network;

namespace Content.Shared._OE14.MagicSpell.Spells;

/// <summary>
/// Spell effect that flashes all entities in a radius around the target position.
/// Range scales with caster INT: BaseRange + ceil(effInt / IntDivisor).
/// Actual flash execution is handled server-side via OE14FlashAreaEffectEvent.
/// </summary>
public sealed partial class OE14SpellFlashArea : OE14SpellEffect
{
    [DataField]
    public float BaseRange = 4f;

    /// <summary>
    /// Divides effInt to get the bonus range added to BaseRange (ceiling applied).
    /// E.g. IntDivisor = 3 → INT 1 adds ceil(1/3)=1, INT 5 adds ceil(5/3)=2, INT 10 adds ceil(10/3)=4.
    /// </summary>
    [DataField]
    public float IntDivisor = 3f;

    [DataField]
    public TimeSpan FlashDuration = TimeSpan.FromSeconds(4);

    [DataField]
    public float SlowTo = 0.8f;

    public override void Effect(EntityManager entManager, OE14SpellEffectBaseArgs args)
    {
        var net = IoCManager.Resolve<INetManager>();
        if (net.IsClient || args.User is null)
            return;

        var effectiveRange = BaseRange;

        if (entManager.TryGetComponent<OE14CharacterStatsComponent>(args.User.Value, out var stats))
        {
            var effInt = Math.Clamp(stats.Intelligence + stats.IntelligenceModifier, 1, stats.MaxStatValue);
            effectiveRange += MathF.Ceiling(effInt / IntDivisor);
        }

        var ev = new OE14FlashAreaEffectEvent(args.User.Value, args.User, effectiveRange, FlashDuration, SlowTo);
        entManager.EventBus.RaiseLocalEvent(args.User.Value, ref ev);
    }
}
