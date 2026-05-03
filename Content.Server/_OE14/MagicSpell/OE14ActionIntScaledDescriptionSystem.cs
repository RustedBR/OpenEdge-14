using Content.Shared._OE14.CharacterStats;
using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared._OE14.MagicSpell.Components;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;

namespace Content.Server._OE14.MagicSpell;

/// <summary>
/// Updates action descriptions that include INT-scaled values whenever the
/// owning player's Intelligence stat changes or when new actions are granted.
/// </summary>
public sealed partial class OE14ActionIntScaledDescriptionSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _meta = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14CharacterStatsComponent, OE14StatsUpdatedEvent>(OnStatsUpdated);
        SubscribeLocalEvent<OE14CharacterStatsComponent, ActionAddedEvent>(OnActionAdded);
    }

    private void OnStatsUpdated(EntityUid uid, OE14CharacterStatsComponent stats, OE14StatsUpdatedEvent args)
    {
        UpdateAllActionDescriptions(uid, stats);
    }

    private void OnActionAdded(EntityUid uid, OE14CharacterStatsComponent stats, ActionAddedEvent args)
    {
        if (!TryComp<OE14ActionIntScaledDescriptionComponent>(args.Action, out var descComp))
            return;

        UpdateActionDescription(args.Action, descComp, stats);
    }

    // ─── Helpers ───────────────────────────────────────────────────────────────

    private void UpdateAllActionDescriptions(EntityUid player, OE14CharacterStatsComponent stats)
    {
        if (!TryComp<ActionsComponent>(player, out var actionsComp))
            return;

        foreach (var actionUid in actionsComp.Actions)
        {
            if (!TryComp<OE14ActionIntScaledDescriptionComponent>(actionUid, out var descComp))
                continue;

            UpdateActionDescription(actionUid, descComp, stats);
        }
    }

    private void UpdateActionDescription(EntityUid actionUid,
        OE14ActionIntScaledDescriptionComponent comp,
        OE14CharacterStatsComponent stats)
    {
        var effInt = Math.Clamp(
            stats.Intelligence + stats.IntelligenceModifier,
            1,
            stats.MaxStatValue);

        var multiplier = effInt >= 5
            ? 1.0f + (effInt - 5) * 0.10f
            : 1.0f + (effInt - 5) * 0.125f;

        var sb = new System.Text.StringBuilder(comp.BaseDescription);

        if (comp.ScaleEntries.Count > 0)
        {
            sb.Append('\n');

            for (var i = 0; i < comp.ScaleEntries.Count; i++)
            {
                if (i > 0)
                    sb.Append(" | ");

                var entry = comp.ScaleEntries[i];

                float currentValue;
                if (entry.IsAdditive)
                    currentValue = entry.BaseValue + effInt * entry.IntBonus;
                else
                    currentValue = entry.BaseValue * multiplier;

                var formatted = entry.Decimals == 0
                    ? ((int) Math.Floor(currentValue)).ToString()
                    : currentValue.ToString("F" + entry.Decimals);

                sb.Append(entry.Label);
                sb.Append(": ");
                sb.Append(formatted);
                sb.Append(entry.Unit);
            }
        }

        _meta.SetEntityDescription(actionUid, sb.ToString());
    }
}
