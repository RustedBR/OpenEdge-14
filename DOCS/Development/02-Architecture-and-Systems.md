# OpenEdge 14 — Architecture and Systems

## Overview

OpenEdge 14 uses an **Entity-Component-System (ECS)** architecture built on the **RobustToolbox** engine (C# / .NET 9). The architecture separates logic into three distinct tiers:

1. **Shared** (`Content.Shared/`) — Code that runs on both server and client
2. **Server** (`Content.Server/`) — Logic that only runs on the server
3. **Client** (`Content.Client/`) — Logic that only runs on the client

This separation ensures deterministic simulation on the server while allowing the client to predict and render changes immediately.

---

## The ECS Pattern

### Components (Data Containers)

Components are **pure data containers** that hold state. They don't contain logic.

```csharp
[RegisterComponent, Access(typeof(OE14WallmountSystem))]
public sealed partial class OE14WallmountComponent : Component
{
    [DataField]
    public int AttachAttempts = 3;

    [DataField]
    public TimeSpan NextAttachTime = TimeSpan.Zero;
}
```

**Key attributes:**
- `[RegisterComponent]` — Registers the component in the ECS registry
- `[DataField]` — Marks a field as serializable (saved in prototypes/maps)
- `[Access(typeof(System))]` — Restricts which systems can access this component
- `[ViewVariables(VVAccess.ReadWrite)]` — Allows admins to inspect/modify via console

### Entity Systems (Logic)

Entity Systems contain **all the logic** and react to events or update each frame.

```csharp
public sealed class OE14WallmountSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly INetManager _net = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<OE14WallmountComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnShutdown(Entity<OE14WallmountComponent> ent, ref ComponentShutdown args)
    {
        // Logic here
    }

    public override void Update(float frameTime)
    {
        if (_net.IsClient)
            return;  // Server-only logic

        // Frame-by-frame logic
    }
}
```

**Key features:**
- Inherit from `EntitySystem`
- Use `[Dependency]` to inject other systems
- Use `SubscribeLocalEvent<>()` to react to events
- Implement `Update()` for per-frame logic
- Check `if (_net.IsClient) return;` for server-only code

---

## How the Server-Client Communication Works

### Automatic Synchronization

The RobustToolbox **automatically synchronizes component data** from server to client using "dirty-checking":

1. **Server creates/modifies entity** with components
2. **RobustToolbox detects changes** to `[DataField]` fields
3. **Network layer sends deltas** (only changed fields) to clients
4. **Clients receive and apply** changes to their local entity copies

This means **you don't manually serialize or send data**. The engine handles it.

### Example: Spell Storage

When a player equips a spell-storage item:

1. **Server**: `OE14SpellStorageSystem.OnMagicStorageInit()` creates spell actions
2. **Server**: Calls `_actions.GrantActions()` to sync spells to the player
3. **Network**: RobustToolbox sends action entities to the client automatically
4. **Client**: Receives the action entities and displays them in the action bar

### Predicted Actions

Some actions are "predicted" (appear immediately on the client, then verified by the server):

```csharp
_popup.PopupPredicted(message, target, target);  // Shows immediately
_audio.PlayPredicted(sound, target, target);     // Plays immediately
```

This reduces apparent latency. The client assumes the server will approve and shows the effect right away.

---

## How Entities Interact with the World

### Event-Driven Interaction

When a player interacts with something (clicks on it), a series of events fire:

1. **Client sends input** — Player clicks on an entity
2. **Server receives `ActivateInWorldEvent`** — Targeted at the clicked entity
3. **Systems subscribe to this event** and execute logic:

```csharp
public override void Initialize()
{
    SubscribeLocalEvent<OE14DoorInteractionPopupComponent, ActivateInWorldEvent>(OnActivatedInWorld);
}

private void OnActivatedInWorld(Entity<OE14DoorInteractionPopupComponent> door,
                                ref ActivateInWorldEvent args)
{
    // This runs on the server when the door is clicked
    _popup.PopupPredicted("Door is locked!", args.Target, args.Target);
}
```

### Example: Door Interaction

**What happens when a player clicks a locked door:**

1. Client detects click → sends `ActivateInWorldEvent` to server
2. Server receives event at `OE14DoorInteractionPopupSystem.OnActivatedInWorld()`
3. System checks:
   - Is the door locked? (`LockComponent`)
   - Has enough time passed since last interaction? (`InteractDelay`)
4. If valid:
   - Shows a popup message
   - Plays a sound
   - Updates `LastInteractTime`
5. Network layer syncs these changes to all clients

---

## How Admins Interact with the World

### ViewVariables Inspection

Admins can inspect and modify component data via the admin console using `ViewVariables`:

```csharp
[ViewVariables(VVAccess.ReadWrite)]
public TimeSpan LastInteractTime = TimeSpan.Zero;
```

**VVAccess options:**
- `Read` — Admin can view but not modify
- `ReadWrite` — Admin can view and modify
- (omitted) — Field is private

**Usage in console:**
```
vv [entity-id]                           # View all components
vv [entity-id] LastInteractTime          # View specific field
vv [entity-id] LastInteractTime 5        # Set field to new value
```

### Admin Commands

Admins can also execute commands via the console. While OpenEdge 14 doesn't have custom admin commands yet, the engine supports them via `ConCmd` attributes (used in the base Ss14-Medieval).

Example pattern (from RobustToolbox):
```csharp
[ConCmd.Command]
public void GodMode(IConsoleShell shell, int args)
{
    // Admin command logic
}
```

### Real-Time State Changes

When an admin modifies a value via `ViewVariables`:

1. **Admin sets value** in console (runs on server)
2. **Component field updates** in memory
3. **RobustToolbox detects change** and marks component as dirty
4. **Network layer sends update** to all clients
5. **Clients reflect the change** immediately

This allows admins to adjust game state without restarting.

---

## Key Systems Architecture

### Server-Side (`Content.Server/`)

Handles:
- **Authoritative logic** (damage, inventory, character state)
- **Event responses** to player actions
- **Spawning/despawning** entities
- **Saving/loading** persistent data

Example:
```csharp
private void OnSpellUsed(Entity<OE14SpellStorageUseDamageComponent> ent,
                        ref OE14SpellFromSpellStorageUsedEvent args)
{
    // Server deducts damage (authoritative)
    _damageable.TryChangeDamage(ent, ent.Comp.DamagePerMana * args.Manacost);
}
```

### Client-Side (`Content.Client/`)

Handles:
- **Rendering** (sprites, animations, visual effects)
- **UI** (windows, buttons, menus)
- **Input** (mouse, keyboard)
- **Prediction** (showing changes before server confirmation)
- **Audio playback**

Example:
```csharp
public sealed partial class OE14ClientSpellStorageSystem : OE14SharedSpellStorageSystem
{
    // Client-specific rendering and UI logic would go here
}
```

### Shared (`Content.Shared/`)

Handles:
- **Event definitions** that both sides use
- **Component definitions** that are serialized between server and client
- **Enum/prototype definitions** shared by both sides

Example event:
```csharp
namespace Content.Shared._OE14.MagicSpell.Events;

[Serializable, NetSerializable]
public sealed partial class OE14SpellFromSpellStorageUsedEvent : EntityEventArgs
{
    public int Manacost { get; set; }
}
```

---

## Data Flow Example: Using a Spell

Here's a complete example of what happens when a player uses a spell stored in an item:

### Step 1: Client-Side (User Input)
```
Player clicks action button → Client detects input
```

### Step 2: Client Sends Action
```
Client → Server: "EntityActionEvent { Action = SpellEntity }"
```

### Step 3: Server Validates & Executes
```
Server receives event
→ OE14ActionManaCostSystem checks: does player have enough mana?
→ If yes: deduct mana, broadcast spell effect
→ If no: deny action, send error popup to client
```

### Step 4: Server-Side Effect
```
OE14SpellStorageSystem.OnSpellUsed() fires
→ Modifies entity state (damage, position, etc.)
→ RobustToolbox marks affected components as dirty
```

### Step 5: Auto-Sync to Clients
```
RobustToolbox network layer sends changes:
- Mana value: 100 → 85
- Health value: 120 → 100
- Spell effect visibility: none → true
```

### Step 6: Client Renders
```
Client receives updates
→ Renders new mana bar
→ Plays spell animation
→ Shows damage numbers
```

---

## Common Patterns

### Pattern 1: Validate Server-Only

```csharp
public override void Update(float frameTime)
{
    if (_net.IsClient)
        return;  // Only run on server

    // Server-only logic here
}
```

### Pattern 2: React to Events

```csharp
public override void Initialize()
{
    SubscribeLocalEvent<MyComponent, ActivateInWorldEvent>(OnActivate);
}

private void OnActivate(Entity<MyComponent> ent, ref ActivateInWorldEvent args)
{
    // This runs when the entity is interacted with
}
```

### Pattern 3: Inject Systems

```csharp
[Dependency] private readonly SharedTransformSystem _transform = default!;
[Dependency] private readonly IGameTiming _timing = default!;

var gridUid = Transform(entity).GridUid;
var curTime = _timing.CurTime;
```

### Pattern 4: Query Entities

```csharp
var query = EntityQueryEnumerator<MyComponent>();
while (query.MoveNext(out var uid, out var component))
{
    // Process each entity with MyComponent
}
```

---

## File Structure Reference

| Path | Purpose |
|------|---------|
| `Content.Server/_OE14/` | Server-only OE14 systems |
| `Content.Client/_OE14/` | Client-only OE14 systems (UI, rendering) |
| `Content.Shared/_OE14/` | OE14 components, shared events, prototypes |
| `Resources/Prototypes/_OE14/` | Entity prototype definitions (YAML) |
| `Resources/Maps/_OE14/` | Map files (YAML) |
| `Resources/ServerInfo/Guidebook/OE14_*.xml` | In-game guidebook pages |

---

## Debugging Tips

### Check if code is running on server or client:
```csharp
if (_net.IsClient) { /* client code */ }
if (_net.IsServer) { /* server code */ }
```

### Allow admins to inspect component state:
```csharp
[ViewVariables(VVAccess.ReadWrite)]
public int MyValue = 0;
```

### Serialize component data to prototypes:
```csharp
[DataField]
public string MyData = "default";
```

### Make an event that the network knows about:
```csharp
[Serializable, NetSerializable]
public sealed class MyEvent : EntityEventArgs { }
```

---

## Next Steps

- Read individual system documentation in `Content.Server/_OE14/`
- Review working examples like `OE14WallmountSystem` and `OE14DoorInteractionPopupSystem`
- Check the Ss14-Medieval base code for similar patterns
- Use the in-game admin tools to test and iterate

