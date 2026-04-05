/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Robust.Shared.Prototypes;

namespace Content.Shared._OE14.Cooking.Prototypes;

[Prototype("OE14FoodType")]
public sealed class OE14FoodTypePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;
}
