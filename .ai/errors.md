# Known Errors & Fixes — OpenEdge 14

This file is maintained by Claude (Cowork + Claude Code sessions).
Add every non-obvious error here with its fix.
Last updated: 2026-04-01 (Session 13)

---

## [2026-03-23] Sandbox Violation: StringBuilder.AppendLine with Interpolation

**Symptom**: `TypeCheckFailedException: Assembly Content.Client failed type checks` during client startup.

**Root cause**: Using `StringBuilder.AppendLine($"interpolated string")` violates RobustToolbox's sandbox. The compiler generates an `AppendInterpolatedStringHandler` type that the sandbox forbids.

**Fix**: Use string concatenation without `StringBuilder`:
```csharp
// ❌ BAD
var sb = new StringBuilder();
sb.AppendLine($"Text: {value}");

// ✅ GOOD
var text = "Text: " + value.ToString() + "\n";
```

**Affected file**: `Content.Client/_OE14/UserInterface/Systems/CharacterStats/OE14CharacterStatsUIController.cs`

---

## [2026-03-23] Guidebook Parser Error

**Symptom**: In-game guidebook shows red "Parser Error" banner when clicking an entry.

**Root cause**: The XML content file uses syntax that the SS14 guidebook parser does not support.

**Confirmed bad patterns**:
- Backtick code blocks: ` ```csharp ... ``` `
- Markdown bold: `**text**`
- Angle brackets raw in text: `<`, `>`
- `<?xml version="1.0"?>` header at top of file
- Custom tags: `<section>`, `<header>`, `<NavSection>`, `<SubSection>`, `<Text>`, `<![CDATA[...]]>`

**Fix**: Rewrite file using only the supported subset:
```
<Document>
  # Heading
  ## Subheading
  [bold]text[/bold]
  [color=#hex]text[/color]
  <Box><GuideEntityEmbed Entity="EntityID"/></Box>
</Document>
```

**Reference for correct format**: `Resources/ServerInfo/_OE14/Guidebook_EN/JobsTabs/AlchemistTabs/Alchemy.xml`

---

## [2026-03-23] Guidebook Entry Shows Localization Key Instead of Name

**Symptom**: Entry appears in tree as `guide-entry-oe14-magic` instead of `Magic`.

**Root cause**: The `name:` field in the guideEntry YAML prototype was set to a FTL localization key that has no corresponding string defined in any `.ftl` file. SS14 falls back to showing the raw key.

**Fix**: Use plain text directly in the `name:` field:
```yaml
- type: guideEntry
  id: OE14Magic
  name: Magic        # ← plain text, NOT a localization key
  text: "/ServerInfo/Guidebook/OE14_Magic.xml"
```

Only use FTL keys if you also add the corresponding string to `Resources/Locale/en-US/_OE14/guidebook/guides.ftl`.

---

## [2026-03-23] Port 1212 Already in Use on Server Start

**Symptom**: `System.Net.Sockets.SocketException (98): Address already in use` when running `runserver.sh`.

**Root cause**: A previous Content.Server process is still running from a prior session or failed shutdown.

**Fix**:
```bash
killall Content.Server
# or more targeted:
lsof -ti:1212 | xargs kill -9
```

---

## [2026-03-23] XML Entity Escaping in Guidebook Text

**Symptom**: `xmllint` validation fails with "StartTag: invalid element name".

**Root cause**: Raw `<` and `>` characters used inside text nodes are interpreted as XML tags.

**Fix**: Escape them as `&lt;` and `&gt;`, or simply avoid using angle brackets in guidebook prose.

---

## [2026-03-23] Guidebook Entry Not Visible in Tree

**Symptom**: guideEntry prototype exists, XML file is valid, but the entry never shows in the guidebook.

**Root cause**: The entry is not listed as a `children:` item of any visible parent entry.

**Fix**: Add the entry ID to the `children:` list of its parent in the appropriate entry YAML file. For OE14 entries, the parent chain is:
```
OE14_EN (entry.yml)
  └── OE14GameSystems (entry.yml)
        └── OE14Magic / OE14Crafting / OE14Economy / OE14Combat / OE14CharacterStats (oe14_systems.yml)
```

---

## [2026-03-23] RobustToolbox Sandbox: Direct write to component fields from external system

**Symptom**: `error RA0002: Tried to perform a 'Write' other-type access to member 'X' in type 'YComponent'`

**Root cause**: RobustToolbox enforces access control via `[Access]` attributes on component fields. Writing a field from a system that doesn't own that component type is forbidden.

**Affected case**: Writing `OE14MagicEnergyContainerComponent.MaxEnergy` from `OE14CharacterStatsSystem`.

**Fix**: Use the owning system's public API instead of writing directly:
```csharp
// ❌ BAD — direct field write from external system
mana.MaxEnergy = newMax;

// ✅ GOOD — use ChangeMaximumEnergy delta method
var delta = desiredMax - mana.MaxEnergy;
if (delta != 0)
    _magicEnergy.ChangeMaximumEnergy((uid, mana), delta);
```

---

## [2026-03-23] `sealed` class cannot be used as base for server/client-side system

**Symptom**: `error CS0509: 'ServerSystem': cannot derive from sealed type 'SharedSystem'`

**Root cause**: The shared system was declared `sealed partial class`, preventing server/client subclasses.

**Fix**: Remove `sealed` from the shared system:
```csharp
// ❌ sealed partial class OE14SharedCharacterStatsSystem : EntitySystem
// ✅
public partial class OE14SharedCharacterStatsSystem : EntitySystem
```

Pattern: Shared = `partial class`, Server = `sealed partial class : SharedClass`, Client = `sealed partial class : SharedClass`.

---

## [2026-03-23] `BaseButton.OnPressed` cannot be assigned with `=`

**Symptom**: `error CS0079: The event 'BaseButton.OnPressed' may only appear on the left hand side of += or -=`

**Root cause**: `OnPressed` is a C# event, not a property. It cannot be assigned with `=`.

**Fix**: Wire buttons once in window creation using `+=`. Store the entity/context in a field or capture from `_player.LocalEntity` at call time:
```csharp
// ❌ _window.StrengthSpendButton.OnPressed = _ => DoThing();
// ✅ (in OnStateEntered, once)
_window.StrengthSpendButton.OnPressed += _ => OnSpendButtonPressed("strength");
```

---

## [2026-03-23] Character creation stat bonuses not persisting from database

**Symptom**: Player spawns with default race stats; character creation bonuses (OE14StatBonus* fields) not applied to entity.

**Root cause**: Entity Framework Core migration `20260323000000_OE14StatBonuses` was added to the .cs file but never executed because the migration system is architecture-sensitive. The migration requires both `.cs` and `.Designer.cs` files. Without the Designer file, EF Core silently skips the migration.

**Fix**:
1. Generate `.Designer.cs` file by copying existing template (e.g., `20250314222717_ConstructionFavorites.Designer.cs`)
2. Update class name and snapshot references to match the migration name
3. Delete the database file and rebuild: `rm Content.Server.Database/Migrations/*.db && dotnet build`
4. Verify migration applied: `sqlite3 server_db.db "SELECT MigrationId FROM __EFMigrationsHistory WHERE MigrationId LIKE '%OE14%'"`

**Files involved**:
- `Content.Server.Database/Migrations/Sqlite/20260323000000_OE14StatBonuses.cs`
- `Content.Server.Database/Migrations/Sqlite/20260323000000_OE14StatBonuses.Designer.cs` (must exist)
- `Content.Server.Database/Model.cs` (defines Profile.OE14StatBonus* properties)
- `Content.Server/Database/ServerDbBase.cs` (ConvertProfiles reads them)

---

## [2026-03-23] Stat point spending allowed when effective stat at max

**Symptom**: Character with INT 8 + INT modifier +2 (effective 10) can still spend an available point on INT, pushing it to 11 and breaking the max cap.

**Root cause**: `OE14CharacterStatsSystem.OnSpendStatPoint()` validation checked only the base stat value against `MaxStatValue`, without considering temporary modifiers from skills or equipment.

**Code that was wrong**:
```csharp
if (statValue >= stats.MaxStatValue)  // ← only checks base, ignores modifiers
{
    Logger.Warning($"... but it's already at max");
    return;
}
```

**Fix**: Calculate effective value and check that:
```csharp
var effectiveValue = statValue + statModifier;  // ← base + temporary modifiers
if (effectiveValue >= stats.MaxStatValue)
{
    Logger.Warning($"... but it's already at {effectiveValue}/{stats.MaxStatValue} (base: {statValue}, modifier: {statModifier})");
    return;
}
```

**Where modifiers come from**:
- Skills with stat bonuses (e.g., Metamagic: +2 INT)
- Equipment temporary effects
- Spell effects
- Tracked in `OE14CharacterStatsComponent.StrengthModifier`, `VitalityModifier`, `DexterityModifier`, `IntelligenceModifier`

**How modifiers are applied**: `OE14CharacterStatsSystem.ApplyStatModifier()` — called by skill/spell systems when modifier should be applied or removed.

---

## [2026-03-23] Stat modifier overflow causes wasted bonus

**Symptom**: When a modifier is applied that would push effective stat above 10, the excess bonus is discarded. Players lose the benefit of the modifier.

**Example**: INT 8 + INT modifier +3 = 11, but caps at 10. That +1 overflow is wasted.

**Root cause**: Modifiers are applied directly without checking if they overflow the max cap.

**Fix**: In `ApplyStatModifier()`, detect overflow and grant overflow amount as `AvailablePoints` compensation:
```csharp
if (intDelta > 0 && stats.Intelligence + stats.IntelligenceModifier + intDelta > stats.MaxStatValue)
{
    var overflow = (stats.Intelligence + stats.IntelligenceModifier + intDelta) - stats.MaxStatValue;
    stats.AvailablePoints += overflow;  // ← compensation
    Logger.Debug($"INT modifier overflow: +{overflow} point(s) granted as available");
}
```

This ensures the modifier effect is never wasted—if capping prevents the full bonus, the player gets spendable points instead.

**When to use**: Skills, spells, or equipment that grant stat bonuses should call `ApplyStatModifier()` to automatically handle overflow.

---

## [2026-03-23] Character creation points being lost

**Symptom**: Player receives 3 creation points, spends 2 on INT, leaves 1 unused. At spawn, only 0 points available (before overflow/fallback).

**Root cause**: System counted which bonuses were spent but didn't track leftover points. The 1 point that wasn't allocated was discarded.

**Fix**: In `OnPlayerSpawnComplete`, calculate remaining points:
```csharp
int totalSpent = profile.OE14StatBonusStrength + profile.OE14StatBonusVitality +
                 profile.OE14StatBonusDexterity + profile.OE14StatBonusIntelligence;
int pointsLeft = 3 - totalSpent;
if (pointsLeft > 0)
    stats.AvailablePoints += pointsLeft;
```

**Commit**: ca5d7dce — "Fix: Contar pontos não gastos"

---

## [2026-03-23] Character creation bonuses not triggering overflow on skill bonuses

**Symptom**: Player customizes INT +2 in creation (INT 8 + 2 = 10), then learns Metamagic (+1 INT). No overflow detected, no refund point granted.

**Root cause**: Creation bonuses were being applied as **base stat changes** instead of **modifiers**. When Metamagic added +1:
- INT base: 8 (unchanged)
- Modifiers: +1 (Metamagic only, creation bonus wasn't tracked as modifier)
- Check: 8 + 1 = 9 < 10 → no overflow

The +2 bonus was "baked into" the base stat (changed 8→10), so it was invisible to the overflow protection system.

**Fix**: In `OnPlayerSpawnComplete()`, apply creation bonuses as **modifiers** instead of base stat changes:
```csharp
// ❌ BEFORE (wrong)
stats.Intelligence = Math.Clamp(stats.Intelligence + profile.OE14StatBonusIntelligence, 1, 10);

// ✅ AFTER (correct)
stats.IntelligenceModifier += profile.OE14StatBonusIntelligence;
```

Now when Metamagic adds +1:
- INT base: 8
- Modifiers: +2 (creation) + 1 (Metamagic) = +3
- Effective: 8 + 3 = 11 > 10 → **overflow detected!** → grant 1 point ✅

**Commit**: 551e1fb1 — "Fix: Aplicar bonuses de criação como modifiers"

---

## [2026-03-24] Innate skills not applying modifiers at spawn

**Symptom**: Elf character starts with INT 8, but Metamagic T1 (+1 INT) and T2 (+1 INT) don't apply. Expected INT 10, got INT 8.

**Root cause**: Both `MapInitEvent` and `ComponentInit` were firing and re-applying skills. `MapInitEvent` fired AFTER `ComponentInit`, clearing and re-applying all skills, but the override check was in the wrong handler.

**Context**:
- Free learned skills (like Elf's Metamagic T1/T2) must apply their effects when the entity is created
- MapInitEvent fires when entity is placed on map (late)
- ComponentInit fires when component is added (earlier)
- If both apply the same skill, duplicates occur or second overwrites first

**Fix**: In `OE14SharedSkillSystem`:
```csharp
// ❌ DISABLED MapInitEvent handler (was clearing and re-applying)
private void OnMapInit(Entity<OE14SkillStorageComponent> ent, ref MapInitEvent args)
{
    // DISABLED: Caused duplicate skill application
}

// ✅ KEPT ComponentInit with proper contains-check
private void OnComponentInit(Entity<OE14SkillStorageComponent> ent, ref ComponentInit args)
{
    var free = ent.Comp.FreeLearnedSkills.ToList();
    foreach (var skill in free)
    {
        if (!ent.Comp.LearnedSkills.Contains(skill))  // ← don't re-apply
        {
            // Apply effect...
            ent.Comp.LearnedSkills.Add(skill);
        }
    }
}
```

**Why this works**: ComponentInit always fires before the entity is visible in the world. It applies free skills once with proper duplicate prevention. MapInitEvent is unnecessary and was causing problems.

**Verification**: Elf now spawns with INT 10 (8 base + 1 T1 + 1 T2) ✓

**Commit**: d769642d — "Fix: Disable duplicate OnMapInit skill application"

---

## [2026-03-24] Character Window (C menu) not opening after UI refactor

**Symptom**: Pressing C key does not open the Character Window. No error messages in logs.

**Root cause**: XAML had **duplicate element definitions with identical names**:
- `<BoxContainer Name="StrBarSlot" ... />` at line 61 (real, inside stat row)
- `<BoxContainer Name="StrBarSlot" ... />` at line 113 (duplicate, marked `Visible="False"`)
- Same for VitBarSlot, DexBarSlot, IntBarSlot

In RobustToolbox XAML, when you have two elements with the same `Name`, the **second definition overwrites the first** in the name registry. The C# code accessing `_window.StrBarSlot` got a reference to the invisible, empty container instead of the real bar.

The `BuildStatBar(_window.StrBarSlot, ...)` call would succeed (null-check passes) but the bar would be a different element, causing layout issues or silent failures.

**How it was created**: When reverting the UI layout changes, the old "placeholder" definitions weren't removed, only hidden with `Visible="False"`. They lingered as orphaned duplicates.

**Fix**: Remove all duplicate definitions:
```xaml
<!-- REMOVED these lines -->
<!-- <BoxContainer Name="StrBarSlot" ... Visible="False"/> -->
<!-- <BoxContainer Name="VitBarSlot" ... Visible="False"/> -->
<!-- <BoxContainer Name="DexBarSlot" ... Visible="False"/> -->
<!-- <BoxContainer Name="IntBarSlot" ... Visible="False"/> -->

<!-- Keep only the REAL ones in the stat rows -->
<BoxContainer Name="StrBarSlot" HorizontalExpand="True" MinHeight="12" ... />
```

**What changed**:
- File: `Content.Client/UserInterface/Systems/Character/Windows/CharacterWindow.xaml`
- Removed: 9 lines (comment + 4 duplicate containers)
- Impact: Window now initializes correctly, all bars render, UI fully functional

**Lesson**: XAML name collisions are **silent failures**. Always check for duplicate element names when reverting complex layouts.

**Commit**: `e51b6f09` — "Fix: Remove duplicate BarSlot definitions in CharacterWindow.xaml"

**After fix**: ✅ Window opens, bars render, modifiers display correctly, all stats calculate properly

---

## [2026-04-02] OpenCode - Background Process Tools

**Context**: Using OpenCode with opencode-background-process plugin.

**Tools available**:
- `background_process_launch` - Start server/commands in background
- `background_process_list` - List running processes
- `background_process_read` - Read output
- `background_process_kill` - Stop process

**Usage in OpenCode**:
```
"start server" → Uses background_process_launch
"show server logs" → Uses background_process_read
"stop server" → Uses background_process_kill
```

**Status**: ✅ Working - Tested with PortWarp for external connections.

---

## [2026-04-03] Guidebook entries invisible when language is pt-BR

**Symptom**: When server language is set to pt-BR, the guidebook tree shows no OE14 game system entries. Only species and jobs appear (or nothing at all).

**Root cause 1**: `OE14_PT` entry in `entry.yml` had `openEdgeAllowed: false`. The `GuidebookWindow.xaml.cs` filters entries at line 215: `if (!entry.OpenEdgeAllowed) continue;` — this filters out ALL entries with false, including parent nodes.

**Root cause 2**: `OE14GameSystems` was only listed as a child of `OE14_EN` (which has `locFilter: "en-US"`). When language is pt-BR, `OE14_EN` is filtered out, so `OE14GameSystems` never appears.

**Root cause 3**: All PT-BR guidebook YAML files in `Resources/Prototypes/_OE14/Guidebook/PT-BR/` had `openEdgeAllowed: false` — same issue as root cause 1.

**Fix**:
1. Change `openEdgeAllowed: false` → `openEdgeAllowed: true` on ALL PT-BR entries
2. Create separate PT-BR game system entries (`OE14_PT_GameSystems`) pointing to PT-BR XML files
3. Add `OE14_PT_GameSystems` as child of `OE14_PT`

**Files affected**:
- `Resources/Prototypes/_OE14/Guidebook/entry.yml` (OE14_PT, OE14_PT_DEVELOPMENT)
- `Resources/Prototypes/_OE14/Guidebook/PT-BR/development.yml` (26 entries)
- `Resources/Prototypes/_OE14/Guidebook/PT-BR/alchemy.yml` (9 entries)
- `Resources/Prototypes/_OE14/Guidebook/PT-BR/inkeeper.yml` (2 entries)
- `Resources/Prototypes/_OE14/Guidebook/PT-BR/mechanics.yml` (1 entry)
- `Resources/Prototypes/_OE14/Guidebook/PT-BR/misc.yml` (4 entries)
- `Resources/Prototypes/_OE14/Guidebook/PT-BR/species.yml` (7 entries)
- `Resources/Prototypes/_OE14/Guidebook/PT-BR/jobs.yml` (11 entries)
- `Resources/Prototypes/_OE14/Guidebook/PT-BR/game_systems.yml` (NEW — 9 entries)

**Lesson**: `openEdgeAllowed: false` means the entry is NEVER visible in the guidebook. Use `locFilter` for language filtering, not `openEdgeAllowed`.

---

## [2026-04-03] FTL file unterminated string literal

**Symptom**: Server logs show `[ERRO] loc: Unterminated string literal` for a pt-BR FTL file.

**Root cause**: Missing closing quote in a multiline FTL string value. Example: `{"[italic]Exemplo[/italic]: text.}` should be `{"[italic]Exemplo[/italic]: text."}`

**Fix**: Add the missing closing quote. Also ensure file ends with a trailing newline.

**File**: `Resources/Locale/pt-BR/_OE14/books/pantheon_gods_sileita_imitators.ftl`

---

## [2026-04-03] Guidebook hallucinations — items that don't exist in code

**Symptom**: Guidebook references items, systems, or features that don't exist as entity prototypes or systems in the codebase.

**Confirmed hallucinations found**:
- **Memory Crystal**: Referenced in skill guide as a consumable that grants skill points. No entity prototype exists. Only `OE14BloodEssence` exists as a skill point consumable (vampire-only, Blood type).
- **Speed Boots artifact**: Referenced as granting Magical Acceleration spell. No "Speed Boots" entity exists. The spell `OE14ActionSpellMagicalAcceleration` is granted by unidentified equipment.
- **Dimension skill tree**: The entire Dimension tree is commented out in `trees.yml`. Shadow Grab/Shadow Step spells exist as entities and scrolls but cannot be learned via skill tree.
- **Resurrection in Healing T3**: The Resurrection skill node is commented out in the Healing tree. The spell and scroll exist but it's scroll-only.
- **OE14SpellAddMemoryPoint**: C# class exists but no YAML prototype references it. Grants 0 points in current game.

**How skill points actually work**:
- **Memory points**: Used by all non-vampire skill trees. Currently NOT obtainable from items — only through special events or progression.
- **Blood points**: Used by Vampire tree only. Gained by: (1) vampire antag selection (2 points), (2) consuming Blood Essence items, (3) gathering essence from victims.

**Fix**: Remove all references to non-existent items from guidebook XMLs. Update skill guide to accurately describe how skill points work.

**Lesson**: Always verify entity prototypes exist before referencing them in guidebook text. Search for `EntityID` in `Resources/Prototypes/` to confirm.

---

## [2026-04-04] GuideEntityEmbed parser error para entidades com componentes server-only

**Symptom**: Guidebook mostra caixa vermelha "Erro de Análise / Tag: GuideEntityEmbed Arguments: Entity = 'X'" ao tentar exibir uma entidade inline.

**Root cause**: O renderer client-side do guidebook tenta inicializar a entidade para gerar o ícone. Se a entidade possui componentes que só existem no servidor (server-only systems), a inicialização falha e gera o erro.

**Entidades confirmadas com o problema**:
- `OE14Mortar` — tem `type: OE14Mortar`, `type: OE14SolutionTemperature`, `type: Injector`
- `OE14Pestle` — tem `type: OE14Pestle`
- `OE14ClothingEyesAlchemyGlasses` — tem `type: SolutionScanner`

**O que funciona**: Entidades simples sem componentes server-only (plantas, essências, itens básicos com apenas Sprite + Item + Clothing).

**Fix temporário**: Remover o `<GuideEntityEmbed>` para essas entidades e descrever em texto:
```xml
<!-- ❌ falha -->
<Box>
  <GuideEntityEmbed Entity="OE14Mortar"/>
  <GuideEntityEmbed Entity="OE14Pestle"/>
</Box>

<!-- ✅ funciona -->
A maior parte dos ingredientes pode ser triturada com um [bold]pilão e almofariz[/bold].
```

**Fix permanente**: Adicionar counterparts client-side para os componentes problemáticos (requer C#). Não implementado.

**Arquivo afetado**: `Resources/ServerInfo/_OE14/Guidebook_PT-BR/JobsTabs/AlchemistTabs/Alchemy.xml`

---

## [2026-04-03] Mixed language in FTL files

**Symptom**: Some PT-BR FTL files contain English words/phrases mixed with Portuguese text.

**Examples found**:
- `interaction/interaction-popup-component.ftl`: "Você reaches out to pet" → should be "Você estende a mão para acariciar"
- `cooking/soups.ftl`: "brew não identificado" → "caldo não identificado"
- `cooking/soups.ftl`: "Delicious." → "Delicioso."
- `cooking/soups.ftl`: "Basically" → "basicamente"
- `cooking/meals.ftl`: "potatoes" (3x) → "batatas"
- `species/species-names.ftl`: Missing ~268 keys (all Human and Tiefling names)

**Fix**: Translate all remaining English fragments. Add missing keys from EN to PT-BR (proper nouns don't need translation, just need to exist as keys).

**How to check**: Run `diff` between EN and PT-BR FTL files, or search for English words in PT-BR files.
