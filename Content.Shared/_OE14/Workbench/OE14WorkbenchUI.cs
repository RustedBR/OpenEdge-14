/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Shared._OE14.Workbench.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._OE14.Workbench;

[Serializable, NetSerializable]
public enum OE14WorkbenchUiKey
{
    Key,
}

[Serializable, NetSerializable]
public sealed class OE14WorkbenchUiCraftMessage(ProtoId<OE14WorkbenchRecipePrototype> recipe)
    : BoundUserInterfaceMessage
{
    public readonly ProtoId<OE14WorkbenchRecipePrototype> Recipe = recipe;
}


[Serializable, NetSerializable]
public sealed class OE14WorkbenchUiRecipesState(List<OE14WorkbenchUiRecipesEntry> recipes) : BoundUserInterfaceState
{
    public readonly List<OE14WorkbenchUiRecipesEntry> Recipes = recipes;
}

[Serializable, NetSerializable]
public readonly struct OE14WorkbenchUiRecipesEntry(ProtoId<OE14WorkbenchRecipePrototype> protoId, bool craftable)
    : IEquatable<OE14WorkbenchUiRecipesEntry>
{
    public readonly ProtoId<OE14WorkbenchRecipePrototype> ProtoId = protoId;
    public readonly bool Craftable = craftable;

    public int CompareTo(OE14WorkbenchUiRecipesEntry other)
    {
        return Craftable.CompareTo(other.Craftable);
    }

    public override bool Equals(object? obj)
    {
        return obj is OE14WorkbenchUiRecipesEntry other && Equals(other);
    }

    public bool Equals(OE14WorkbenchUiRecipesEntry other)
    {
        return ProtoId.Id == other.ProtoId.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ProtoId, Craftable);
    }

    public override string ToString()
    {
        return $"{ProtoId} ({Craftable})";
    }

    public static int CompareTo(OE14WorkbenchUiRecipesEntry left, OE14WorkbenchUiRecipesEntry right)
    {
        return right.CompareTo(left);
    }
}
