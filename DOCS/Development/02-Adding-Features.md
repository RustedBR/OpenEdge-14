# Adding Features to OpenEdge 14

This guide explains how to add new features to OpenEdge 14, whether it's a new spell, crafting recipe, item, or game system.

> **Using OpenCode?** Say "find files containing X" or "search for pattern" to search the codebase.

## Understanding the Architecture

OpenEdge 14 uses the **Entity Component System (ECS)** pattern. Before adding features, understand these concepts:

- **Entity**: A game object (player, NPC, item, projectile)
- **Component**: Data attached to entities (position, sprite, health)
- **System**: Logic that processes components (movement, combat, crafting)

## Adding a Simple Item

### Step 1: Create the Prototype

Items are defined in YAML files under `Resources/Prototypes/_OE14/Entities/`.

Create a new file: `Resources/Prototypes/_OE14/Entities/Objects/Items/sword_example.yml`

```yaml
- type: entity
  id: OE14WeaponSwordExample
  name: Example Sword
  description: A fine steel sword made for adventurers
  components:
  - type: Transform
  - type: Sprite
    sprite: _OE14/Objects/Weapons/sword.rsi
    state: sword_example
  - type: Item
    size: Normal
  - type: Damageable
    damageContainer: Biological
  - type: MeleeWeapon
    wieldSounds:
      - path: /Audio/Weapons/sword_draw.ogg
    attackSounds:
      - path: /Audio/Weapons/sword_hit.ogg
    damage:
      types:
        Slash: 10
```

### Step 2: Add the Sprite (If Needed)

If you're adding a new item, you'll need a sprite:

1. Create/edit an RSI file in `Resources/Textures/_OE14/Objects/Weapons/sword.rsi/`
2. Add your sprite image and metadata in the RSI format
3. The `state:` field references a state in that RSI file

### Step 3: Register in Guidebook (Optional)

To showcase your item in the guidebook, add it to the appropriate XML:

```xml
<Box>
  <GuideEntityEmbed Entity="OE14WeaponSwordExample"/>
</Box>
```

### Step 4: Build and Test

```bash
dotnet build -c Release
bash runserver.sh
# Connect and spawn: /spawn OE14WeaponSwordExample
```

## Adding a Magic Spell

### Step 1: Create the Action Prototype

Spells are actions defined in `Resources/Prototypes/_OE14/Entities/Actions/Spells/`.

Create: `Resources/Prototypes/_OE14/Entities/Actions/Spells/Fire/fireball_example.yml`

```yaml
- type: entity
  id: OE14ActionSpellFireballExample
  parent: OE14ActionSpellBase
  name: Fireball Example
  description: Launch a ball of fire at the target
  components:
  - type: OE14ActionManaCost
    manaCost: 20
  - type: OE14ActionSpeaking
    startSpeech: "Incendium!"
  - type: DoAfterArgs
    repeat: false
    breakOnDamage: true
    delay: 1.0
    distanceThreshold: 10
  - type: Action
    useDelay: 2.0
    icon:
      sprite: _OE14/Actions/Spells/fire.rsi
      state: fireball
  - type: TargetAction
    range: 7
  - type: EntityTargetAction
    event: !type:OE14EntityTargetModularEffectEvent
      effects:
      - !type:OE14SpellApplyEntityEffect
        effects:
        - !type:BurningReaction
          temperature: 500
```

### Step 2: Create a Spell Scroll Item

To make it learnable, create a scroll item:

```yaml
- type: entity
  parent: OE14BaseSpellScrollFire
  id: OE14SpellScrollFireballExample
  name: fireball example spell scroll
  components:
  - type: OE14SpellStorage
    spells:
    - OE14ActionSpellFireballExample
```

### Step 3: Create Guide Display (For Guidebook)

Add to `Resources/Prototypes/_OE14/Entities/Guidebook/spell_displays.yml`:

```yaml
- type: entity
  parent: OE14GuideSpellDisplayBase
  id: OE14GuideSpellFireballExample
  name: Fireball Example
  components:
  - type: Sprite
    sprite: _OE14/Actions/Spells/fire.rsi
    state: fireball
```

### Step 4: Test and Balance

```bash
# Spawn and equip the scroll
/spawn OE14SpellScrollFireballExample
# The spell appears in your action bar - test damage/mana cost
```

## Adding a Crafting Recipe

### Step 1: Create the Recipe Prototype

Recipes go in `Resources/Prototypes/_OE14/Recipes/`.

Create: `Resources/Prototypes/_OE14/Recipes/Smithing/sword_example_recipe.yml`

```yaml
- type: entity
  id: OE14RecipeSwordExample
  parent: OE14BaseSmithingRecipe
  components:
  - type: Recipe
    name: Forge Example Sword
    description: A fine steel sword
    difficulty: 3
    time: 15
    result:
      proto: OE14WeaponSwordExample
      amount: 1
    materials:
    - proto: OE14MetalSteel
      amount: 5
    - proto: OE14WoodLog
      amount: 2
    tools:
    - OE14Hammer
```

### Step 2: Verify Tool and Material Exist

Make sure the materials and tools reference valid item prototypes:
- `OE14MetalSteel` - the steel material
- `OE14Hammer` - the crafting tool

If they don't exist, create them using the item creation steps above.

### Step 3: Test Crafting

```bash
# Spawn materials and tool
/spawn OE14MetalSteel 5
/spawn OE14WoodLog 2
/spawn OE14Hammer
# Use the workbench to craft
```

## Adding a New NPC/Mob

### Step 1: Create the Mob Prototype

NPCs go in `Resources/Prototypes/_OE14/Entities/Mobs/`.

Create: `Resources/Prototypes/_OE14/Entities/Mobs/enemy_goblin.yml`

```yaml
- type: entity
  id: OE14MobGoblin
  name: Goblin
  description: A mischievous goblin raider
  components:
  - type: Transform
  - type: Sprite
    sprite: _OE14/Mobs/Goblin/goblin.rsi
    state: idle
  - type: Physics
    bodyType: Dynamic
  - type: Collidable
  - type: InteractionOutline
  - type: Damageable
    damageContainer: Biological
  - type: Health
    baseHealth: 30
  - type: ActorComponent
  - type: AI
    ai: OE14CombatAI
  - type: MeleeWeapon
    damage:
      types:
        Slash: 5
  - type: Examinable
    description: A small green creature, looks aggressive
```

### Step 2: Create Sprite Assets

Create the sprite RSI for the goblin with appropriate states (idle, walk, attack, death).

### Step 3: Test Spawning

```bash
/spawn OE14MobGoblin
```

## Adding a Game System

For complex features (new economy mechanics, quest system, etc.), you'll need to create C# code.

### File Structure

```
Content.Shared/_OE14/YourSystem/
├── Components/
│   └── YourSystemComponent.cs
├── Events/
│   └── YourSystemEvent.cs
└── Systems/
    └── YourSystemSystem.cs (both Client and Server)
```

### Example: Simple Item Counter System

**Content.Shared/_OE14/Counter/CounterComponent.cs**

```csharp
namespace Content.Shared._OE14.Counter;

[RegisterComponent]
public sealed partial class CounterComponent : Component
{
    [DataField]
    public int Count = 0;

    [DataField]
    public int MaxCount = 100;
}
```

**Content.Server/_OE14/Counter/CounterSystem.cs**

```csharp
namespace Content.Server._OE14.Counter;

public sealed class CounterSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CounterComponent, ComponentInit>(OnInit);
    }

    private void OnInit(Entity<CounterComponent> entity, ref ComponentInit args)
    {
        // Initialize the counter
        entity.Comp.Count = 0;
    }

    public void Increment(Entity<CounterComponent> entity)
    {
        if (entity.Comp.Count < entity.Comp.MaxCount)
            entity.Comp.Count++;
    }
}
```

## Building and Testing

### Development Build
```bash
dotnet build -c Debug  # Faster, includes debugging info
```

### Release Build
```bash
dotnet build -c Release  # Optimized, smaller size
```

### Running Tests
```bash
dotnet test Content.IntegrationTests
```

## Best Practices

1. **Follow Naming Conventions**
   - Prototypes: `OE14<Type><Name>` (e.g., `OE14WeaponSword`, `OE14MobGoblin`)
   - Components: `<Feature>Component` (e.g., `CombatComponent`)
   - Systems: `<Feature>System` (e.g., `CombatSystem`)

2. **Document Your Code**
   - Add XML comments to public methods
   - Explain complex logic inline
   - Document component fields in YAML

3. **Test Before Committing**
   - Spawn and use your feature in-game
   - Test edge cases and error conditions
   - Verify performance doesn't degrade

4. **Keep Commits Clean**
   - One feature per commit
   - Write meaningful commit messages
   - Reference related systems in the message

5. **Use Version Control**
   ```bash
   git add <files>
   git commit -m "Add feature: Description of what was added and why"
   ```

## Common Issues

### "Entity not found" Error
- **Cause**: Prototype ID referenced doesn't exist
- **Fix**: Check YAML file name and entity ID match

### Sprite doesn't show
- **Cause**: RSI file path incorrect or state doesn't exist
- **Fix**: Verify RSI path and check meta.json for correct state names

### Build errors
- **Cause**: Syntax errors in YAML or C# code
- **Fix**: Check indentation in YAML, verify C# compilation

## Next Steps

- Read the [Entity System Guide](04-Entity-System.md) for deeper understanding
- Explore [existing prototypes](../../Resources/Prototypes/_OE14/) for examples
- Check [Map Creation Guide](03-Map-Creation.md) for world building
- See CLAUDE.md or in-game Guidebook "OpenCode Guide" for dev tools

---

**Co-Authored-By**: Claude Haiku 4.5 <noreply@anthropic.com>
