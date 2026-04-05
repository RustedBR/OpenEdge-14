using Content.Shared._OE14.MagicEnergy.Components;
using Content.Shared.Construction;
using Content.Shared.Examine;
using JetBrains.Annotations;
using Robust.Shared.Utility;

namespace Content.Server._OE14.Construction.Condition;

/// <summary>
///     Makes the condition fail if any entities on a tile have (or not) a component.
/// </summary>
[UsedImplicitly]
[DataDefinition]
public sealed partial class OE14ManaFilled : IGraphCondition
{
    public bool Condition(EntityUid uid, IEntityManager entityManager)
    {
        if (!entityManager.TryGetComponent(uid, out OE14MagicEnergyContainerComponent? container))
            return true;

        return container.Energy >= container.MaxEnergy;
    }

    public bool DoExamine(ExaminedEvent args)
    {
        if (Condition(args.Examined, IoCManager.Resolve<IEntityManager>()))
            return false;

        args.PushMarkup(Loc.GetString("oe14-construction-condition-mana-filled"));
        return true;
    }

    public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
    {
        yield return new ConstructionGuideEntry()
        {
            Localization = "oe14-construction-condition-mana-filled",
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/_OE14/Actions/Spells/meta.rsi"), "mana"),
        };
    }
}
