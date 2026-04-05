using Robust.Shared.Serialization;

namespace Content.Shared._OE14.CharacterStats;

/// <summary>
/// Sent by the client to the server to request spending one available stat point.
/// The server validates the request and applies the change.
/// </summary>
[Serializable, NetSerializable]
public sealed class OE14SpendStatPointMessage(NetEntity entity, string statName) : EntityEventArgs
{
    public NetEntity Entity = entity;

    /// <summary>
    /// One of: "strength", "vitality", "dexterity", "intelligence"
    /// </summary>
    public string StatName = statName;
}
