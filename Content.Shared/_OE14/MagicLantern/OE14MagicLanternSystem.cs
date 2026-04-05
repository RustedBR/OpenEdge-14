using Content.Shared._OE14.MagicEnergy.Components;

namespace Content.Shared._OE14.MagicLantern;

public partial class OE14MagicLanternSystem : EntitySystem
{

    [Dependency] private readonly SharedPointLightSystem _pointLight = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14MagicLanternComponent, OE14SlotCrystalPowerChangedEvent>(OnSlotPowerChanged);
    }

    private void OnSlotPowerChanged(Entity<OE14MagicLanternComponent> ent, ref OE14SlotCrystalPowerChangedEvent args)
    {
        SharedPointLightComponent? pointLight = null;
        if (_pointLight.ResolveLight(ent, ref pointLight))
        {
            _pointLight.SetEnabled(ent, args.Powered, pointLight);
        }
    }
}
