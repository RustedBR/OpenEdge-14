using Content.Shared.DoAfter;
using Content.Shared.Maps;
using Robust.Shared.Audio;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._OE14.TileEditTool;

[RegisterComponent, Access(typeof(OE14EditTileToolSystem))]
public sealed partial class OE14EditTileToolComponent : Component
{
    [DataField]
    public TimeSpan Delay = TimeSpan.FromSeconds(1f);

    [DataField]
    public SoundSpecifier? Sound;

    [DataField]
    public Dictionary<ProtoId<ContentTileDefinition>, ProtoId<ContentTileDefinition>> TileReplace = new();
}

[Serializable, NetSerializable]
public sealed partial class OE14TileToolReplaceDoAfter : DoAfterEvent
{
    [DataField(required:true)]
    public NetCoordinates Coordinates;

    public OE14TileToolReplaceDoAfter(NetCoordinates coordinates)
    {
        Coordinates = coordinates;
    }

    public override DoAfterEvent Clone() => this;
}
