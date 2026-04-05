using Content.Shared._OE14.CharacterStats;
using Content.Shared._OE14.CharacterStats.Components;
using Content.Shared._OE14.CharacterStats.Systems;
using Robust.Client.Player;

namespace Content.Client._OE14.CharacterStats;

/// <summary>
/// Client-side character stats system.
/// Handles updating local UI when stats change and sending spend-point requests to the server.
/// </summary>
public sealed partial class OE14ClientCharacterStatsSystem : OE14SharedCharacterStatsSystem
{
    [Dependency] private readonly IPlayerManager _player = default!;

    public event Action<EntityUid>? OnStatsUpdated;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<OE14CharacterStatsComponent, AfterAutoHandleStateEvent>(OnAfterAutoHandleState);
    }

    private void OnAfterAutoHandleState(Entity<OE14CharacterStatsComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        if (ent.Owner != _player.LocalEntity)
            return;

        // Recalculate ManaBonus, HealthBonus, etc. from the updated modifiers.
        UpdateStatsCalculations(ent);
        OnStatsUpdated?.Invoke(ent.Owner);
    }

    /// <summary>
    /// Sends a request to the server to spend one available stat point on the given stat.
    /// </summary>
    public void RequestSpendPoint(EntityUid entity, string statName)
    {
        var msg = new OE14SpendStatPointMessage(GetNetEntity(entity), statName);
        RaiseNetworkEvent(msg);
    }
}
