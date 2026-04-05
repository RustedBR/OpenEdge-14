# Map Creation Guide

This guide teaches you how to create and modify maps in OpenEdge 14. Maps are the worlds where gameplay happens.

## Map File Structure

Maps are stored in `Resources/Maps/_OE14/` as YAML files. The main maps are:

- **Comoss**: The main medieval town (2.8 MB)
- **Venicialis**: A water-based village
- **dev_map.yml**: A small testing map

## Understanding Map Basics

### Coordinates and Grids

OpenEdge 14 uses a grid-based coordinate system:

- **X-axis**: Left (0) to Right (∞)
- **Y-axis**: Down (0) to Up (∞)
- **Z-axis**: Below ground (-∞) to Above ground (∞)

Each tile is 1x1 unit. Most entities occupy a single tile, but large structures span multiple tiles.

### Map Boundaries

A map defines its playable area:

```yaml
id: dev_map
tilemap: dev_map
viewrange: 15
```

- **viewrange**: How far players can see (typically 15-20 tiles)
- **tilemap**: The tileset/background layer

## Creating a New Map

### Step 1: Create the Map File

Create `Resources/Maps/_OE14/my_new_map.yml`:

```yaml
tilemap: my_new_map
viewport:
  x: 0
  y: 0
  width: 100
  height: 100

grids:
- name: default
  tilemap: tile_set_name
  chunksize: 16

spawners:
- id: SpawnPointDefault
  prototype: SpawnPointObservatory
  transforms:
    - coordinates: [50, 50]

entities:
- uid: 1
  components:
  - type: Transform
    coordinates: [25, 25]
  - type: Sprite
    sprite: path/to/sprite.rsi
    state: default
```

### Step 2: Set Up the Tilemap

The tilemap defines what tiles exist. Create or reference a tileset:

```yaml
tilemap: my_new_map
tiles:
  0: floor_wood
  1: wall_stone
  2: grass
  3: water
```

### Step 3: Add Spawn Points

Define where players spawn:

```yaml
spawners:
- id: SpawnPointDefault
  prototype: SpawnPointObservatory
  transforms:
    - coordinates: [50, 50]

- id: SpawnPointGuard
  prototype: SpawnPointGuard
  transforms:
    - coordinates: [40, 50]
```

Each spawn point should be accessible from the lobby.

### Step 4: Populate with Entities

Add trees, buildings, NPCs, etc.:

```yaml
entities:
# Building
- uid: 100
  components:
  - type: Transform
    coordinates: [30, 30]
  - type: MetaData
    name: Tavern
  - type: Sprite
    sprite: _OE14/Buildings/Tavern/tavern.rsi

# Tree
- uid: 101
  components:
  - type: Transform
    coordinates: [60, 20]
  - type: Sprite
    sprite: _OE14/Flora/Trees/oak.rsi

# NPC Merchant
- uid: 102
  prototype: OE14NPCMerchant
  components:
  - type: Transform
    coordinates: [35, 35]
```

## Building a Medieval Town

### Layout Planning

For a town like Comoss, plan the layout:

```
┌─────────────────────────────────────┐
│ Forests      [Tavern]    [Market]   │
│              [Smithy]     [Church]   │
│ [Town Hall]                         │
│              [School]     [Inn]      │
│ Farms        [Crops]      [Crops]   │
└─────────────────────────────────────┘
```

### Creating Structures

**Town Hall (Example)**

```yaml
- uid: 200
  components:
  - type: Transform
    coordinates: [50, 30]
  - type: Sprite
    sprite: _OE14/Buildings/TownHall/town_hall.rsi
    state: default
  - type: Door
    open: false
  - type: Damageable
    damageContainer: Structure
  - type: Health
    baseHealth: 500
```

**Shops and Vendors**

```yaml
# Vendor NPC
- uid: 300
  prototype: OE14NPCMerchantWeapons
  components:
  - type: Transform
    coordinates: [40, 25]

# Shop Counter
- uid: 301
  components:
  - type: Transform
    coordinates: [40, 24]
  - type: Sprite
    sprite: _OE14/Furniture/Counters/counter.rsi
```

### Natural Features

**Forests**

```yaml
# Tree cluster (10 trees)
- uid: 400
  components:
  - type: Transform
    coordinates: [10, 10]
  - type: Sprite
    sprite: _OE14/Flora/Trees/oak.rsi

- uid: 401
  components:
  - type: Transform
    coordinates: [11, 10]
  - type: Sprite
    sprite: _OE14/Flora/Trees/pine.rsi

# ... (repeat for clustering effect)
```

**Farmland**

```yaml
- uid: 450
  components:
  - type: Transform
    coordinates: [70, 60]
  - type: Sprite
    sprite: _OE14/Flora/Crops/wheat.rsi

- uid: 451
  components:
  - type: Transform
    coordinates: [70, 61]
  - type: Sprite
    sprite: _OE14/Flora/Crops/corn.rsi
```

**Water Features**

```yaml
# River
- uid: 500
  components:
  - type: Transform
    coordinates: [50, 0]
  - type: Sprite
    sprite: _OE14/Environment/Water/river.rsi
```

## Using Multiple Levels (Z-Levels)

For dungeons or multi-story buildings:

```yaml
entities:
# Above ground
- uid: 1000
  components:
  - type: Transform
    coordinates: [50, 50]
    z: 0  # Ground level
  - type: Sprite
    sprite: _OE14/Buildings/Tower/tower_ground.rsi

# First floor
- uid: 1001
  components:
  - type: Transform
    coordinates: [50, 50]
    z: 1  # One level up
  - type: Sprite
    sprite: _OE14/Buildings/Tower/tower_first.rsi

# Underground
- uid: 1002
  components:
  - type: Transform
    coordinates: [50, 50]
    z: -1  # One level down
  - type: Sprite
    sprite: _OE14/Dungeon/dungeon_level_1.rsi
```

Players can move between levels using stairs/elevators.

## Advanced Features

### Ambient Sound

Add atmosphere to your maps:

```yaml
- uid: 2000
  components:
  - type: Transform
    coordinates: [50, 50]
  - type: AmbientSound
    sound:
      path: /Audio/Ambience/town_ambience.ogg
      params:
        volume: -5
```

### Lighting

Dynamic lighting for mood:

```yaml
- uid: 2100
  components:
  - type: Transform
    coordinates: [40, 30]
  - type: PointLight
    radius: 5
    energy: 2
    color: "#FF8800"  # Warm orange (lantern)
```

### Interactive Elements

Doors, chests, levers:

```yaml
# Locked Door
- uid: 2200
  components:
  - type: Transform
    coordinates: [50, 45]
  - type: Door
    open: false
  - type: AccessReader
    access:
    - "bar"  # Requires "bar" access

# Chest
- uid: 2201
  prototype: OE14StorageChest
  components:
  - type: Transform
    coordinates: [50, 44]
  - type: Storage
    maxSize: 30
```

### Procedural Spawning (For Dungeons)

Use spawners to create encounters dynamically:

```yaml
- uid: 3000
  prototype: OE14SpawnerEnemies
  components:
  - type: Transform
    coordinates: [70, 70]
  - type: RandomEntitySpawner
    prototypes:
    - OE14MobGoblin
    - OE14MobOrc
    - OE14MobSkeletonWarrior
    spawnAttempts: 3
    spawnRange: 5
```

## Performance Optimization

### Chunk Loading

Maps are divided into chunks (typically 16x16 tiles) for performance:

```yaml
grids:
- name: default
  tilemap: main_tileset
  chunksize: 16  # Only load nearby chunks
```

### Entity Limits

Too many entities cause lag. Optimize by:

1. Using sprites instead of full entities for decoration
2. Pooling entities (reuse instead of creating new ones)
3. Using Z-levels to separate areas

### Testing Performance

```bash
dotnet build -c Release
# Connect and use admin panel: /adminpanel
# Monitor entity count and frame rate
```

## Map Configuration

### Difficulty Settings

Mark areas by difficulty:

```yaml
- uid: 4000
  components:
  - type: Transform
    coordinates: [80, 80]
  - type: MetaData
    name: Dangerous Dungeon
  - type: Tag
    tags:
    - HighDanger  # Custom tag
```

### Safe Zones

Prevent combat in towns:

```yaml
- uid: 4100
  components:
  - type: Transform
    coordinates: [50, 50]
  - type: SafeZone
    allowCombat: false
    allowHarm: false
```

## Testing Your Map

### Load Map

```bash
# In game admin console
/loadmap my_new_map
```

### Validate Spawn Points

```bash
# Check all spawn points are accessible
/spawntest my_new_map
```

### Performance Check

```bash
# Monitor while playing
# Look for: entity count, chunk loading, frame rate
```

## Debugging Map Issues

### Entity Doesn't Appear

1. Check coordinates are within map bounds
2. Verify sprite path exists
3. Check Z-level (might be invisible on another level)

### Performance Lag

1. Count entities in area
2. Check for spawners creating too many entities
3. Reduce lighting effects or shadow complexity

### Spawn Point Issues

1. Ensure spawn point is on passable terrain
2. Check z-level of spawn point
3. Test with `/spawn` command on that location

## Common Map Patterns

### Town Center
- Large open square (30x30 tiles)
- Tavern, market stalls, notice boards
- Town Hall for governance
- Surrounding shops and services

### Residential Area
- Grid of small houses (10x10 each)
- Streets with sidewalks
- Gardens and parks

### Dungeon
- Interconnected rooms
- Multiple levels (z-levels)
- Enemy spawners
- Treasure chests
- Boss arena

### Wilderness
- Random terrain generation or manual placement
- Forests, mountains, water
- Occasional ruins or NPCs
- Resource nodes (ore, trees, crops)

## Next Steps

- Explore existing maps in `Resources/Maps/_OE14/`
- Create your first small test map (20x20 tiles)
- Use the town planning template above to design a settlement
- Read the [Entity System Guide](04-Entity-System.md) for custom behaviors

## Resources

- **SS14 Wiki**: https://docs.spacestation14.io/en/robust-toolbox/tilemap/
- **Example Maps**: `Resources/Maps/_OE14/`
- **Sprite Assets**: `Resources/Textures/_OE14/`

---

**Co-Authored-By**: Claude Haiku 4.5 <noreply@anthropic.com>
