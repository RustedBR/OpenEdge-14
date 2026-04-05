using Content.Client.Items;
using Content.Shared._OE14.MagicEnergy;
using Content.Shared._OE14.MagicEnergy.Components;

namespace Content.Client._OE14.MagicEnergy;

public sealed class OE14MagicEnergySystem : OE14SharedMagicEnergySystem
{
    public override void Initialize()
    {
        base.Initialize();

        Subs.ItemStatus<OE14MagicEnergyExaminableComponent>( ent => new OE14MagicEnergyStatusControl(ent));
    }
}
