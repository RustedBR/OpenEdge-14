# Project Structure

This document describes the OpenEdge 14 project structure for developers.

## Directory Layout

```
OpenEdge-14/
├── CLAUDE.md                    ← AI context (read first!)
├── .ai/                        ← AI working files
│   ├── errors.md              ← Known errors and fixes
│   ├── session-log.md         ← Session history
│   ├── decisions.md           ← Architecture decisions
│   └── dev-preferences.md     ← Developer preferences
├── Content.Server/             ← Server-side C# code
├── Content.Client/            ← Client-side C# code
├── Content.Shared/             ← Shared C# (both sides)
│   └── _OE14/                 ← OpenEdge custom systems
├── Resources/
│   ├── Prototypes/
│   │   ├── _OE14/             ← OE14 entity prototypes
│   │   └── Guidebook/         ← Guidebook YAML entries
│   ├── ServerInfo/Guidebook/  ← Guidebook XML pages
│   ├── Maps/_OE14/            ← Game maps
│   └── Locale/                ← Localization files
├── DOCS/
│   ├── Development/           ← Dev guides
│   └── Gameplay/              ← Gameplay guides
├── RobustToolbox/             ← SS14 engine (do not modify unless necessary)
└── runserver.sh / runclient.sh ← Dev launchers
```

## Key Systems Location

All OE14-specific systems are in `Content.Shared/_OE14/` and `Content.Server/_OE14/`:

| System | Location |
|--------|----------|
| Spell Storage | Content.Shared/_OE14/Spell/ |
| Mana System | Content.Shared/_OE14/Mana/ |
| Cooking | Content.Server/_OE14/Cooking/ |
| Crafting | Content.Server/_OE14/Workbench/ |
| Modular Craft | Content.Server/_OE14/ModularCraft/ |
| Trading | Content.Server/_OE14/Trading/ |
| Skills | Content.Server/_OE14/Skill/ |
| Status Effects | Content.Shared/_OE14/StatusEffects/ |
| Character Stats | Content.Shared/_OE14/CharacterStats/ |

## Using OpenCode

See CLAUDE.md or in-game Guidebook "OpenCode Guide" for details.

Quick start:
```bash
opencode
```

Useful commands:
- `/build` - Build project
- `/server` - Start server
- `/review <file>` - Review code
- "find files containing X" - Search