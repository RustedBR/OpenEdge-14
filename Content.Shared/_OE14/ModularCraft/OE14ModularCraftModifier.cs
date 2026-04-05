using Content.Shared._OE14.ModularCraft.Components;
using JetBrains.Annotations;

namespace Content.Shared._OE14.ModularCraft;

[ImplicitDataDefinitionForInheritors]
[MeansImplicitUse]
public abstract partial class OE14ModularCraftModifier
{
    public abstract void Effect(EntityManager entManager, Entity<OE14ModularCraftStartPointComponent> start, Entity<OE14ModularCraftPartComponent>? part);
}
