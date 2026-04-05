using System.Numerics;
using Content.Shared._OE14.Procedural.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._OE14.Demiplane;

[Serializable, NetSerializable]
public enum OE14DemiplaneMapUiKey
{
    Key,
}

[Serializable, NetSerializable]
public sealed class OE14DemiplaneMapUiState(Dictionary<Vector2i, OE14DemiplaneMapNode> nodes, HashSet<(Vector2i, Vector2i)>? edges = null) : BoundUserInterfaceState
{
    public Dictionary<Vector2i, OE14DemiplaneMapNode> Nodes = nodes;
    public HashSet<(Vector2i, Vector2i)> Edges = edges ?? new();
}

[Serializable, NetSerializable]
public sealed class OE14DemiplaneMapNode(Vector2 uiPosition, ProtoId<OE14ProceduralLocationPrototype>? locationConfig = null, List<ProtoId<OE14ProceduralModifierPrototype>>? modifiers = null)
{
    public bool Start = false;
    public Vector2 UiPosition = uiPosition;
    public int Level = 0;

    public bool Opened = false;

    public ProtoId<OE14ProceduralLocationPrototype>? LocationConfig = locationConfig;
    public List<ProtoId<OE14ProceduralModifierPrototype>> Modifiers = modifiers ?? [];
}
