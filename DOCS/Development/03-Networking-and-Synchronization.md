# OpenEdge 14 — Networking and Synchronization

## Overview

OpenEdge 14 uses RobustToolbox's **automatic synchronization system**. The server is authoritative, and clients receive deltas (only changed data) automatically. You rarely need to manually serialize data.

---

## Core Concepts

### Authority Model

- **Server is always authoritative** — The source of truth for all game state
- **Clients are optimistic** — They predict changes and show them immediately, then wait for server confirmation
- **If server disagrees** — Client's prediction is rolled back and corrected

### Dirty-Checking

Components are marked "dirty" when their data changes. Only dirty fields are sent to clients:

```
Server modifies: InteractDelay = 2.0 seconds
↓
RobustToolbox detects change
↓
Sends ONLY: "InteractDelay = 2.0" (not the entire component)
↓
Clients receive delta and apply it
```

This saves bandwidth compared to sending full component snapshots.

### Automatic Sync (No Code Needed)

If you use `[DataField]` on a component, the engine **automatically synchronizes it**:

```csharp
[RegisterComponent]
public sealed partial class MyComponent : Component
{
    [DataField]
    public int MyValue = 0;  // ← This field syncs automatically!
}
```

---

## How Data Flows from Server to Client

### Step 1: Server Modifies State

```csharp
// In a server-side system
doorComponent.LastInteractTime = _timing.CurTime;  // Changed!
```

### Step 2: Engine Detects Change

RobustToolbox monitors all `[DataField]` mutations. It marks the component as dirty.

### Step 3: Engine Sends Delta

During the network tick (~60 Hz by default), the engine sends:

```
EntityId: 1234
Component: OE14DoorInteractionPopupComponent
Field: LastInteractTime = 5.234
```

### Step 4: Client Receives and Applies

The client's networked entity receives the update and applies it locally.

### Step 5: Client Can React

Client-side systems can subscribe to component events and react:

```csharp
SubscribeLocalEvent<OE14DoorInteractionPopupComponent, ComponentHandleState>(OnStateChanged);

private void OnStateChanged(Entity<OE14DoorInteractionPopupComponent> ent, ref ComponentHandleState args)
{
    // Update UI or animations based on the new state
}
```

---

## Client-Only vs Server-Only Code

### Server-Only Logic

Use this pattern for authoritative logic that should NEVER run on clients:

```csharp
public override void Update(float frameTime)
{
    if (_net.IsClient)
        return;  // Skip on client

    // Only server runs this
    _damageable.TryChangeDamage(entity, 10);
}
```

**Examples of server-only:**
- Damage calculations
- Inventory modifications
- Character experience gains
- Save/load data

### Client-Only Logic

Use this for rendering and UI:

```csharp
public override void Update(float frameTime)
{
    if (_net.IsServer)
        return;  // Skip on server

    // Only client runs this
    _sprite.SetColor(entity, newColor);
}
```

**Examples of client-only:**
- Drawing graphics
- Playing sounds
- Showing UI windows
- Animations

### Shared Logic

Code that runs on both sides:

```csharp
// This runs on both server and client
public override void Initialize()
{
    SubscribeLocalEvent<MyComponent, MyEvent>(OnMyEvent);
}

private void OnMyEvent(Entity<MyComponent> ent, ref MyEvent args)
{
    // Server: updates game state
    // Client: updates visuals/UI
}
```

---

## Predicted Actions

### What is Prediction?

"Predicted" actions appear **immediately on the client** while waiting for server confirmation. This reduces perceived latency.

### Predicted Popup

```csharp
_popup.PopupPredicted("You opened the door!", target, target);
```

**What happens:**
1. Client sees popup **instantly**
2. Client sends action to server
3. Server validates and applies
4. Server sends confirmation (usually approved)
5. If server **rejected**, client would see error instead

### Predicted Audio

```csharp
_audio.PlayPredicted(doorOpenSound, target, target);
```

Same flow: play sound immediately, confirm with server.

### When to Use Prediction

✅ **Good candidates:**
- Visual feedback (popups, particles)
- Sound effects
- Temporary UI changes
- Animation state

❌ **Bad candidates:**
- Damage (must be server-calculated)
- Inventory changes (must be server-confirmed)
- Character stats (must be authoritative)

---

## Event System

### Local Events (Only on One Side)

```csharp
// Raises an event only on the server
RaiseLocalEvent(entity, new MyEvent { Data = 5 });
```

Use for internal communication within a side.

### Networked Events (Sent to Other Side)

```csharp
[Serializable, NetSerializable]
public sealed partial class OE14SpellUsedEvent : EntityEventArgs
{
    public EntityUid Caster { get; set; }
    public int ManaCost { get; set; }
}
```

**The `[Serializable, NetSerializable]` attributes are required** so the network can serialize the event.

### Event Subscriptions

```csharp
public override void Initialize()
{
    // Responds to events on this entity
    SubscribeLocalEvent<MyComponent, MyEvent>(OnMyEvent);

    // Server responds to networked events from client
    SubscribeNetworkEvent<ClientRequestSpellEvent>(OnClientRequest);
}

private void OnMyEvent(Entity<MyComponent> ent, ref MyEvent args)
{
    // Handle it
}

private void OnClientRequest(ClientRequestSpellEvent args)
{
    // Handle client request
}
```

---

## Component State (Serialization)

### ComponentState

Each networked component can define how it serializes for transmission:

```csharp
public sealed partial class MyComponent : Component
{
    [DataField]
    public int CurrentValue = 0;

    public override ComponentState GetComponentState()
    {
        return new MyComponentState { CurrentValue = CurrentValue };
    }
}

[Serializable, NetSerializable]
public sealed class MyComponentState : ComponentState
{
    public int CurrentValue { get; set; }
}
```

**Note:** Most components don't need this! `[DataField]` handles it automatically.

---

## Admin Console Commands

### ViewVariables (Inspect & Modify)

Admins can use `vv` command to inspect entity state in real-time:

```
> vv 1234
# Shows all components on entity 1234

> vv 1234 OE14DoorInteractionPopupComponent.InteractDelay
# Shows the InteractDelay field

> vv 1234 OE14DoorInteractionPopupComponent.InteractDelay 3
# Sets InteractDelay to 3 seconds
```

### How ViewVariables Works

1. Admin sets a value via console
2. Server updates the component field
3. RobustToolbox marks component dirty
4. Network sends change to all clients
5. Clients see updated value immediately

### Making Fields Admin-Accessible

```csharp
[ViewVariables(VVAccess.ReadWrite)]
public int MyValue = 0;
```

**VVAccess options:**
- `Read` — view only
- `ReadWrite` — view and modify
- (omitted) — private (not visible)

---

## Performance Considerations

### Bandwidth

- **Dirty-checking is efficient** — Only changed data is sent
- **Avoid large [DataField] arrays** — Send entire array on any element change
- **Use appropriate sync rates** — Default 60 Hz is good for most games

### Latency

- **Prediction reduces perceived latency** — Use `PopupPredicted()` and `PlayPredicted()`
- **Not suitable for all actions** — Damage/inventory must wait for server
- **Rollback can feel jarring** — If prediction fails, client sees "correction"

### Best Practices

✅ **DO:**
- Use server-only for authoritative calculations
- Use predicted actions for visual feedback
- Subscribe to events for updates instead of polling
- Keep components lean (use prototypes for defaults)

❌ **DON'T:**
- Send large objects repeatedly if unchanged
- Predict damage or inventory changes
- Trust client-side calculations for player stats
- Serialize complex nested structures every frame

---

## Debugging Network Issues

### Check if Running on Server

```csharp
if (_net.IsServer) Log.Info("Server-side code");
if (_net.IsClient) Log.Info("Client-side code");
if (_net.IsConnected) Log.Info("Client is connected");
```

### Log Component Changes

Add debug logging when components are serialized:

```csharp
private void OnStateChange(Entity<MyComponent> ent, ref ComponentHandleState args)
{
    Log.Info($"Component state changed: {ent.Comp.MyValue}");
}
```

### Use Admin Tools

- `vv [entity-id]` — See all component states
- `vv [entity-id] [component-field]` — See specific value
- Toggle `sv_netlogging true` — Log all network messages (verbose!)

### Monitor Network Traffic

RobustToolbox has built-in network statistics accessible via `/net_stats` command.

---

## Common Patterns

### Pattern: Server Updates Client

```csharp
// Server-side system
public override void Update(float frameTime)
{
    if (_net.IsClient) return;

    // Modify component data
    component.LastUpdateTime = _timing.CurTime;
    // ↓ Automatically syncs to clients ↓
}
```

### Pattern: Client Reacts to Update

```csharp
// Client-side system
public override void Initialize()
{
    SubscribeLocalEvent<MyComponent, ComponentHandleState>(OnStateUpdate);
}

private void OnStateUpdate(Entity<MyComponent> ent, ref ComponentHandleState args)
{
    // Client sees that LastUpdateTime changed
    // Can update visuals here
}
```

### Pattern: Admin Changes Value

```csharp
// Component definition
[ViewVariables(VVAccess.ReadWrite)]
public int AdminControlledValue = 0;

// Admin console
> vv 1234 OE14MyComponent.AdminControlledValue 100
// ↓ Automatically syncs to all clients ↓
```

---

## Next Steps

- Review `OE14SharedMagicSpellStorageSystem` for a complete example
- Check how `OE14DoorInteractionPopupSystem` handles interaction events
- Examine `OE14WallmountSystem` for server-only update logic
- Use `vv` command in-game to inspect live entity state

