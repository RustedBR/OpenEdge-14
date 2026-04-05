# Spell Inventory by Race — OpenEdge 14

Complete inventory of races, their initial stats, free spells, and calculated effective stats after spell bonuses.

**Last Updated**: 2026-03-24 (Session 13 - Added T3 trees, Illusion/Hydrosophistry bonuses, TraderWit/AlchemyVision)

---

## Summary Table

| Race | Base STR | Base VIT | Base DEX | Base INT | Free Spells | Effective STR | Effective VIT | Effective DEX | Effective INT | Available Points | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Human | 5 | 5 | 5 | 5 | None | 5 | 5 | 5 | 5 | 5 | Neutral baseline, no bonuses, 5 spendable points |
| Elf | 6 | 6 | 1 | 7 | MetamagicT1/T2 | 6 | 6 | 1 | 9 | 3 | INT bonus from spells |
| Silva | 1 | 7 | 5 | 7 | HealingT1/T2 | 1 | 9 | 5 | 7 | 3 | VIT bonus from spells (7 base + 2 = 9 effective) |
| Carcat | 5 | 5 | 7 | 3 | ElectromancyT1/T2 | 5 | 5 | 9 | 3 | 3 | DEX bonus from spells (7 base + 2 = 9 effective) |
| Goblin | 6 | 6 | 7 | 1 | AthleticT1/T2 | 6 | 6 | 9 | 1 | 3 | DEX bonus from spells (7 base + 2 = 9 effective) |
| Tiefling | 5 | 5 | 2 | 6 | PyrokineticT1/T2 | 7 | 5 | 2 | 6 | 3 | STR bonus from spells (5 base + 2 = 7 effective) |
| Dwarf | 7 | 7 | 2 | 3 | HydrosophistryT1/T2 | 7 | 9 | 2 | 3 | 3 | VIT bonus from spells (7 base + 2 = 9 effective) |

**Notes**:
- All races with free spells now get +2 effective in one stat (T1 + T2 each grant +1)
- Human gets 5 available points (compensates for no free spells)
- All other races get 3 available points
- No stats are pre-capped at 10; all races can spend their points freely

---

## Detailed Breakdown

### Human
**Base Stats**: STR 5 | VIT 5 | DEX 5 | INT 5
**Free Spells**: None
**Available Points**: 5

**Effective Stats** (after spells): STR 5 | VIT 5 | DEX 5 | INT 5

**Derived Values**:
- HP: 100 crit / 200 dead (neutral VIT)
- Mana: 100 (neutral INT)
- Stamina: 100 crit (neutral DEX)
- Damage: 1.0x (neutral STR)

**Special Notes**:
- Baseline for comparison. No free skills or stat bonuses.
- Gets 5 spendable points instead of 3 (compensation for no free spells)

---

### Elf
**Base Stats**: STR 6 | VIT 6 | DEX 1 | INT 7
**Free Spells**:
- MetamagicT1 (learnCost: 0.5) → +1 INT
- MetamagicT2 (learnCost: 0.5) → +1 INT
- OE14ActionSpellManaTrance (learnCost: 0) → No stat bonus

**Spell Bonuses**:
- MetamagicT1: +1 INT
- MetamagicT2: +1 INT
- **Total INT bonus: +2**

**Available Points**: 3

**Effective Stats** (after spells):
- STR: 6
- VIT: 6
- DEX: 1
- INT: 7 + 2 = **9** (optimized mage stat)

**Derived Values**:
- HP: 125 crit / 250 dead (VIT 6)
- Mana: 175 (INT 9)
- Stamina: 25 crit (DEX 1, very low)
- Damage: 1.25x (STR 6)

**Special Notes**:
- INT gets +2 from free spells
- Can spend 3 points on any stat without capping
- Penalty: Very low DEX (1)
- Arcane specialist build

---

### Silva
**Base Stats**: STR 1 | VIT 7 | DEX 5 | INT 7
**Free Spells**:
- HealingT1 (learnCost: 0.5) → +1 VIT
- HealingT2 (learnCost: 0.5) → +1 VIT

**Spell Bonuses**:
- HealingT1: +1 VIT
- HealingT2: +1 VIT
- **Total VIT bonus: +2**

**Available Points**: 3

**Effective Stats** (after spells):
- STR: 1
- VIT: 7 + 2 = **9** (optimized healer stat)
- DEX: 5
- INT: 7

**Derived Values**:
- HP: 150 crit / 300 dead (VIT 9, very tanky)
- Mana: 150 (INT 7)
- Stamina: 100 crit (DEX 5)
- Damage: 0.5x (STR 1, very weak)

**Special Notes**:
- VIT gets +2 from free spells (support/healer specialization)
- Can spend 3 points freely
- Weakness: Very low STR (1)
- Nature spirit/healer archetype

---

### Carcat
**Base Stats**: STR 5 | VIT 5 | DEX 7 | INT 3
**Free Spells**:
- ElectromancyT1 (learnCost: 0.5) → +1 DEX
- ElectromancyT2 (learnCost: 0.5) → +1 DEX

**Spell Bonuses**:
- ElectromancyT1: +1 DEX
- ElectromancyT2: +1 DEX
- **Total DEX bonus: +2**

**Available Points**: 3

**Effective Stats** (after spells):
- STR: 5
- VIT: 5
- DEX: 7 + 2 = **9** (optimized agility stat)
- INT: 3

**Derived Values**:
- HP: 100 crit / 200 dead (VIT 5, neutral)
- Mana: 50 (INT 3, very low)
- Stamina: 175 crit (DEX 9, excellent)
- Damage: 1.0x (STR 5, neutral)

**Special Notes**:
- DEX gets +2 from free spells
- Can spend 3 points freely
- Feline specialization: high DEX and mobility
- Minimal magic capability (INT 3)
- Physical combatant archetype

---

### Goblin
**Base Stats**: STR 6 | VIT 6 | DEX 7 | INT 1
**Free Spells**:
- AthleticT1 (learnCost: 0.5) → +1 DEX
- AthleticT2 (learnCost: 0.5) → +1 DEX

**Spell Bonuses**:
- AthleticT1: +1 DEX
- AthleticT2: +1 DEX
- **Total DEX bonus: +2**

**Available Points**: 3

**Effective Stats** (after spells):
- STR: 6
- VIT: 6
- DEX: 7 + 2 = **9** (optimized agility stat)
- INT: 1

**Derived Values**:
- HP: 125 crit / 250 dead (VIT 6)
- Mana: 25 (INT 1, zero magic)
- Stamina: 175 crit (DEX 9, excellent)
- Damage: 1.25x (STR 6)

**Special Notes**:
- DEX gets +2 from free spells
- Can spend 3 points freely
- Athletic/nimble specialization
- Strong melee, no magic

---

### Tiefling
**Base Stats**: STR 5 | VIT 5 | DEX 2 | INT 6
**Free Spells**:
- PyrokineticT1 (learnCost: 0.5) → +1 STR
- PyrokineticT2 (learnCost: 0.5) → +1 STR

**Spell Bonuses**:
- PyrokineticT1: +1 STR
- PyrokineticT2: +1 STR
- **Total STR bonus: +2**

**Available Points**: 3

**Effective Stats** (after spells):
- STR: 5 + 2 = **7** (optimized strength stat)
- VIT: 5
- DEX: 2
- INT: 6

**Derived Values**:
- HP: 100 crit / 200 dead (VIT 5, neutral)
- Mana: 125 (INT 6)
- Stamina: 50 crit (DEX 2, low)
- Damage: 1.5x (STR 7)

**Special Notes**:
- STR gets +2 from free spells
- Can spend 3 points freely
- Infernal heritage: strong melee + fire magic
- Special synergy: Pyrokinetic also grants +STR for physical fire abilities
- Weakness: Low DEX (2) reduces stamina

---

### Dwarf
**Base Stats**: STR 7 | VIT 7 | DEX 2 | INT 3
**Free Spells**:
- HydrosophistryT1 (learnCost: 0.5) → +1 VIT
- HydrosophistryT2 (learnCost: 0.5) → +1 VIT

**Spell Bonuses**:
- HydrosophistryT1: +1 VIT
- HydrosophistryT2: +1 VIT
- **Total VIT bonus: +2**

**Available Points**: 3

**Effective Stats** (after spells):
- STR: 7
- VIT: 7 + 2 = **9** (optimized tank stat)
- DEX: 2
- INT: 3

**Derived Values**:
- HP: 150 crit / 300 dead (VIT 9, extremely tanky)
- Mana: 50 (INT 3, very low)
- Stamina: 50 crit (DEX 2, low)
- Damage: 1.5x (STR 7)

**Special Notes**:
- VIT gets +2 from free spells
- Can spend 3 points freely
- Tanky specialist: high HP, high damage, low mobility
- Minimal magic capability
- Strong melee tank archetype

---

## Analysis: All Spells with 0.5 Cost Now Grant +1 Stat Bonus

### Spells That Grant Stat Bonuses (ALL 0.5 cost spells now included)

| Spell | Stat | Bonus | Races with Free Access |
|---|---|---|---|
| MetamagicT1 | INT | +1 | Elf |
| MetamagicT2 | INT | +1 | Elf |
| AthleticT1 | DEX | +1 | Goblin |
| AthleticT2 | DEX | +1 | Goblin |
| HealingT1 | VIT | +1 | Silva |
| HealingT2 | VIT | +1 | Silva |
| PyrokineticT1 | STR | +1 | Tiefling |
| PyrokineticT2 | STR | +1 | Tiefling |
| ElectromancyT1 | DEX | +1 | Carcat |
| ElectromancyT2 | DEX | +1 | Carcat |
| HydrosophistryT1 | VIT | +1 | Dwarf |
| HydrosophistryT2 | VIT | +1 | Dwarf |

**Key Change**: As of this session, ALL spells with `learnCost: 0.5` now grant +1 stat bonus. This was implemented per user request: "Coloque bonus de um status em todas as spells que custam 0.5 de memória."

---

## Rebalancing Summary

### Distribution Pattern

All races now follow a **min-max design** where each race is optimized for one stat but penalized in another:

| Race | Max Stat | Max Value | Min Stat | Min Value | Spell Bonus Target | Effective in Max |
|---|---|---|---|---|---|---|
| Elf | INT | 7 (+2) | DEX | 1 | INT | 9 |
| Silva | VIT | 7 (+2) | STR | 1 | VIT | 9 |
| Carcat | DEX | 7 (+2) | INT | 3 | DEX | 9 |
| Goblin | DEX | 7 (+2) | INT | 1 | DEX | 9 |
| Tiefling | STR | 5 (+2) | DEX | 2 | STR | 7 |
| Dwarf | VIT | 7 (+2) | INT | 3 | VIT | 9 |
| Human | NEUTRAL | 5 | NEUTRAL | 5 | None | 5 |

### Stat Point Distribution

All races (except Human) start with exactly **20 base stat points**:
- Min-max races: 7+7+1+5=20 or 7+6+6+1=20 pattern
- Human: 5+5+5+5=20 (but gets 5 spendable points instead of 3)

### Overflow Protection

With the rebalancing:
- No race pre-caps at 10 with just base stats
- When free spells grant +2 bonus, effective stat reaches 9
- Characters can spend remaining 3 points freely to reach 10 if desired
- Overflow protection ensures no stat exceeds 10 maximum

---

## Character Creation Point Distribution

| Race | Available Points | Reason |
|---|---|---|
| Human | 5 | No free spells; extra 2 points to remain competitive |
| All Others | 3 | Balanced against free spell bonuses |

---

## File References

**Character Stats Definitions**:
- Base: `Resources/Prototypes/_OE14/Entities/Mobs/Species/base.yml`
- Elf: `Resources/Prototypes/_OE14/Entities/Mobs/Species/elf.yml`
- Human: `Resources/Prototypes/_OE14/Entities/Mobs/Species/human.yml`
- Silva: `Resources/Prototypes/_OE14/Entities/Mobs/Species/silva.yml`
- Carcat: `Resources/Prototypes/_OE14/Entities/Mobs/Species/carcat.yml`
- Goblin: `Resources/Prototypes/_OE14/Entities/Mobs/Species/goblin.yml`
- Tiefling: `Resources/Prototypes/_OE14/Entities/Mobs/Species/tiefling.yml`
- Dwarf: `Resources/Prototypes/_OE14/Entities/Mobs/Species/dwarf.yml`

**Spell/Skill Definitions**:
- Metamagic: `Resources/Prototypes/_OE14/Skill/Basic/metamagic.yml`
- Athletic: `Resources/Prototypes/_OE14/Skill/Basic/athletic.yml`
- Healing: `Resources/Prototypes/_OE14/Skill/Basic/healing.yml`
- Pyrokinetic: `Resources/Prototypes/_OE14/Skill/Basic/pyrokinetic.yml`
- Electromancy: `Resources/Prototypes/_OE14/Skill/Basic/electromancy.yml`
- Hydrosophistry: `Resources/Prototypes/_OE14/Skill/Basic/hydrosophistry.yml`

**Stat Bonus Effect**:
- `Content.Shared/_OE14/Skill/Effects/OE14AddStatBonus.cs` (always grants +1 to specified stat)

---

## Tier 3 (T3) Spell Trees

**Two spell trees have a third tier available**:

### Metamagic T3
- **Cost**: 0.5 (requires MetamagicT2 as prerequisite)
- **Bonus**: +1 INT
- **Available to**: Elf (and anyone else who learns it)
- **Effect**: Grants additional intelligence bonus, stackable with T1 and T2

### Athletic T3
- **Cost**: 0.5 (requires AthleticT2 as prerequisite)
- **Bonus**: +1 DEX
- **Available to**: Goblin (and anyone else who learns it)
- **Effect**: Grants additional dexterity bonus, stackable with T1 and T2

**Max Potential Bonuses from T3 Trees**:
- Elf with Metamagic T1+T2+T3: +3 INT (up from +2)
- Goblin with Athletic T1+T2+T3: +3 DEX (up from +2)

---

## Job Stat Bonuses (Session 13)

All jobs granting stat bonuses from learned skills:

| Job | Skill | Bonus | Cost |
|---|---|---|---|
| OE14Innkeeper | TraderWit | +1 VIT | 1.0 |
| OE14Alchemist | AlchemyVision | +1 INT | 1.0 |
| OE14Alchemist | TraderWit | +1 VIT | 1.0 |
| OE14Blacksmith | TraderWit | +1 VIT | 1.0 |
| OE14Apprentice | AlchemyVision | +1 INT | 1.0 |
| OE14Merchant | TraderWit | +1 VIT | 1.0 |

---

## Updated Spell Bonus Summary (Session 13)

**All 0.5-cost spells** now grant +1 stat bonus:

| Tree | T1 | T2 | T3 | Bonus |
|---|---|---|---|---|
| Metamagic | ✓ | ✓ | ✓ | +INT |
| Pyrokinetic | ✓ | ✓ | - | +STR |
| Healing | ✓ | ✓ | - | +VIT |
| Athletic | ✓ | ✓ | ✓ | +DEX |
| Electromancy | ✓ | ✓ | - | +DEX |
| **Illusion** | ✓ | ✓ | - | **+VIT** (NEW) |
| **Hydrosophistry** | ✓ | ✓ | - | **+STR** (CHANGED from +VIT) |

**All 1.0-cost skills** now grant +1 stat bonus:

| Skill | Bonus |
|---|---|
| TraderWit | +VIT |
| AlchemyVision | +INT |

---

## Session Completion Status

✅ **All major stat system changes completed**:
1. ✅ Character creation UI aligned with in-game Character window
2. ✅ Spell modifiers displayed in character creation
3. ✅ "Current" values include spell bonuses
4. ✅ Humans get 5 creation points
5. ✅ All races rebalanced with min-max design
6. ✅ All 0.5-cost spells now grant +1 stat bonus
7. ✅ No stat pre-capping issues (all stats can be spent to 10)

**Status**: Ready for testing and gameplay validation.

