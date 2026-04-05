using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;

namespace Content.Shared._OE14.Religion.Systems;

[Serializable, NetSerializable]
public enum OE14ReligionEntityUiKey
{
    Key,
}

[Serializable, NetSerializable]
public sealed class OE14ReligionEntityUiState(Dictionary<NetEntity, string> altars, Dictionary<NetEntity, string> followers, FixedPoint2 followerPercentage, FixedPoint2 manaPercentage) : BoundUserInterfaceState
{
    public Dictionary<NetEntity, string> Altars = altars;
    public Dictionary<NetEntity, string> Followers = followers;
    public FixedPoint2 FollowerPercentage = followerPercentage;
    public FixedPoint2 ManaPercentage = manaPercentage;
}

[Serializable, NetSerializable]
public sealed class OE14ReligionEntityTeleportAttempt(NetEntity entity) : BoundUserInterfaceMessage
{
    public readonly NetEntity Entity = entity;
}
