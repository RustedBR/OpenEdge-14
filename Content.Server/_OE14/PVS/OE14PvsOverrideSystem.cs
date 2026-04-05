using Robust.Server.GameStates;

namespace Content.Server._OE14.PVS;

public sealed partial class OE14PvsOverrideSystem : EntitySystem
{
    [Dependency] private readonly PvsOverrideSystem _pvs = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<OE14PvsOverrideComponent, ComponentStartup>(OnLighthouseStartup);
        SubscribeLocalEvent<OE14PvsOverrideComponent, ComponentShutdown>(OnLighthouseShutdown);
    }

    private void OnLighthouseShutdown(Entity<OE14PvsOverrideComponent> ent, ref ComponentShutdown args)
    {
        _pvs.RemoveGlobalOverride(ent);
    }

    private void OnLighthouseStartup(Entity<OE14PvsOverrideComponent> ent, ref ComponentStartup args)
    {
        _pvs.AddGlobalOverride(ent);
    }
}
