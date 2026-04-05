# OpenEdge 14 — Creating New Systems

## Quick Start: Add a New Spell Effect

This guide walks you through adding a complete new system using a concrete example.

### Goal: Add a "Freezing" Status Effect

**What we're building:**
- A spell that freezes enemies
- Movement speed is reduced to 50%
- Effect lasts 5 seconds
- Admins can inspect and modify duration

---

## Step 1: Define the Component

Create `Content.Shared/_OE14/StatusEffects/OE14FrozenComponent.cs`:

```csharp
using Robust.Shared.GameObjects;

namespace Content.Shared._OE14.StatusEffects;

/// <summary>
/// Applied when a character is frozen. Reduces movement speed.
/// </summary>
[RegisterComponent, Access(typeof(OE14FrozenSystem))]
public sealed partial class OE14FrozenComponent : Component
{
    /// <summary>
    /// When this effect expires and is removed.
    /// </summary>
    [DataField]
    public TimeSpan ExpirationTime = TimeSpan.Zero;

    /// <summary>
    /// Movement speed multiplier (0.5 = 50% speed)
    /// </summary>
    [DataField("speedMultiplier")]
    [ViewVariables(VVAccess.ReadWrite)]
    public float SpeedMultiplier = 0.5f;

    /// <summary>
    /// Default duration in seconds. Admins can modify per-instance.
    /// </summary>
    [DataField("duration")]
    public float Duration = 5f;
}
```

**Key points:**
- `[RegisterComponent]` makes it discoverable
- `[DataField]` fields are serializable and networked
- `[ViewVariables]` fields are inspectable by admins
- `ExpirationTime` is calculated per-instance
- `SpeedMultiplier` and `Duration` are configurable

---

## Step 2: Create the Entity System

Create `Content.Server/_OE14/StatusEffects/OE14FrozenSystem.cs`:

```csharp
using Content.Shared._OE14.StatusEffects;
using Content.Shared.Movement.Systems;
using Robust.Shared.Timing;

namespace Content.Server._OE14.StatusEffects;

/// <summary>
/// Manages the frozen status effect on characters.
/// </summary>
public sealed class OE14FrozenSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _speedMod = default!;
    [Dependency] private readonly INetManager _net = default!;

    public override void Initialize()
    {
        // React when frozen component is added
        SubscribeLocalEvent<OE14FrozenComponent, ComponentInit>(OnFrozenAdded);
        SubscribeLocalEvent<OE14FrozenComponent, ComponentShutdown>(OnFrozenRemoved);
    }

    private void OnFrozenAdded(Entity<OE14FrozenComponent> ent, ref ComponentInit args)
    {
        var component = ent.Comp;

        // Calculate when this effect should expire
        component.ExpirationTime = _timing.CurTime + TimeSpan.FromSeconds(component.Duration);

        // Apply speed penalty (server-only)
        if (!_net.IsClient)
        {
            _speedMod.ChangeBaseSpeed(ent, component.SpeedMultiplier);
        }
    }

    private void OnFrozenRemoved(Entity<OE14FrozenComponent> ent, ref ComponentShutdown args)
    {
        // Restore movement speed
        if (!_net.IsClient)
        {
            _speedMod.ChangeBaseSpeed(ent, 1.0f);
        }
    }

    public override void Update(float frameTime)
    {
        // Server only: check if frozen effects have expired
        if (_net.IsClient)
            return;

        var query = EntityQueryEnumerator<OE14FrozenComponent>();
        while (query.MoveNext(out var uid, out var frozen))
        {
            // Has the effect expired?
            if (_timing.CurTime >= frozen.ExpirationTime)
            {
                // Remove the component (triggers OnFrozenRemoved)
                RemComp<OE14FrozenComponent>(uid);
            }
        }
    }
}
```

**Key points:**
- `Initialize()` sets up event subscriptions
- `OnFrozenAdded()` runs when component is added (set expiration, apply effect)
- `OnFrozenRemoved()` runs when component is removed (cleanup)
- `Update()` checks each frame if effects should expire (server-only)
- `if (_net.IsClient) return;` ensures server-only logic

---

## Step 3: Create an Event to Apply the Effect

Create `Content.Shared/_OE14/StatusEffects/OE14FrozenEvent.cs`:

```csharp
using Robust.Shared.GameObjects;

namespace Content.Shared._OE14.StatusEffects.Events;

/// <summary>
/// Event fired when something should be frozen.
/// Handlers will add the OE14FrozenComponent to the target.
/// </summary>
public sealed partial class OE14ApplyFrozenEvent : EntityEventArgs
{
    public EntityUid Target { get; set; }
    public float Duration { get; set; } = 5f;
    public float SpeedMultiplier { get; set; } = 0.5f;
}
```

---

## Step 4: Create a Spell That Uses This Effect

Update `Content.Server/_OE14/StatusEffects/OE14FrozenSystem.cs` to handle the event:

```csharp
public override void Initialize()
{
    SubscribeLocalEvent<OE14FrozenComponent, ComponentInit>(OnFrozenAdded);
    SubscribeLocalEvent<OE14FrozenComponent, ComponentShutdown>(OnFrozenRemoved);
    SubscribeLocalEvent<OE14ApplyFrozenEvent>(OnApplyFrozen);  // ← Add this
}

private void OnApplyFrozen(OE14ApplyFrozenEvent args)
{
    var component = new OE14FrozenComponent
    {
        Duration = args.Duration,
        SpeedMultiplier = args.SpeedMultiplier,
    };

    AddComp(args.Target, component);
}
```

Now spells can freeze targets by raising this event:

```csharp
// In a spell system
RaiseLocalEvent(new OE14ApplyFrozenEvent
{
    Target = targetEntity,
    Duration = 10f,
    SpeedMultiplier = 0.3f,
});
```

---

## Step 5: Create a Prototype

Create or update `Resources/Prototypes/_OE14/Entities/Objects/spells.yml`:

```yaml
- type: entity
  parent: BaseMagicSpell
  id: SpellFreeze
  name: Freeze
  components:
    - type: Spell
      actionName: "Freeze Target"
      manaCost: 30
      castTime: 1.5
      targetType: entity
```

---

## Step 6: Test It

Build and run:

```bash
dotnet build -c Release
bash runserver.sh
bash runclient.sh
```

### Test with Admin Console

```
# Spawn a test entity with the frozen component
> spawn SpellFreeze

# Inspect the frozen component
> vv 1234

# Check if speed was reduced
# (should see SpeedMultiplier: 0.5 under OE14FrozenComponent)

# Modify duration for testing
> vv 1234 OE14FrozenComponent.Duration 2
# Effect now lasts 2 seconds instead of 5
```

---

## Architecture Review Checklist

### ✅ Component Design
- [ ] Data-only (no logic in component)
- [ ] `[RegisterComponent]` present
- [ ] `[DataField]` on all serializable fields
- [ ] `[ViewVariables]` on admin-modifiable fields
- [ ] `[Access(typeof(System))]` to restrict access

### ✅ System Design
- [ ] Inherits from `EntitySystem`
- [ ] Dependencies injected with `[Dependency]`
- [ ] `Initialize()` subscribes to events
- [ ] Server-only logic has `if (_net.IsClient) return;`
- [ ] Query-based updates use `EntityQueryEnumerator<>`

### ✅ Networking
- [ ] Component changes automatically sync (via `[DataField]`)
- [ ] No manual serialization needed
- [ ] Events are marked `[Serializable, NetSerializable]` if cross-network

### ✅ Testing
- [ ] Test via admin console (`vv` command)
- [ ] Verify values persist when modified by admins
- [ ] Check that server-side logic doesn't run on clients
- [ ] Confirm client-side rendering updates correctly

---

## Common Mistakes to Avoid

### ❌ Mistake 1: Logic in Components

**Bad:**
```csharp
public class MyComponent : Component
{
    public void ApplyEffect() { /* logic */ }  // ← NO!
}
```

**Good:**
```csharp
public class MyComponent : Component
{
    [DataField]
    public int Value = 0;  // Data only
}

public class MySystem : EntitySystem
{
    private void OnComponentInit(Entity<MyComponent> ent, ref ComponentInit args)
    {
        // Logic here
    }
}
```

### ❌ Mistake 2: Forgetting `[DataField]`

**Bad:**
```csharp
public string MyData = "default";  // ← Won't sync to clients!
```

**Good:**
```csharp
[DataField]
public string MyData = "default";  // ← Will sync automatically
```

### ❌ Mistake 3: Client-Side Logic on Server

**Bad:**
```csharp
public override void Update(float frameTime)
{
    _sprite.SetColor(entity, newColor);  // ← Client-only!
}
```

**Good:**
```csharp
public override void Update(float frameTime)
{
    if (_net.IsClient) return;

    // Server-side logic
    entity.Comp.ServerState = newState;
    // ↓ Automatically syncs to clients ↓
}
```

### ❌ Mistake 4: Trusting Client Calculations

**Bad:**
```csharp
// Client-side code
var damage = Math.Random(100);  // ← Client calculated!
SendEvent(server, new DealDamageEvent { Damage = damage });
```

**Good:**
```csharp
// Client-side code
SendEvent(server, new RequestDamageEvent { TargetId = enemy.Uid });

// Server-side code (authoritative)
public void OnRequestDamage(RequestDamageEvent args)
{
    var damage = CalculateDamage(args.TargetId);  // Server decides
    _damageable.TryChangeDamage(args.TargetId, damage);
}
```

---

## File Checklist

When adding a new system, you'll typically create:

- [ ] `Content.Shared/_OE14/[Feature]/[Feature]Component.cs` — Component definition
- [ ] `Content.Server/_OE14/[Feature]/[Feature]System.cs` — Server logic
- [ ] `Content.Client/_OE14/[Feature]/[Feature]System.cs` — Client logic (if needed)
- [ ] `Content.Shared/_OE14/[Feature]/Events/[Feature]Event.cs` — Event definition
- [ ] `Resources/Prototypes/_OE14/[Feature].yml` — Prototype definition
- [ ] `Content.Server/_OE14/[Feature]/Components/*.cs` — Additional data components (if needed)

---

## Next Steps

1. Copy the Frozen example and modify it for your effect
2. Test via `dotnet build` and admin console (`vv` commands)
3. Add more effects using the same pattern
4. Review existing systems for more complex examples:
   - `OE14WallmountSystem` — Multi-component interaction
   - `OE14DoorInteractionPopupSystem` — Event-driven interaction
   - `OE14SpellStorageSystem` — Action synchronization

