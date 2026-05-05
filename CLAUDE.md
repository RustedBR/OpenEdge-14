# CLAUDE.md — OpenEdge 14 AI Developer Reference

This file is the primary context document for AI developers (Claude/OpenCode) working on this project.
Read this before touching any file. Keep it updated as the project evolves.

---

## Project Identity

- **Name**: OpenEdge 14
- **Base**: Fork/clone of Ss14-Medieval (Space Station 14 framework)
- **Owner**: RustedBR
- **Repo**: https://github.com/RustedBR/OpenEdge-14
- **Engine**: RobustToolbox (.NET 9, C#)
- **Theme**: Medieval fantasy sandbox multiplayer RPG

---

## OpenCode Configuration

This project uses **OpenCode** as the primary AI development tool with the following configuration:

### Plugins Installed (Global)
Located in: `~/.config/opencode/opencode.json`

```json
{
  "plugin": [
    "@howaboua/opencode-background-process@latest",
    "@jredeker/opencode-morph-fast-apply@latest"
  ]
}
```

### Available Commands (Custom)
Located in: `~/.config/opencode/commands/`

| Command | Description |
|---------|-------------|
| `/plan` | Switch to plan mode (read-only analysis) |
| `/doctor` | Run diagnostics on the project |
| `/build` | Build the project (Release) |
| `/server` | Start game server |
| `/kill-server` | Stop game server |
| `/review` | Review code for bugs and issues |
| `/status` | Check git status |
| `/diff` | View current changes |
| `/test` | Run tests |

### Background Process Tools
The `opencode-background-process` plugin provides:

- `background_process_launch` - Start server/commands in background
- `background_process_list` - List running background processes
- `background_process_read` - Read output from background process
- `background_process_kill` - Stop a background process

**Example Usage:**
```
Start the server in background
→ Uses background_process_launch with id "ss14-server"
→ Server runs without blocking conversation
→ Use background_process_read to check logs
```

### Skills (Global)
Located in: `~/.claude/skills/`

| Skill | Description |
|-------|-------------|
| `start-server` | Starts game server in background |
| `file-search` | Searches files in background |
| `process-manager` | Manages background processes |

---

## Workspace Layout

```
OpenEdge-14/
├── CLAUDE.md                        ← YOU ARE HERE — read first
├── .ai/                             ← AI working files (logs, notes, decisions)
│   ├── errors.md                    ← Known errors and their fixes
│   ├── session-log.md               ← Per-session notes
│   └── decisions.md                 ← Architecture decisions made
├── Content.Server/                  ← Server-side C# systems
├── Content.Client/                  ← Client-side C# systems
├── Content.Shared/                  ← Shared C# (both sides)
│   └── _OE14/                       ← All OpenEdge custom systems
├── Resources/
│   ├── Prototypes/
│   │   ├── _OE14/                   ← OE14 YAML entity/prototype definitions
│   │   └── Guidebook/
│   │       └── oe14_systems.yml     ← Guidebook entry registrations (our file)
│   ├── ServerInfo/
│   │   └── Guidebook/               ← Guidebook XML pages (our files: OE14_*.xml)
│   └── Locale/
│       └── en-US/_OE14/guidebook/   ← Localization strings for guidebook
├── Resources/Maps/_OE14/            ← Maps: Comoss, Venicialis, dev_map
├── DOCS/                            ← Human-readable documentation
│   ├── Gameplay/
│   └── Development/
└── runserver.sh / runclient.sh      ← Dev server/client launchers
```

---

## How to Run

### Using OpenCode (Recommended)
```bash
# Start OpenCode in the project directory
cd /home/rusted/Git/OpenEdge-14
opencode

# Within OpenCode, you can:
# - Say "start server" to launch game server in background
# - Say "show server logs" to check server output
# - Use /build command to build the project
# - Use /server command to start the server
```

### Manual Commands
```bash
# Build
dotnet build -c Release

# Run server (dev mode)
bash runserver.sh

# Run client (dev mode)
bash runclient.sh

# Kill server on port 1212
killall Content.Server
# or
lsof -ti:1212 | xargs kill -9
```

---

## Development Workflow with OpenCode

### Starting a Development Session
1. Run `opencode` in the project directory
2. Read CLAUDE.md first (automatically loaded)
3. Use `/build` to verify project compiles
4. Use `/server` or say "start server" to launch game

### Key OpenCode Features for This Project
- **Background Processes**: Use for long-running tasks like server
- **File Search**: Say "find files containing X" for code search
- **Code Review**: Use `/review` on specific files
- **Plan Mode**: Use `/plan` or Shift+Tab for complex tasks

### Important Notes
- OpenCode uses `background_process_*` tools from the plugin
- Server runs in background - doesn't block conversation
- Check logs anytime with "show server logs" or `background_process_read`
- Kill server with "stop server" or `background_process_kill`

---

## Code Rules

- **Language**: All code, YAML, and comments written in **English**
- **Commit messages**: Portuguese is acceptable for commit messages
- **Co-author every commit**: `Co-Authored-By: Claude Haiku 4.5 <noreply@anthropic.com>`
- **AI policy**: This is a personal fork — AI-assisted code is allowed. Do NOT push AI code to upstream Ss14-Medieval.
- **Build check**: Always run `dotnet build` before committing. 0 errors required.

## Git Commit Policy — **IMPORTANT**

**RULE: NEVER make a commit without explicit permission from the user.**

1. **Before committing**, always ask the user for approval:
   - Show what will be committed: `git status` and `git diff`
   - Explain what changed and why
   - Wait for explicit confirmation ("yes", "ok", "proceed", etc.)

2. **Never commit automatically** when:
   - The session ends
   - Tests pass
   - Build succeeds
   - Changes are "complete"
   - Any other automated trigger

3. **Always require explicit permission** even if:
   - User said "implement this feature" (that's implementation approval, not commit approval)
   - Changes are small or "obvious"
   - Previous commits were already made (each commit needs fresh approval)
   - The branch will be for a PR (commits still need approval)

4. **Acceptable commit workflows**:
   - ✅ User: "Commit this" → make commits with user message
   - ✅ User: "Implement feature X and commit it" → implement + show changes + wait for "yes" → commit
   - ✅ User: "Just implement, don't commit" → implement → stop (no commit)
   - ❌ User: "Implement feature X" → implement + auto-commit (WRONG!)

5. **After committing**, always:
   - Show the commit hash: `git log --oneline -1`
   - Confirm commit was successful
   - Ask if user wants to push or make a PR

---

## Guidebook System — How It Works

The in-game guidebook is a multi-layer system. All three layers must be correct for a page to appear.

### Layer 1 — XML Content Page
- **Location**: `Resources/ServerInfo/Guidebook/OE14_*.xml`
- **Format**: Must start with `<Document>` (capital D), end with `</Document>`
- **Allowed markup**: `# H1`, `## H2`, `### H3`, `[bold]text[/bold]`, `[color=#hex]text[/color]`, `<Box>`, `<GuideEntityEmbed Entity="ID"/>`
- **FORBIDDEN**: backticks ` ``` `, markdown `**bold**`, `{`, `}` inside text, `<?xml ...?>` headers, `<section>`, `<header>`, `<NavSection>`, `<CDATA>`

### Layer 2 — Prototype Entry (YAML)
- **Location**: `Resources/Prototypes/_OE14/Guidebook/entry.yml` or `Resources/Prototypes/Guidebook/oe14_systems.yml`
- **Required fields**: `type: guideEntry`, `id`, `name` (plain text, NOT a localization key), `text` (path to XML)
- **OE14 flag**: Add `openEdgeAllowed: true` so the entry shows under the OpenEdge guidebook tree. **CRITICAL**: `openEdgeAllowed: false` means the entry is NEVER visible, regardless of `locFilter`.
- **Language filter**: Use `locFilter: "pt-BR"` or `locFilter: "en-US"` to show entries only in specific languages. If `locFilter` is `null`, the entry appears in all languages.
- **Children**: list child entry IDs to create a nested tree

### Layer 3 — Parent Registration
- The entry must be listed as a `children:` item of a visible parent entry
- **EN tree**: `OE14_EN` → `OE14GameSystems` → individual entries
- **PT-BR tree**: `OE14_PT` → `OE14_PT_Species`, `OE14_PT_Jobs`, `OE14_PT_GameSystems` → individual entries
- **Development tree**: `OE14_PT_DEVELOPMENT` → development entries (all `openEdgeAllowed: true`)

### Guidebook XML Locations by Language

| Language | XML Location | YAML Entry Location |
|---|---|---|
| English | `Resources/ServerInfo/Guidebook/OE14_*.xml` | `Resources/Prototypes/Guidebook/oe14_systems.yml` |
| English (SS14 upstream) | `Resources/ServerInfo/_OE14/Guidebook_EN/` | `Resources/Prototypes/_OE14/Guidebook/Eng/` |
| Portuguese (OE14) | `Resources/ServerInfo/_OE14/Guidebook_PT-BR/GameSystems/` | `Resources/Prototypes/_OE14/Guidebook/PT-BR/game_systems.yml` |
| Portuguese (SS14 upstream) | `Resources/ServerInfo/_OE14/Guidebook_PT-BR/` | `Resources/Prototypes/_OE14/Guidebook/PT-BR/*.yml` |

### Current Guidebook Pages (OE14 Game Systems)

| Entry ID (EN) | Entry ID (PT-BR) | Name (EN) | Name (PT-BR) |
|---|---|---|---|
| OE14GameSystems | OE14_PT_GameSystems | Game Systems | Sistemas do Jogo |
| OE14Magic | OE14_PT_Magic | Magic | Magia |
| OE14Crafting | OE14_PT_Crafting | Crafting | Crafting |
| OE14Economy | OE14_PT_Economy | Economy | Economia |
| OE14Combat | OE14_PT_Combat | Combat | Combate |
| OE14CharacterStats | OE14_PT_CharacterStats | Character Stats | Atributos |
| OE14Skill | OE14_PT_Skill | Skills | Habilidades |
| OE14Trading | OE14_PT_Trading | Trading | Comércio |
| OE14StatusEffects | OE14_PT_StatusEffects | Status Effects | Efeitos de Status |

### Guidebook Icons
Every guide should include at least one `<GuideEntityEmbed>` to illustrate content for players. Use `<Box>` with multiple embeds for groups of related items. Always verify the entity exists in `Resources/Prototypes/` before referencing.

---

## Known Errors & Fixes

### 1. Guidebook Parser Error
**Cause**: XML content file uses unsupported syntax.
**Bad**: backticks, `**bold**`, `{code}`, `<?xml?>` header, `<section>`, `<CDATA>`
**Fix**: Use only `<Document>`, `#` headings, `[bold]`, `[color=]`. See Alchemy.xml as reference:
`Resources/ServerInfo/_OE14/Guidebook_EN/JobsTabs/AlchemistTabs/Alchemy.xml`

### 2. Entry name shows as key (e.g. `guide-entry-oe14-magic`)
**Cause**: The `name:` field in the guideEntry prototype was set to a localization key that doesn't exist in any `.ftl` file.
**Fix**: Use plain text directly: `name: Magic` — no FTL key needed for OE14 entries.

### 3. Port 1212 already in use on server start
**Cause**: A previous Content.Server process is still running.
**Fix**: `killall Content.Server` or `lsof -ti:1212 | xargs kill -9`

### 4. XML entity escaping errors (`<` `>` in text)
**Cause**: Raw angle brackets inside text nodes.
**Fix**: Use `&lt;` and `&gt;`, or simply avoid angle brackets in guidebook text.

### 5. Entry not visible in guidebook tree
**Cause**: Entry prototype exists but is not listed as a `children:` item of any visible parent.
**Fix**: Add the entry ID to the `children:` list of its parent in `entry.yml`.

### 6. Character stat bonuses migration not applied
**Cause**: Migration .cs file existed but missing .Designer.cs file (EF Core requires both).
**Fix**: Generate Designer from ModelSnapshot template and modify class/attribute names to match migration.
**Status**: FIXED - migrations now apply correctly on fresh database.

### 7. Stat modifiers causing overflow when spending points
**Cause**: Server validation only checked base stat, not effective (base + modifier).
**Example**: INT 8 + Metamagic +2 = 10 effective, but server allowed spending because base (8) < max (10).
**Fix**: Include modifier in validation: `if ((base + modifier) >= MaxStatValue) return;`
**Status**: FIXED - commit ab1876fe

### 8. Character Window (C menu) not opening after UI refactor
**Cause**: XAML had duplicate BoxContainer elements with identical names (StrBarSlot, VitBarSlot, DexBarSlot, IntBarSlot). The invisible placeholder definitions overwrote references to the actual bar slots in the XAML name registry.
**Fix**: Remove all duplicate element definitions. Keep only the real ones inside the stat rows.
**Status**: FIXED - commit e51b6f09

### 9. Guidebook entries invisible when language is pt-BR
**Cause**: `OE14_PT` had `openEdgeAllowed: false`. The `GuidebookWindow.xaml.cs` filters entries at line 215: `if (!entry.OpenEdgeAllowed) continue;` — this filters out ALL entries with false, including parent nodes. Also `OE14GameSystems` was only a child of `OE14_EN` (filtered by `locFilter: "en-US"`).
**Fix**: Change `openEdgeAllowed: false` → `openEdgeAllowed: true` on ALL PT-BR entries. Create separate PT-BR game system entries (`OE14_PT_GameSystems`) pointing to PT-BR XML files. Add `OE14_PT_GameSystems` as child of `OE14_PT`.
**Status**: FIXED

### 10. Guidebook hallucinations — items that don't exist in code
**Cause**: Guidebook referenced "Memory Crystals" as skill point consumables, but no entity prototype exists. Only `OE14BloodEssence` exists (vampire-only). Also mentioned "Speed Boots" artifact which doesn't exist.
**Fix**: Remove all references to non-existent items. Update skill guide to accurately describe how skill points work (Memory: not obtainable from items; Blood: vampire-only via Blood Essence).
**Status**: FIXED

### 11. Mixed language in FTL files
**Cause**: Some PT-BR FTL files contained English words/phrases mixed with Portuguese text (e.g., "Você reaches out to pet", "brew não identificado", "potatoes").
**Fix**: Translate all remaining English fragments. Add missing keys from EN to PT-BR (e.g., ~268 Human and Tiefling name keys).
**Status**: FIXED — 100% of FTL files translated

---

## Key Systems (OE14-specific)

All custom systems live under `Content.Shared/_OE14/` and `Content.Server/_OE14/`.

| System | File prefix | What it does |
|---|---|---|
| Spell Storage | OE14SpellStorage | Adds spells to action bar when item equipped |
| Mana Cost | OE14ActionManaCost | Validates and deducts mana on spell cast |
| Workbench | OE14Workbench | Recipe-based crafting with DoAfter delay |
| Cooking | OE14Cooking | Temperature-based item transformation |
| Modular Craft | OE14ModularCraft | Assembly crafting with stat modifiers |
| Currency | OE14Currency | Physical coin entities with value |
| Salary | OE14Salary | Auto-pays coins to job holders over time |
| Trading Platform | OE14TradingPlatform | Buy/sell with dynamic pricing |
| Farming | OE14Farming | Plant growth with energy and harvest |
| Skills | OE14Skill | XP-based skill progression per combat type |
| Status Effects | StatusEffect | Burning, Frozen, Poisoned, Bleeding |
| Vampire | OE14Vampire | Vampire role mechanics |
| Demiplane | OE14Demiplane | Dimensional travel system |
| Character Stats | OE14CharacterStats | Strength/Vitality/Dexterity/Intelligence with point spending |

---

## Prototype Locations

| What | Where |
|---|---|
| OE14 entity prototypes | `Resources/Prototypes/_OE14/` |
| OE14 maps | `Resources/Maps/_OE14/` |
| Guidebook entries (our) | `Resources/Prototypes/Guidebook/oe14_systems.yml` |
| Guidebook tree root (OE14) | `Resources/Prototypes/_OE14/Guidebook/entry.yml` |
| Guidebook XML pages | `Resources/ServerInfo/Guidebook/OE14_*.xml` |
| OE14 EN guidebook pages | `Resources/ServerInfo/_OE14/Guidebook_EN/` |
| OE14 PT-BR guidebook pages | `Resources/ServerInfo/_OE14/Guidebook_PT-BR/GameSystems/` |
| Localization (EN) | `Resources/Locale/en-US/_OE14/` |
| Localization (PT-BR) | `Resources/Locale/pt-BR/_OE14/` |

---

## Working Reference Files

When unsure about a format, read these files as ground truth:

| Need | Reference file |
|---|---|
| Guidebook XML format | `Resources/ServerInfo/_OE14/Guidebook_EN/JobsTabs/AlchemistTabs/Alchemy.xml` |
| guideEntry YAML format | `Resources/Prototypes/_OE14/Guidebook/Eng/*.yml` |
| Entity prototype format | `Resources/Prototypes/_OE14/Entities/Objects/` |
| Map format | `Resources/Maps/_OE14/dev_map.yml` |

---

## Session History Summary

| Date | What was done |
|---|---|
| 2026-03-22 | Project setup, README, initial docs, first commit, pushed to GitHub |
| 2026-03-22 | Created OE14_Magic, OE14_Crafting, OE14_Economy, OE14_Combat guidebook pages (wrong format) |
| 2026-03-23 | Debug: fixed port 1212 conflict, XML entity escaping, build validation |
| 2026-03-23 | Discovered correct guidebook format by reading Alchemy.xml |
| 2026-03-23 | Rewrote all 4 guidebook pages in correct `<Document>` format |
| 2026-03-23 | Added guideEntry YAML prototypes and registered under OE14_EN tree |
| 2026-03-23 | Fixed parser errors (backticks, code blocks, markdown syntax) |
| 2026-03-23 | Fixed entry names (FTL key → plain text) |
| 2026-03-23 | All 4 pages confirmed working in-game |
| 2026-03-23 | Expanded OE14_GettingStarted.xml; added OE14_CharacterStats.xml (5th guidebook page) |
| 2026-03-23 | Added OE14CharacterStats to Silva and Carcat species prototypes |
| 2026-03-23 | Created stat point spending system (network event + server validation + UI buttons) |
| 2026-03-23 | Server now applies stats to MobThresholds/Stamina/MagicEnergy dynamically |
| 2026-03-23 | **Session 5 (continuation)**: Completed stat bonuses system, documentation updates |
| 2026-03-23 | - Fixed character stat bonuses database migration (added missing .Designer.cs) |
| 2026-03-23 | - Fixed stat modifier overflow bug: validates effective value (base + modifiers) |
| 2026-03-23 | - Prevents overflow: INT 8 + Metamagic +2 = 10 (maxed, cannot spend) |
| 2026-03-23 | - Overflow protection: modifiers exceeding cap grant AvailablePoints compensation |
| 2026-03-23 | - Updated OE14_CharacterStats.xml guidebook with Stat Modifiers section |
| 2026-03-23 | - Updated .ai/session-log.md, .ai/errors.md, .ai/decisions.md with Session 5 notes |
| 2026-03-23 | - Commits: pending (documentation + final fixes) |
| 2026-03-23 | **Session 6 (continuation 2)**: Fixed remaining stat bugs, began UI refactor |
| 2026-03-23 | - Fixed: Creation points being lost (3 given, 2 spent, 1 unused = lost) |
| 2026-03-23 | - Fixed: Creation bonuses not applying as modifiers (preventing overflow detection) |
| 2026-03-23 | - Fixed: Elf innate skills (T1/T2) not applying INT modifiers at spawn |
| 2026-03-23 | - Elf now correctly spawns with INT 10 (8 base + 1 T1 + 1 T2) |
| 2026-03-23 | - **WIP**: Character Window refactor with 3-column layout + ModifierSource tracking |
| 2026-03-23 | - Created ModifierSource enum (Spell, Spend, Item, Buff) for tracking mod origins |
| 2026-03-23 | - Changed component: `int modifier` → `Dictionary<ModifierSource, int>` |
| 2026-03-23 | - UI format: `INT: 8 → 10 (+2 Spell)` shows base, current, and source |
| 2026-03-23 | - Commits: ca5d7dce, 551e1fb1, bf6e015e, 09752bed |
| 2026-03-24 | **Session 10 (continuation 5)**: Fixed Character Window (C menu) not opening |
| 2026-03-24 | - Debugged: XAML had duplicate BoxContainer elements with same names |
| 2026-03-24 | - Root cause: Invisible placeholder definitions overwrote real bar slots in name registry |
| 2026-03-24 | - Removed 9 lines of duplicate definitions from CharacterWindow.xaml |
| 2026-03-24 | - ✅ Character Window now opens correctly with C key, bars and modifiers display properly |
| 2026-03-24 | - Commit: e51b6f09 — "Fix: Remove duplicate BarSlot definitions in CharacterWindow.xaml" |
| 2026-04-02 | **Session: OpenCode Setup** |
| 2026-04-02 | - Configured OpenCode with background-process plugin |
| 2026-04-02 | - Set up PortWarp for external server connections (CGNAT workaround) |
| 2026-04-02 | - Created global commands: /build, /server, /kill-server, /review, etc. |
| 2026-04-02 | - Created skills: start-server, file-search, process-manager |
| 2026-04-02 | - Successfully connected external player via PortWarp tunnel |
| 2026-04-02 | - Fixed Cooking Recipes guidebook parser error (invalid entity IDs) |
