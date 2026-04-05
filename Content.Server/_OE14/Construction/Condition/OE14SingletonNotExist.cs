using Content.Shared._OE14.UniqueLoot;
using Content.Shared.Construction;
using Content.Shared.Examine;
using JetBrains.Annotations;

namespace Content.Server._OE14.Construction.Condition;

[UsedImplicitly]
[DataDefinition]
public sealed partial class OE14SingletonNotExist : IGraphCondition
{
    [DataField(required: true)]
    public string Key = string.Empty;

    public bool Condition(EntityUid craft, IEntityManager entityManager)
    {
        var query = entityManager.EntityQueryEnumerator<OE14SingletonComponent>();
        while (query.MoveNext(out var uid, out var singleton))
        {
            if (singleton.Key == Key)
                return false;
        }

        return true;
    }

    public bool DoExamine(ExaminedEvent args)
    {
        if (Condition(args.Examined, IoCManager.Resolve<IEntityManager>()))
            return false;

        args.PushMarkup(Loc.GetString("oe14-construction-condition-singleton"));
        return true;
    }

    public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
    {
        yield return new ConstructionGuideEntry()
        {
            Localization = "oe14-construction-condition-singleton",
        };
    }
}
