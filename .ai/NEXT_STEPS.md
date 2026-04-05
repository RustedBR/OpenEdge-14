# Next Steps — OpenEdge 14

Generated after Session 16 (2026-04-03). Updated with localization work.

---

## 1. Localization — FTL Files

**Goal**: Complete pt-BR localization for all FTL files.

**Status** (as of 2026-04-04):
- ✅ Entity translations: 1,096 entities translated (weapons, tools, materials, mobs, spells, structures, misc)
- ✅ UI strings: alerts, info-window, lobby, ghost, changelog, generic, shell
- ✅ Game systems: game presets (15), damage types, round-end, objectives
- ✅ Administration: 61 files
- ✅ Crafting/Construction: construction categories, chemistry, medical, cargo, botany
- ✅ Recently translated (Session 17-18):
  - quick-dialog, vote-manager, verb-system, station-ai
  - character-tabs, options-menu, artifact-analyzer, gun, pda
  - toilet, vote-commands, robotics-console, node-scanner
  - sandbox/sandbox-manager.ftl, stack-component.ftl, door-remote.ftl
  - store/currency.ftl, revenant.ftl, artifact-hints.ftl
  - recipes/*.ftl, robotics/borg_modules.ftl, stack/stacks.ftl
  - door-*.ftl, doorjack.ftl, door-welded.ftl
- ✅ Build: 0 errors

**Files modified**: 200+ FTL files translated

---

## 2. Spell Memory Cost → Status Points

**Goal**: All spells purchased with "memory" that cost 0.5 should grant 1 status point as compensation.

**Context**:
- Character creation allows spending "memory cost" (fractional spell purchases)
- Some spells cost 0.5 memory, leaving the remainder wasted
- Need to give back stat points for this fractional cost

**What to do**:
- Identify all spells with `memoryCost: 0.5` in prototypes
- When such a spell is learned/purchased, grant 1 AvailablePoint to character stats
- Ensure this happens during character creation spell selection
- Track where spell learning happens (OE14SkillSystem? OE14SpellStorage?)

**Files to check**:
- `Content.Server/_OE14/Skill/OE14SkillSystem.cs`
- `Content.Server/_OE14/Spell/OE14SpellStorageSystem.cs` (or equivalent)
- `Resources/Prototypes/_OE14/Spell/` — grep for `memoryCost: 0.5`

---

## 2. Overflow Protection for Spell Bonuses

**Goal**: Ensure stat point overflow protection works correctly when spells grant stat modifiers.

**Context**:
- Current system: When effective stat exceeds cap, overflow grants AvailablePoints
- Metamagic spell applies +INT modifiers correctly
- Need to verify ALL spell stat bonuses follow the same pattern

**What to do**:
1. Review `OE14CharacterStatsSystem.ApplyStatModifier()` logic
2. Ensure spell bonus application follows Metamagic pattern exactly
3. Test: Grant spell with +3 INT to character with INT 8 + modifier +2 (effective 10)
   - Expected: Overflow = 1 point, refunded as AvailablePoints
4. Log all stat modifiers with their sources for debugging

**Follow Metamagic Logic**:
- Metamagic grants +1 INT per tier
- Applied via `ApplyStatModifier(entity, "intelligence", +1)`
- Triggers overflow check automatically
- Should not be reapplied on spawn (use ComponentInit, not MapInitEvent)

---

## 3. Spell List by Race — Documentation

✅ **COMPLETED** — See `.ai/SPELL_INVENTORY_BY_RACE.md`

**Summary of findings**:
- **Elf**: STR 3, VIT 3, DEX 5, INT 8 → Free spells (Metamagic T1/T2) cap INT at 10
- **Human**: STR 5, VIT 5, DEX 5, INT 5 → No free spells
- **Silva**: STR 2, VIT 5, DEX 6, INT 7 → Free spells (Healing T1/T2) give NO stat bonus
- **Carcat**: STR 6, VIT 5, DEX 8, INT 2 → No free spells
- **Goblin**: STR 4, VIT 4, DEX 8, INT 2 → Free spells (Athletic T1/T2) cap DEX at 10
- **Tiefling**: STR 5, VIT 5, DEX 4, INT 6 → Free spells (Pyrokinetic T1/T2) give NO stat bonus
- **Dwarf**: STR 7, VIT 8, DEX 3, INT 3 → No free spells

**Spells costing 0.5 memory with stat bonuses**:
- MetamagicT1/T2: +1 INT each
- AthleticT1/T2: +1 DEX each
- HealingT1/T2: No bonus ❌
- PyrokineticT1/T2: No bonus ❌

**Key imbalance identified**:
- Elf and Goblin get pre-capped stats (INT and DEX respectively) from free spells
- Other races can spend all 3 points freely
- Healing/Pyrokinetic don't grant stat compensation despite costing 0.5 memory

---

## 4. Rebalance Race Stats

**Goal**: Adjust initial stat allocation to account for free spells that grant bonuses.

**Context**:
- Elf gets +2 INT from Metamagic (T1+T2), so starts at effective INT 10 (capped)
- Other races may have spell bonuses that push them to 10 without point spending
- Need to "redistribute" those effective points back to other stats or give them as spendable points

**What to do**:
1. Complete Step 3 (Spell List by Race) first
2. For each race:
   - Calculate current base stats + free spell bonuses = effective stats
   - If any stat is capped at 10, note the "wasted" modifier
   - Rebalance: Move base points from capped stat to uncapped stat
   - Ensure total base points distributed remain constant
3. Update race prototypes: Modify `OE14CharacterStats` initial values
4. Test: Verify character spawns with correct effective stats after rebalance

**Example**:
- **Elf current**: STR 3, VIT 3, DEX 5, INT 8 + Metamagic (+2 INT)
- **Elf effective**: STR 3, VIT 3, DEX 5, INT 10 (capped, +2 wasted)
- **Elf rebalanced**: STR 3, VIT 3, DEX 7, INT 8 (same effective after +2 = 10, but DEX gets the base points back)

---

## 5. Human — 5 Unspent Points

**Goal**: Humans get 5 available points instead of 3 to match other races' "effective point pool".

**Context**:
- Currently all races get 3 points to spend in character creation
- Humans have no free spells, so 3 points = true spendable
- Other races get 3 points + free spell bonuses (some stats capped immediately)
- To make Humans competitive: Give them 5 points instead

**What to do**:
1. Find Human race prototype in `Resources/Prototypes/_OE14/Species/`
2. Locate `OE14CharacterStats` component in Human definition
3. Change `AvailablePoints: 3` → `AvailablePoints: 5`
4. Test: Verify Human spawns with 5 points in character creation UI
5. Update CLAUDE.md race stats table to note: "Human: 5 points, Others: 3 points"

**File to modify**:
- `Resources/Prototypes/_OE14/Species/human.yml` (or wherever Human is defined)

---

## Priority Order

1. **First**: Complete Step 3 (Spell List) — required input for Steps 4 & 5
2. **Second**: Step 5 (Human 5 points) — simple, quick win
3. **Third**: Step 4 (Rebalance races) — depends on Step 3
4. **Fourth**: Step 2 (Overflow protection) — verify existing logic works
5. **Fifth**: Step 1 (0.5 memory → point) — add new feature

---

## Notes for Next Session

- **Server deployment**: Kill first (`killall -9 Content.Server`), then compile, then restart
- **Stat row consistency**: All 4 rows (STR/VIT/DEX/INT) in XAML must have identical structure
- **Character Window**: Currently working perfectly after Session 11 fix
- **Known working**: Metamagic overflow protection, character creation bonuses, modifier tracking
- **Testing tips**: Use Elf for easiest testing (has visible spell bonuses), spawn with dev map or character creation UI

---

## Files to Review Before Starting

1. `CLAUDE.md` — Project overview and known issues
2. `.ai/errors.md` — Technical errors and fixes (especially "Stat modifier overflow")
3. `.ai/decisions.md` — Architecture patterns for stat systems
4. `Content.Server/_OE14/CharacterStats/OE14CharacterStatsSystem.cs` — Core stat logic
5. `Resources/Prototypes/_OE14/Species/*.yml` — Race definitions
6. `Resources/Prototypes/_OE14/Skill/*.yml` — Spell/skill definitions
