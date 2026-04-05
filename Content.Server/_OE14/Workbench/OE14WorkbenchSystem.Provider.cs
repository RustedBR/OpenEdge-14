using Content.Shared.Placeable;

namespace Content.Server._OE14.Workbench;

public sealed partial class OE14WorkbenchSystem
{
    private void InitProviders()
    {
        SubscribeLocalEvent<OE14WorkbenchPlaceableProviderComponent, OE14WorkbenchGetResourcesEvent>(OnGetResource);
    }

    private void OnGetResource(Entity<OE14WorkbenchPlaceableProviderComponent> ent, ref OE14WorkbenchGetResourcesEvent args)
    {
        if (!TryComp<ItemPlacerComponent>(ent, out var placer))
            return;

        args.AddResources(placer.PlacedEntities);
    }
}

public sealed class OE14WorkbenchGetResourcesEvent : EntityEventArgs
{
    public HashSet<EntityUid> Resources { get; private set; } = new();

    public void AddResource(EntityUid resource)
    {
        Resources.Add(resource);
    }

    public void AddResources(IEnumerable<EntityUid> resources)
    {
        foreach (var resource in resources)
        {
            Resources.Add(resource);
        }
    }
}
