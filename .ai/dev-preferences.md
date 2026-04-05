# Dev Preferences — RustedBR

How this project's owner wants AI-assisted work to be done.
OpenCode, Claude Code and Cowork: read this file at session start.

---

## Language

- All **code, YAML prototypes, and inline comments** → English
- **Commit messages** → Portuguese is fine
- **Conversation** → Portuguese is fine
- **Documentation files** (DOCS/) → English

---

## OpenCode Specific

When using OpenCode:
- Use **background processes** for long-running tasks (server, builds)
- Say "start server" to run server in background without blocking
- Say "show server logs" to check output
- Use `/build` or `/doctor` for diagnostics

---

## Verification

- Do NOT report something as "working" unless it has been tested in the actual game client
- XML valid + build passing ≠ guidebook page working in-game
- The only valid test for guidebook is: open page in client → no parser error → content renders

---

## Before writing any new file

Always read a working reference file of the same type first:

| File type | Read this first |
|---|---|
| Guidebook XML | `Resources/ServerInfo/_OE14/Guidebook_EN/JobsTabs/AlchemistTabs/Alchemy.xml` |
| guideEntry YAML | `Resources/Prototypes/_OE14/Guidebook/Eng/*.yml` |
| Entity prototype | `Resources/Prototypes/_OE14/Entities/Objects/` |

---

## Before documenting any system

1. Confirm it exists: `grep -r "SystemName" Content.* --include="*.cs" -l`
2. Read the component file (data definition)
3. Read the system file (logic)
4. Read the prototype YAML (configuration)
5. Only then write documentation

Never invent mechanics. If you can't grep for it, it doesn't exist.

---

## Before build/edit sessions

```bash
killall Content.Server
pkill -f Content.Client
```
Running processes can lock files and cause cached builds to not reflect changes.

---

## AI working files

All session notes, error logs, and decisions go in `.ai/` at project root. Not scattered anywhere else.

---

## Commits

- Always run `dotnet build` before committing — 0 errors required
- Co-author every commit: `Co-Authored-By: Claude Haiku 4.5 <noreply@anthropic.com>`
- Do NOT push AI-generated code to upstream Ss14-Medieval

---

## AI Policy (this repo)

This is a personal fork — AI-assisted code is allowed.
- Documentation: use OpenCode/Claude output as draft
- Code: review/rewrite before committing
- Upstream Ss14-Medieval: NO AI code, ever

---

## OpenCode Commands & Plugins

### Installed Plugins
- `opencode-background-process` - Run server and long tasks in background
- `opencode-morph-fast-apply` - Faster code editing

### Global Commands
Located in: `~/.config/opencode/commands/`
- `/plan`, `/doctor`, `/build`, `/server`, `/kill-server`, `/review`, `/status`, `/diff`, etc.

### Skills
Located in: `~/.claude/skills/`
- `start-server`, `file-search`, `process-manager`
