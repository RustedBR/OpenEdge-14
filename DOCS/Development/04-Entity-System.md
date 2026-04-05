# Entity Component System (ECS) Deep Dive

This guide explains OpenEdge 14's architecture and how to work with entities, components, and systems.

## What is ECS?

The Entity Component System is a design pattern that structures game code differently from object-oriented programming:

| OOP | ECS |
|-----|-----|
| Class hierarchy (Dog extends Animal) | Flat entity list |
| Data in classes | Data in components |
| Methods in classes | Logic in systems |
| Rigid structure | Flexible composition |

### Example

**OOP Approach (Not Used Here)**
```csharp
abstract class Actor { }
class Monster : Actor {
    int health;
    float speed;
    void TakeDamage() { }
    void Move() { }
}
class Player : Actor {
    int health;
    float speed;
    void TakeDamage() { }
    void Move() { }
    void Cast() { }
}
// Code duplication, rigid hierarchy
```

**ECS Approach (Used Here)**
```csharp
// Entities are just IDs
Entity player = entityManager.SpawnEntity("player_prototype");

// Components are pure data
component Health { int current; int max; }
component Position { float x, y; }
component Speed { float walkSpeed; float sprintSpeed; }
component Spellcaster { List<Spell> spells; }

// Systems have logic
class HealthSystem : EntitySystem {
    void TakeDamage(Entity entity, int damage) { /* ... */ }
}
class MovementSystem : EntitySystem {
    void Move(Entity entity, Vector direction) { /* ... */ }
}
```

## Core Concepts

### Entity

An entity is a unique identifier for a game object. It doesn't contain data—it's just an ID that components attach to.

```csharp
EntityUid playerId = entityManager.SpawnEntity("player");
// playerId = 123 (just an ID)
```

### Component

A component is pure data with no logic. Components define properties that can be shared across different entity types.

```csharp
[RegisterComponent]
public sealed partial class HealthComponent : Component
{
    [DataField]
    public int CurrentHealth = 100;

    [DataField]
    public int MaxHealth = 100;

    [DataField]
    public string DamageContainer = "Biological";
}
```

The `[RegisterComponent]` attribute lets the engine know this component exists.
The `[DataField]` attribute marks fields that can be set in YAML prototypes.

### System

A system contains logic that operates on entities with specific components.

```csharp
public sealed class HealthSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        // Subscribe to events
        SubscribeLocalEvent<HealthComponent, DamageEvent>(OnDamage);
    }

    private void OnDamage(Entity<HealthComponent> entity, ref DamageEvent args)
    {
        // This runs when a HealthComponent receives damage
        entity.Comp.CurrentHealth -= (int)args.Damage;

        if (entity.Comp.CurrentHealth <= 0)
        {
            _entityManager.DeleteEntity(entity.Owner);
        }
    }
}
```

### Prototype

A prototype is a YAML template that defines what components and data an entity has when spawned.

```yaml
- type: entity
  id: OE14Goblin
  name: Goblin
  description: A small, aggressive creature
  components:
  - type: Transform
  - type: Sprite
    sprite: _OE14/Mobs/Goblin/goblin.rsi
    state: idle
  - type: Health
    currentHealth: 30
    maxHealth: 30
  - type: MeleeWeapon
    damage:
      types:
        Slash: 5
```

When you spawn this entity, it automatically gets a Transform, Sprite, Health, and MeleeWeapon component.

## Working with Entities

### Spawning an Entity

```csharp
// From a prototype
EntityUid goblin = _entityManager.SpawnEntity("OE14Goblin", new MapCoordinates(50, 50, mapId));

// Custom with builder
EntityUid sword = _entityManager.SpawnEntity("OE14Sword");
_transformSystem.SetWorldPosition(sword, new Vector2(60, 60));
```

### Getting Components

```csharp
// Try to get a component
if (TryComp<HealthComponent>(entity, out var health))
{
    health.CurrentHealth -= 10;
}

// Get component (throws if missing)
var transform = Comp<TransformComponent>(entity);
transform.Coordinates = new EntityCoordinates(entity, Vector2.Zero);
```

### Adding Components

```csharp
// Add a component to an existing entity
var newComponent = new StatusEffectComponent { EffectName = "Poisoned" };
_entityManager.AddComponent(entity, newComponent);
```

### Deleting Entities

```csharp
_entityManager.DeleteEntity(entity);
```

## Creating Custom Components

### Simple Data Component

```csharp
namespace Content.Shared._OE14.MyFeature;

/// <summary>
/// Tracks how many items an entity has collected.
/// </summary>
[RegisterComponent]
public sealed partial class CollectorComponent : Component
{
    /// <summary>
    /// Number of items collected so far.
    /// </summary>
    [DataField]
    public int ItemCount = 0;

    /// <summary>
    /// Maximum items that can be collected.
    /// </summary>
    [DataField]
    public int MaxItems = 100;

    /// <summary>
    /// Reward for collecting all items.
    /// </summary>
    [DataField]
    public string RewardPrototype = "OE14RewardGold";
}
```

**Using in YAML**

```yaml
- type: entity
  id: OE14QuestCollector
  components:
  - type: Collector
    itemCount: 0
    maxItems: 10
    rewardPrototype: OE14RewardSilver
```

### Component with Events

```csharp
namespace Content.Shared._OE14.Magic;

/// <summary>
/// Indicates this entity can cast spells.
/// </summary>
[RegisterComponent]
public sealed partial class SpellcasterComponent : Component
{
    /// <summary>
    /// Mana pool for spell casting.
    /// </summary>
    [DataField]
    public float Mana = 100;

    [DataField]
    public float MaxMana = 100;

    /// <summary>
    /// Known spells.
    /// </summary>
    [DataField]
    public List<string> Spells = new();
}

/// <summary>
/// Raised when an entity attempts to cast a spell.
/// </summary>
[ByRefEvent]
public record struct CastSpellEvent(string SpellId, EntityUid Target);
```

## Creating Custom Systems

### Basic System Structure

```csharp
namespace Content.Server._OE14.MyFeature;

/// <summary>
/// Manages collection rewards and progression.
/// </summary>
public sealed class CollectorSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        // Subscribe to component addition
        SubscribeLocalEvent<CollectorComponent, ComponentInit>(OnCollectorInit);

        // Subscribe to events
        SubscribeLocalEvent<CollectorComponent, CollectItemEvent>(OnCollectItem);
    }

    private void OnCollectorInit(Entity<CollectorComponent> entity, ref ComponentInit args)
    {
        // Called when a CollectorComponent is added to an entity
        Logger.Info($"Collector initialized with max {entity.Comp.MaxItems} items");
    }

    private void OnCollectItem(Entity<CollectorComponent> entity, ref CollectItemEvent args)
    {
        entity.Comp.ItemCount++;
        Logger.Info($"Collected item {entity.Comp.ItemCount}/{entity.Comp.MaxItems}");

        if (entity.Comp.ItemCount >= entity.Comp.MaxItems)
        {
            GiveReward(entity);
        }
    }

    private void GiveReward(Entity<CollectorComponent> entity)
    {
        // Spawn the reward item
        var reward = _entityManager.SpawnEntity(entity.Comp.RewardPrototype,
            _transform.GetMoverCoordinates(entity));

        Logger.Info("Reward given!");
        entity.Comp.ItemCount = 0;  // Reset for next collection cycle
    }
}
```

### Event-Based Communication

Instead of systems directly calling each other, they use events:

```csharp
// System A raises an event
var @event = new CastSpellEvent("FireballSpell", targetEntity);
RaiseLocalEvent(casterEntity, ref @event);

// System B responds to that event
SubscribeLocalEvent<SpellcasterComponent, CastSpellEvent>(OnSpellCast);

private void OnSpellCast(Entity<SpellcasterComponent> entity, ref CastSpellEvent args)
{
    if (!entity.Comp.Spells.Contains(args.SpellId))
        return;  // Don't know this spell

    // Execute the spell
    Logger.Info($"Casting {args.SpellId} on {args.Target}");
}
```

## Component Organization

### Shared vs Server vs Client Components

**Shared** (`Content.Shared/_OE14/`)
- Used by both server and client
- Network synchronized automatically
- Examples: Position, Sprite, Health

**Server** (`Content.Server/_OE14/`)
- Server-only logic
- Not sent to clients
- Examples: AI behavior, loot tables

**Client** (`Content.Client/_OE14/`)
- Client-only (rare in SS14)
- Graphics, sound, UI
- Not synchronized to server

```csharp
// Shared - both sides know about it
[RegisterComponent]
public sealed partial class HealthComponent : Component { /* ... */ }

// Server only - client doesn't know about AI
[RegisterComponent]
public sealed partial class AIComponent : Component { /* ... */ }
```

## Advanced Patterns

### Generic Queries

Query multiple entities with specific components:

```csharp
public void UpdateAllEnemies()
{
    // Get all entities with Health AND MeleeWeapon
    var query = EntityQueryEnumerator<HealthComponent, MeleeWeaponComponent>();

    while (query.MoveNext(out var uid, out var health, out var weapon))
    {
        if (health.CurrentHealth <= 0)
        {
            _entityManager.DeleteEntity(uid);
        }
    }
}
```

### Component Ordering with Dependencies

Components can depend on each other:

```csharp
[RegisterComponent]
public sealed partial class StatusEffectComponent : Component
{
    [DataField]
    public List<StatusEffect> Effects = new();
}

public override void Initialize()
{
    base.Initialize();
    // StatusEffect system runs after Health system
    UpdatesAfter.Add(typeof(HealthSystem));
}
```

### Networked Data

Data is automatically sent to clients if components are in Shared:

```csharp
[RegisterComponent]
public sealed partial class HealthComponent : Component
{
    [DataField]
    public int CurrentHealth { get; set; }  // Automatically networked!
}
```

Changes to CurrentHealth automatically appear on all clients.

## Debugging ECS

### Checking Entity Components

```bash
# In admin console
/entcomp <entity_uid>
# Shows all components on that entity
```

### Subscribing to Events

```csharp
// Log all damage events
SubscribeLocalEvent<DamageEvent>(args =>
    Logger.Info($"Damage event: {args}"));
```

### Performance Profiling

```csharp
_stopwatch.Start();
// ... code to profile ...
_stopwatch.Stop();
Logger.Info($"Took {_stopwatch.ElapsedMilliseconds}ms");
```

## Best Practices

1. **Keep Components Small**
   - One concern per component
   - Reuse components across entity types

2. **Use Events for Communication**
   - Don't have systems call each other directly
   - Keeps systems decoupled

3. **Put Data in Components**
   - Systems contain logic, not data
   - Components are just data containers

4. **Use Shared for Synchronized Data**
   - Server and client both need to know
   - Sprite, Position, Health, etc.

5. **Namespace Organization**
   - `Content.Shared/_OE14/SystemName/`
   - `Content.Server/_OE14/SystemName/`
   - `Content.Client/_OE14/SystemName/`

## Real-World Example: Spell Casting

### Components

```csharp
// SpellcasterComponent - tracks spell state
[RegisterComponent]
public sealed partial class SpellcasterComponent : Component
{
    [DataField] public List<string> KnownSpells = new();
    [DataField] public float Mana = 100;
    [DataField] public float MaxMana = 100;
}

// CastingComponent - in-progress cast
[RegisterComponent]
public sealed partial class CastingComponent : Component
{
    [DataField] public string SpellId = "";
    [DataField] public float CastTime = 0;
    [DataField] public float ElapsedTime = 0;
}
```

### Events

```csharp
[ByRefEvent]
public record struct CastSpellEvent(string SpellId, EntityUid Target);

[ByRefEvent]
public record struct SpellCompleteEvent(string SpellId, EntityUid Target);
```

### System

```csharp
public sealed class SpellSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SpellcasterComponent, CastSpellEvent>(OnCast);
    }

    private void OnCast(Entity<SpellcasterComponent> entity, ref CastSpellEvent args)
    {
        if (!entity.Comp.KnownSpells.Contains(args.SpellId))
            return;

        // Add casting component to track progress
        var casting = AddComp<CastingComponent>(entity);
        casting.SpellId = args.SpellId;
        casting.CastTime = GetSpellCastTime(args.SpellId);
    }

    public override void Update(float frameTime)
    {
        // Update all active casts
        var query = EntityQueryEnumerator<CastingComponent>();
        while (query.MoveNext(out var uid, out var casting))
        {
            casting.ElapsedTime += frameTime;

            if (casting.ElapsedTime >= casting.CastTime)
            {
                // Cast complete!
                RemComp<CastingComponent>(uid);
                var @event = new SpellCompleteEvent(casting.SpellId, uid);
                RaiseLocalEvent(uid, ref @event);
            }
        }
    }
}
```

## Next Steps

- Study existing systems in `Content.Server/_OE14/`
- Create a simple component and system
- Read the [Adding Features guide](02-Adding-Features.md)
- Explore the SS14 documentation: https://docs.spacestation14.io/

---

**Co-Authored-By**: Claude Haiku 4.5 <noreply@anthropic.com>
