using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests._OE14;

#nullable enable

[TestFixture]
public sealed class OE14EntityTest
{
    [Test]
    public async Task CheckAllOE14EntityHasForkFilteredCategory()
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;

        var compFactory = server.ResolveDependency<IComponentFactory>();
        var protoManager = server.ResolveDependency<IPrototypeManager>();

        await server.WaitAssertion(() =>
        {
            Assert.Multiple(() =>
            {
                if (!protoManager.TryIndex<EntityCategoryPrototype>("ForkFiltered", out var indexedFilter))
                    return;

                foreach (var proto in protoManager.EnumeratePrototypes<EntityPrototype>())
                {
                    if (!proto.ID.StartsWith("OE14"))
                        continue;

                    if (proto.Abstract || proto.HideSpawnMenu)
                        continue;

                    Assert.That(proto.Categories.Contains(indexedFilter), $"OE14 fork proto: {proto} does not marked abstract, or have a HideSpawnMenu or ForkFiltered category");
                }
            });
        });
        await pair.CleanReturnAsync();
    }
}
