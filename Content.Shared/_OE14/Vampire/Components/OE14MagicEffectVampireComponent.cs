using Content.Shared._OE14.MagicSpell;

namespace Content.Shared._OE14.Vampire.Components;

/// <summary>
/// Use is only available if the vampire is in a “visible” dangerous form.
/// </summary>
[RegisterComponent, Access(typeof(OE14SharedVampireSystem))]
public sealed partial class OE14MagicEffectVampireComponent : Component
{
}
