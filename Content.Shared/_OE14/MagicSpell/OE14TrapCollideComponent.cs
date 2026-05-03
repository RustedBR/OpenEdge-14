using Content.Shared._OE14.MagicSpell.Spells;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;

namespace Content.Shared._OE14.MagicSpell;

/// <summary>
/// Replaces TriggerOnCollide + DeleteOnTrigger + SpawnOnTrigger for magic traps.
/// Supports owner immunity and INT-scaled damage (via OE14SpellApplyDamageScaled with scaleWithCasterInt).
/// When triggered, passes Owner as User in spell effect args — OE14SpellArea with affectCaster: false
/// will naturally skip the owner.
/// </summary>
[RegisterComponent]
public sealed partial class OE14TrapCollideComponent : Component
{
    [DataField(serverOnly: true)]
    public List<OE14SpellEffect> Effects = new();

    [DataField]
    public EntityWhitelist? Whitelist;

    [DataField]
    public SoundSpecifier? TriggerSound;

    /// <summary>Set at runtime by OE14SpellSpawnTrapEntityOnTarget to the spell caster.</summary>
    public EntityUid? Owner;
}
