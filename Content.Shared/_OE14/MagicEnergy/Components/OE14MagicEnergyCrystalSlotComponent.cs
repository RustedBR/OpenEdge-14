using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._OE14.MagicEnergy.Components;

/// <summary>
/// Allows you to examine how much energy is in that object.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedOE14MagicEnergyCrystalSlotSystem))]
public sealed partial class OE14MagicEnergyCrystalSlotComponent : Component
{
    [DataField(required: true)]
    public string SlotId = string.Empty;

    public bool Powered = false;
}

[Serializable, NetSerializable]
public enum OE14MagicSlotVisuals : byte
{
    Inserted,
    Powered,
}

/// <summary>
/// Is called when the state of the crystal is changed: it is pulled out, inserted, or the amount of energy in it has changed.
/// </summary>
public sealed class OE14SlotCrystalChangedEvent : EntityEventArgs
{
    public readonly bool Ejected;

    public OE14SlotCrystalChangedEvent(bool ejected)
    {
        Ejected = ejected;
    }
}

/// <summary>
/// Is called when the power status of the device changes.
/// </summary>
public sealed class OE14SlotCrystalPowerChangedEvent : EntityEventArgs
{
    public readonly bool Powered;

    public OE14SlotCrystalPowerChangedEvent(bool powered)
    {
        Powered = powered;
    }
}
