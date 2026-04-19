using Robust.Shared.Serialization;

namespace Content.Shared._OE14.CommunicationCrystal;

[Serializable, NetSerializable]
public enum OE14CommunicationCrystalUiKey
{
    Board,
}

[Serializable, NetSerializable]
public sealed class OE14CommunicationCrystalUiState : BoundUserInterfaceState
{
    public int CurrentEnergy { get; }
    public int MaxEnergy { get; }
    public bool HasEnergyCrystal { get; }
    public bool CanSendGlobal { get; }
    public TimeSpan? GlobalCooldown { get; }

    public OE14CommunicationCrystalUiState(int currentEnergy, int maxEnergy, bool hasEnergyCrystal, bool canSendGlobal, TimeSpan? globalCooldown)
    {
        CurrentEnergy = currentEnergy;
        MaxEnergy = maxEnergy;
        HasEnergyCrystal = hasEnergyCrystal;
        CanSendGlobal = canSendGlobal;
        GlobalCooldown = globalCooldown;
    }
}

[Serializable, NetSerializable]
public sealed class OE14CommunicationCrystalSendMessage : BoundUserInterfaceMessage
{
    public bool IsGlobal { get; }
    public string Message { get; }

    public OE14CommunicationCrystalSendMessage(bool isGlobal, string message)
    {
        IsGlobal = isGlobal;
        Message = message;
    }
}

[Serializable, NetSerializable]
public sealed class OE14CommunicationCrystalRemoveCrystal : BoundUserInterfaceMessage
{
}