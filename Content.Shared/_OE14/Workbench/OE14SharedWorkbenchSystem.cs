/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Shared._OE14.Workbench.Prototypes;
using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._OE14.Workbench;

public abstract class OE14SharedWorkbenchSystem : EntitySystem
{
}

[Serializable, NetSerializable]
public sealed partial class OE14CraftDoAfterEvent : DoAfterEvent
{
    [DataField(required: true)]
    public ProtoId<OE14WorkbenchRecipePrototype> Recipe = default!;

    public override DoAfterEvent Clone() => this;
}
