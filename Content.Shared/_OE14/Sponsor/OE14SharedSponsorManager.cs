using System.Diagnostics.CodeAnalysis;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._OE14.Sponsor;

public interface IOE14SponsorManager
{
    public void Initialize();

    public bool UserHasFeature(NetUserId userId,
        ProtoId<OE14SponsorFeaturePrototype> feature,
        bool ifDisabledSponsorhip = true);

    public bool TryGetSponsorOOCColor(NetUserId userId, [NotNullWhen(true)] out Color? color);
}

public sealed class OE14SponsorRoleUpdate : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public ProtoId<OE14SponsorRolePrototype> Role { get; set; }

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        Role = buffer.ReadString();
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(Role);
    }
}
