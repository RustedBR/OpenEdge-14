using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server._OE14.Salary;

/// <summary>
/// Pays out the salary upon interaction, if it has accumulated for the player.
/// </summary>
[RegisterComponent, Access(typeof(OE14SalarySystem))]
public sealed partial class OE14SalaryPairollComponent : Component
{
    [DataField]
    public SoundSpecifier BuySound = new SoundPathSpecifier("/Audio/_OE14/Effects/cash.ogg")
    {
        Params = AudioParams.Default.WithVariation(0.1f),
    };

    [DataField]
    public EntProtoId BuyVisual = "OE14CashImpact";
}
