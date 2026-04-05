# Architecture Decisions — OpenEdge 14

Decisions made during development that affect how we approach future work.
Last updated: 2026-04-02 (OpenCode + Guidebook PT-BR)

---

## [2026-04-02] Commits devem ser em Português

**Decisão**: Todas as mensagens de commit devem ser escritas em português brasileiro.

**Como aplicar**:
- Usarverbos no infinitivo: "Adicionar", "Corrigir", "Atualizar"
- Manterconciso (1-2 linhas)
- Exemplo: "Corrige erros de sintaxe em OE14DemiplaneSystem"

---

## [2026-04-02] Guia PT-BR e EN-US devem estar sincronizados

**Decisão**: Qualquer mudança no guidebook EN deve ser replicada para PT-BR, e vice-versa.

**Como aplicar**:
- Antes de fazer commit, verificar se ambas as pastas têm as mesmas páginas
- Se adicionar uma nova página em Guidebook_EN/, criar também em Guidebook_PT-BR/
- Se modificar conteúdo em um idioma, replicar as mudanças no outro
- Manter IDs iguais em ambos os idiomas (exceto prefixo: OE14_EN_* vs OE14_PT_*)

**Estrutura**:
- `Guidebook_EN/` → arquivos XML em inglês
- `Guidebook_PT-BR/` → arquivos XML em português
- Ambos devem ter exatamente as mesmas páginas

---

## [2026-04-02] Localização PT-BR e EN-US devem estar sincronizados

**Decisão**: Qualquer mudança em localization EN deve ser replicada para PT-BR, e vice-versa.

**Como aplicar**:
- Antes de fazer commit, verificar se ambas as pastas de locale têm os mesmos arquivos
- Se adicionar uma nova key em en-US/_OE14/, criar também em pt-BR/_OE14/
- Se modificar conteúdo em um idioma, replicar as mudanças no outro
- Manter FTL keys idênticas (somente traduzir valores após o =)

**Estrutura**:
- `Resources/Locale/en-US/_OE14/` → localization em inglês
- `Resources/Locale/pt-BR/_OE14/` → localização em português
- Ambos devem ter exatamente os mesmos arquivos

---

## [2026-03-23] Never document features without reading the code first

**Decision**: All guidebook and documentation content must be verified by reading the actual C# or YAML source.

**Reason**: Sessions 1-2 produced guidebook pages with hallucinated features (summoning spells, spell books, permanent learning) that don't exist in code. The only valid source of truth is the codebase itself.

**How to apply**:
1. Use `grep -r "FeatureName" Content.*` to confirm the feature exists
2. Read the component file (data definition) and system file (logic)
3. Only document what the code actually does
4. Cite file paths in documentation
5. Rule of thumb: "If I can't grep for it, it doesn't exist."

---

## [2026-03-23] Spell categories verified in codebase

**Decision**: The following spell categories exist and are safe to reference in documentation.

**Verified path**: `Resources/Prototypes/_OE14/Entities/Actions/Spells/`

**Categories**: Athletic, Dimension, Earth, Electric, Fire, Life, Light, Lurker, Meta, Misc, Skeleton, Slime, Vampire, Water

**Verified staffs**:
- OE14MagicHealingStaff (3 spells)
- OE14MagicFireStaff (3 spells)
- OE14MagicShadowStaff (2 spells)
- OE14MagicGuardElectroStaff (3 spells)
- OE14MagicDruidStaff (1 spell)

---

## [2026-03-23] Systems NOT yet verified (do not document without reading first)

- ✅ OE14ActionManaCost (verified via OE14_Magic.xml)
- ✅ OE14Cooking (verified Session 13 — temperature system 400K-500K)
- ✅ OE14TradingPlatform (verified Session 13 — dynamic pricing)
- ✅ OE14Skill (verified Session 13 — trees, XP, free skills)
- ✅ StatusEffect (verified Session 13 — Magic Vision, Mana Armor)

---

## [2026-03-23] ECS Pattern in use (RobustToolbox)

**Decision**: All custom systems follow the RobustToolbox ECS pattern.

**How to apply**:
- Component = `[RegisterComponent]` class with `[DataField]` fields — stores data only
- System = `EntitySystem` class with `SubscribeLocalEvent<>()` calls — contains all logic
- Shared code = Base classes inherited by both server and client
- Server-only: `if (_net.IsClient) return;` pattern
- Network sync: `[DataField]` fields are synced automatically by RobustToolbox

**Reference implementations** (verified, read in Session 3):
- `OE14WallmountComponent` + `OE14WallmountSystem`
- `OE14DoorInteractionPopupComponent` + `OE14DoorInteractionPopupSystem`
- `OE14SpellStorageSystem`

---

## [2026-03-23] Guidebook pages written in English

**Decision**: All new guidebook XML pages (OE14_*.xml) are written in English.

**Reason**: The original OE14 content (Ss14-Medieval) has both EN and RU versions. Our additions start with EN only. RU translations can be added later if needed.

---

## [2026-03-23] guideEntry prototypes stored in Prototypes/Guidebook/ (not _OE14/)

**Decision**: Our custom guideEntry YAML file is `Resources/Prototypes/Guidebook/oe14_systems.yml`, not inside `_OE14/`.

**Reason**: The `_OE14/Guidebook/` folder contains entries that were inherited from upstream. Keeping our additions in the base `Guidebook/` folder with a clear `oe14_` prefix makes them easier to find and avoids merge conflicts with upstream.

---

## [2026-03-23] .ai/ folder as AI working directory

**Decision**: All AI session notes, error logs, decisions, and temporary working files go in `.ai/` at the project root.

**Reason**: Keeps the project root clean. Claude can always find its own context by reading `.ai/` first. Not committed to the main branch if not needed, but checked in for persistence.

---

## [2026-03-23] CLAUDE.md as primary context file

**Decision**: `CLAUDE.md` at the project root is the first file Claude reads at the start of every session.

**Reason**: Provides instant project context, file layout, known errors, and working references without requiring a full codebase scan. Modeled on the Claude Code convention.

---

## [2026-03-23] Guidebook content: gameplay-focused, not code-focused

**Decision**: Guidebook pages explain how systems work for players, not how the C# implementation works.

**Reason**: Early attempts at technical documentation (code blocks, system internals) caused parser errors and were unreadable in-game. The Alchemy.xml reference page is the correct model: clear prose, gameplay tips, item embeds.

---

## [2026-03-23] Stats system architecture: Shared calculates, Server applies

**Decision**: `OE14SharedCharacterStatsSystem` (Shared) calculates bonus values. `OE14CharacterStatsSystem` (Server) applies them to actual game systems (MobThresholds, Stamina, MagicEnergy).

**Reason**: Writing to MobThresholds/Stamina/MagicEnergy must happen server-side to avoid prediction conflicts. The client shows the state via networked component fields.

**How to apply**:
1. Add stat values to `OE14CharacterStatsComponent` YAML (race prototype)
2. Pre-calculate the health/stamina/mana values and put them in YAML too (for first spawn)
3. Server system `OnServerStartup` calls `ApplyStatsToGameSystems` — keeps everything in sync if YAML is updated
4. When points are spent, `ApplyStatsToGameSystems` is called again to update in real time

---

## [2026-03-24] Stat scale: 1–10, neutral = 5, guaranteed minimums

**Decision**: All character stats use a 1–10 integer scale. Stat 5 is neutral (no bonus, no penalty). Stat calculations guarantee HP ≥ 50 and Stamina ≥ 25.

**Formulas** (from `OE14SharedCharacterStatsSystem`):
- Strength: `DamageMultiplier = 1.0 + (Str - 5) * 0.1`
- Vitality: `HealthBonus = (Vit - 5) * 12.5` → crit threshold = 100 + HealthBonus (at Vit=1: 50 HP minimum)
- Dexterity: `StaminaMultiplier = 1.0 + (Dex - 5) * 0.1875` → stamina crit = 100 * StaminaMultiplier (at Dex=1: 25 stamina minimum)
- Intelligence: `ManaBonus = (Int - 5) * 25` → max mana = 100 + ManaBonus

**Race values** (verified 2026-03-23):
| Race | STR | VIT | DEX | INT |
|---|---|---|---|---|
| Human (base) | 5 | 5 | 5 | 5 |
| Dwarf | 7 | 8 | 3 | 3 |
| Elf | 3 | 3 | 5 | 8 |
| Tiefling | 5 | 5 | 4 | 6 |
| Silva | 2 | 5 | 6 | 7 |
| Carcat | 6 | 5 | 8 | 2 |

---

## [2026-03-23] Stat point spending: server-validated via network event

**Decision**: Clients request stat point spending via `OE14SpendStatPointMessage`. The server validates (has points? stat below max?) and applies.

**How to apply**:
- Client: inject `OE14ClientCharacterStatsSystem` and call `RequestSpendPoint(entity, statName)`
- Server: `SubscribeNetworkEvent<OE14SpendStatPointMessage>` → validate → `UpdateStatsCalculations` → `ApplyStatsToGameSystems`
- UI: buttons shown only when `AvailablePoints > 0` and stat `< MaxStatValue (9)`

**Granting points**: Set `OE14CharacterStatsComponent.AvailablePoints` on the server (admin command or progression system).

---

## [2026-03-23] Guidebook is the primary human-facing documentation

**Decision**: Guidebook pages (in-game) are more important than `.ai/` files. `.ai/` is for AI context only. Guidebook is what players read.

**How to apply**: When documenting a system, write the guidebook page first. Update `.ai/` notes only as a secondary step.

---

## [2026-03-23] Stat point spending validates effective value (base + modifiers)

**Decision**: Stat point spending is only allowed if the effective stat (base + all temporary modifiers) is below `MaxStatValue` (10).

**Reason**:
- Temporary modifiers come from skills (Metamagic +2 INT), equipment, or spells
- A player should not be able to bypass the max cap by spending points while modifiers are active
- The system prevents: INT 8 (base) + 2 (modifier) = 10 (effective), then spend point to go to 9+2=11

**How to apply**:
1. `OnSpendStatPoint()` gets both base value and modifier from the switch statement
2. Calculate: `effectiveValue = statValue + statModifier`
3. Check: `if (effectiveValue >= MaxStatValue) return;` (deny spending)
4. Allow spending only if `effectiveValue < MaxStatValue`

**Related overflow protection**:
- `ApplyStatModifier()` also detects when a modifier would overflow the max
- If overflow detected, grant `AvailablePoints` equal to overflow amount
- Example: INT 8 + modifier +3 would be 11 → cap at 10 + grant 1 available point

**Where it's enforced**: `Content.Server/_OE14/CharacterStats/OE14CharacterStatsSystem.cs` — `OnSpendStatPoint()` method, lines 125-147.

---

## [2026-04-01] Modular craft: damage types defined in slot prototypes

**Decision**: Damage types for modular crafting are defined in slot prototypes, not per-equipment.

**Reason**: Previously, each equipment prototype defined its own `damageType` field. This caused duplication across 25+ files. Moving the damage type to the slot definition (MainHand, OffHand, Head, Body) simplifies the architecture.

**How to apply**:
1. In `Resources/Prototypes/_OE14/Modular/slots.yml`, each slot has a `damageType` field
2. Equipment prototypes no longer need `damageType` field
3. Damage parts reference the slot's damageType when building the modular item

**Reference**: `Resources/Prototypes/_OE14/Modular/slots.yml` — defines Blunt, Slash, Pierce, etc. per slot type.

---

## [2026-04-03] Idioma padrão do servidor é pt-BR

**Decisão**: O idioma padrão do servidor foi mudado de `en-US` para `pt-BR`.

**Arquivo**: `Content.Shared/CCVar/CCVars.Localization.cs`
```csharp
CVarDef.Create("loc.server_language", "pt-BR", CVar.SERVER | CVar.REPLICATED);
```

---

## [2026-04-03] Guidebook: openEdgeAllowed deve ser true para entradas visíveis

**Decisão**: Todas as entradas do guidebook que devem aparecer na árvore precisam ter `openEdgeAllowed: true`. O valor `false` filtra a entrada completamente, independente de `locFilter`.

**Como o filtro funciona** (`GuidebookWindow.xaml.cs`):
1. Linha 215: `if (!entry.OpenEdgeAllowed) continue;` — filtra entradas com false
2. Linha 216: `if (entry.LocFilter is not null && entry.LocFilter != _cfg.GetCVar(CCVars.Language)) continue;` — filtra por idioma
3. Se `LocFilter` é `null`, a entrada aparece em qualquer idioma

**Padrão para entradas PT-BR**:
```yaml
- type: guideEntry
  openEdgeAllowed: true    # ← DEVE ser true para aparecer
  locFilter: "pt-BR"           # ← filtra por idioma
  id: OE14_PT_Something
  name: Nome em Português
  text: "/ServerInfo/_OE14/Guidebook_PT-BR/Path.xml"
```

**Erro comum**: Definir `openEdgeAllowed: false` achando que é para filtrar por idioma. Use `locFilter` para idioma, `openEdgeAllowed` para visibilidade geral.

---

## [2026-04-03] Estrutura de localização do guidebook PT-BR

**Decisão**: XMLs de guidebook em português ficam em `Resources/ServerInfo/_OE14/Guidebook_PT-BR/GameSystems/` e as entradas YAML em `Resources/Prototypes/_OE14/Guidebook/PT-BR/`.

**Árvore de entradas PT-BR**:
```
OE14_PT (entry.yml)
  ├── OE14_PT_Species
  ├── OE14_PT_Jobs
  └── OE14_PT_GameSystems (game_systems.yml)
        ├── OE14_PT_Magic
        ├── OE14_PT_Crafting
        ├── OE14_PT_Economy
        ├── OE14_PT_Combat
        ├── OE14_PT_CharacterStats
        ├── OE14_PT_Skill
        ├── OE14_PT_Trading
        └── OE14_PT_StatusEffects
OE14_PT_DEVELOPMENT (entry.yml)
  ├── OE14_PT_Welcome
  ├── OE14_PT_Systems
  └── OE14_PT_Other
```

---

## [2026-04-03] Todos os guias devem ter ícones ilustrativos

**Decisão**: Todo guia do guidebook deve ter pelo menos um `<GuideEntityEmbed>` para ilustrar o conteúdo para jogadores humanos.

**Como aplicar**:
- Usar `<Box>` com múltiplos `<GuideEntityEmbed Entity="ID" Caption="Nome"/>` para grupos de itens relacionados
- Incluir ícones de: armas, equipamentos, plataformas, moedas, pergaminhos, poções, cristais
- Sempre verificar que a entidade existe em `Resources/Prototypes/` antes de referenciar

**Entidades mais úteis para embeddar**:
- Moedas: `OE14CopperCoin1`, `OE14SilverCoin1`, `OE14GoldCoin1`, `OE14PlatinumCoin1`
- Plataformas: `OE14TradingPlatform`, `OE14TradingSellingPlatform`, `OE14SalaryPlatform`
- Bancadas: `OE14Workbench`, `OE14WorkbenchAnvil`, `OE14WorkbenchSewing`, `OE14WorkbenchCooking`
- Armas: `OE14WeaponSwordIron`, `OE14WeaponWarAxeIron`, `OE14WeaponSpearIron`, `OE14BowCombat`
- Cajados: `OE14MagicFireStaff`, `OE14MagicHealingStaff`, `OE14ManaOperationGlove`
- Pergaminhos: `OE14SpellScrollFireball`, `OE14SpellScrollCureWounds`, `OE14SpellScrollFreeze`
- Cristais: `OE14ModularCrystalStrength`, `OE14ModularCrystalVitality`, `OE14ModularCrystalDexterity`, `OE14ModularCrystalIntelligence`
- Bandagens: `OE14HerbalBandage1`, `OE14Gauze1`
- Poções: `OE14VialSmallHealingPoison`, `OE14VialSmallHealingMana`

---

## [2026-04-03] Localização FTL: 100% traduzido

**Decisão**: Todos os arquivos FTL em `Resources/Locale/pt-BR/_OE14/` devem existir e estar traduzidos. Nenhum arquivo pode ser idêntico ao correspondente em `en-US`.

**Como verificar**:
```bash
for f in $(find Resources/Locale/en-US/_OE14 -name "*.ftl" -type f | sed 's|Resources/Locale/en-US/_OE14/||'); do
  diff -q "Resources/Locale/en-US/_OE14/$f" "Resources/Locale/pt-BR/_OE14/$f"
done
```
Se não houver output, todos os arquivos foram traduzidos.

**Arquivos de marcações** (markings/) são nomes de estilos visuais na criação de personagem — devem ser traduzidos para português mesmo sendo nomes estilísticos, pois aparecem na UI de customização.
