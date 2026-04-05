/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Shared.Chemistry.Components;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Cooking.Requirements;

public sealed partial class AlwaysMet : OE14CookingCraftRequirement
{
    public override bool CheckRequirement(IEntityManager entManager,
        IPrototypeManager protoManager,
        List<ProtoId<TagPrototype>> placedTags,
        Solution? solution = null)
    {
        return true;
    }

    public override float GetComplexity()
    {
        return 0;
    }
}
