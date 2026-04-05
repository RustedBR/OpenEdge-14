using Robust.Shared.Prototypes;

namespace Content.Server._OE14.GameTicking.Rules.Components;

/// <summary>
///When attached to shuttle, start firebombing it until FTL ends.
/// </summary>
[RegisterComponent, Access(typeof(OE14CrashingShipRule))]
public sealed partial class OE14CrashingShipComponent : Component
{
    [DataField]
    public TimeSpan NextExplosionTime = TimeSpan.Zero;

    [DataField]
    public EntProtoId ExplosionProto = "OE14ShipExplosion";

    [DataField]
    public EntProtoId FinalExplosionProto = "OE14ShipExplosionBig";
}
