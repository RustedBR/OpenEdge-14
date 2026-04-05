using Content.Shared._OE14.ModularCraft.Components;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Labels.EntitySystems;
using Content.Shared.Interaction.Events;
using Robust.Shared.Serialization;

namespace Content.Shared._OE14.ModularCraft;

public abstract class OE14SharedModularCraftSystem : EntitySystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly LabelSystem _label = default!;
    [Dependency] private readonly MetaDataSystem _meta = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14ModularCraftStartPointComponent, InteractUsingEvent>(OnInteractUsingStart);
        SubscribeLocalEvent<OE14ModularCraftPartComponent, InteractUsingEvent>(OnInteractUsingPart);
        SubscribeLocalEvent<OE14LabeledRenamingComponent, OE14LabeledEvent>(OnLabelRenaming);
    }

    private void OnLabelRenaming(Entity<OE14LabeledRenamingComponent> ent, ref OE14LabeledEvent args)
    {
        if (args.Text is null)
            return;
        _meta.SetEntityName(ent, args.Text);
        _label.Label(ent, null);
    }

    private void OnInteractUsingStart(Entity<OE14ModularCraftStartPointComponent> start, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp<OE14ModularCraftPartComponent>(args.Used, out var part))
            return;

        _doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager,
            args.User,
            part.DoAfter,
            new OE14ModularCraftAddPartDoAfter() { User = GetNetEntity(args.User) },
            args.Used,
            args.Used,
            start)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            BreakOnDropItem = true,
        });

        args.Handled = true;
    }

    private void OnInteractUsingPart(Entity<OE14ModularCraftPartComponent> part, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (!HasComp<OE14ModularCraftStartPointComponent>(args.Target))
            return;

        _doAfter.TryStartDoAfter(new DoAfterArgs(EntityManager,
            args.User,
            part.Comp.DoAfter,
            new OE14ModularCraftAddPartDoAfter() { User = GetNetEntity(args.User) },
            args.Target,
            args.Target,
            part)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            BreakOnDropItem = true,
        });

        args.Handled = true;
    }
}

[Serializable, NetSerializable]
public sealed partial class OE14ModularCraftAddPartDoAfter : SimpleDoAfterEvent
{
    /// <summary>
    /// The user who initiated the crafting. Not networked - server-side only.
    /// </summary>
    [NonSerialized]
    public NetEntity? User;
}
