using Content.Shared._OE14.Procedural.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.GameTicking.Rules.Components;

/// <summary>
/// A rule that assigns common goals to different roles. Common objectives are generated once at the beginning of a round and are shared between players.
/// </summary>
[RegisterComponent, Access(typeof(OE14ExpeditionToWindlandsRule))]
public sealed partial class OE14ExpeditionToWindlandsRuleComponent : Component
{
    [DataField]
    public ProtoId<OE14ProceduralLocationPrototype> Location = "T1GrasslandIsland";

    [DataField]
    public List<ProtoId<OE14ProceduralModifierPrototype>> Modifiers = [];

    [DataField]
    public float FloatingTime = 120;
}
