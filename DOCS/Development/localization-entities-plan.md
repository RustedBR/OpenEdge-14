# Plano de Localização de Entidades OE14

## Contexto

O sistema SS14 resolve nomes de entidades procurandouma chave FTL `ent-{EntityID}` (ex: `ent-OE14BloodEssence`). Se não encontrar, usa o `name:` do YAML como fallback.

A vantagem: **não precisa modificar os YAMLs**. Basta criar arquivos FTL e o sistema automaticamente traduz.

---

## Status Atual

### ✅ Concluído
- [x] Script Python para gerar stubs FTL: `scripts/generate_oe14_entity_ftl.py`
- [x] Gerados **540 stubs** de entidades em 6 arquivos por categoria:
  - `weapons.ftl` - 22 entidades
  - `tools.ftl` - 10 entidades
  - `structures.ftl` - 10 entidades
  - `mobs.ftl` - 6 entidades
  - `materials.ftl` - 43 entidades
  - `misc.ftl` - 449 entidades (maior parte, inclui alimentos, poções, decorações, etc.)

### ⚠️ Limitações do Script
O script original não capturou todas as ~1883 entidades porque muitos YAMLs têm tags customizadas (`!type:HTNCompoundTask`, `!type:Container`, etc.) que o PyYAML não consegue parsear sem o schema do SS14. 

A solução é modificar os protótipos YAML para usar `localizationId:` ou rodar a extração com o parser correto do RobustToolbox.

---

## Stub FTL Gerado (Exemplo)

```ftl
# Resources/Locale/pt-BR/_OE14/entities/weapons.ftl

ent-OE14WeaponSwordIron = iron sword
    .desc = A finely crafted sword favored by imperial officers...

ent-OE14WeaponDaggerIron = iron dagger
    .desc = A standard dagger made of iron.
```

**Próximo passo**: Traduzir os valores de inglês para português.

---

## Fases de Tradução

### Fase 1: Armas (weapons.ftl) - 22 entidades
Prioridade: 🔴 Alta — itens visíveis no jogo com frequência

| ID | Nome EN | Nome PT-BR |
|----|---------|------------|
| OE14WeaponSwordIron | iron sword | espada de ferro |
| OE14WeaponDaggerIron | iron dagger | adaga de ferro |
| OE14WeaponDaggerHatchet | hatchet | machadinha |
| OE14WeaponDaggerSickle | sickle | foice |
| OE14WeaponWarAxeIron | iron war axe | machado de guerra de ferro |
| OE14WeaponWarHammerIron | iron war hammer | martelo de guerra de ferro |
| OE14WeaponTwoHandedSwordIron | iron two-handed sword | espada de duas mãos de ferro |
| OE14WeaponRapierIron | rapier | florete |
| OE14WeaponSpearIron | iron spear | lança de ferro |
| OE14WeaponSkimitarIron | iron skimitar | scimitar de ferro |
| OE14WeaponShovelIron | iron shovel | pá de ferro |

### Fase 2: Ferramentas (tools.ftl) - 10 entidades
Prioridade: 🔴 Alta

| ID | Nome EN | Nome PT-BR |
|----|---------|------------|
| OE14BaseWrench | wrench | chave inglês |
| ... |

### Fase 3: Estruturas (structures.ftl) - 10 entidades
Prioridade: 🟡 Média

### Fase 4: Mobs (mobs.ftl) - 6 entidades
Prioridade: 🟡 Média

### Fase 5: Materiais (materials.ftl) - 43 entidades
Prioridade: 🟡 Média
Inclui: sangue, essências, cristais, minérios

### Fase 6: Misc (misc.ftl) - 449 entidades
Prioridade: 🟢 Baixa
Inclui: alimentos, poções, móveis, decorações, etc.

---

## Regras de Tradução

1. **Nomes próprios de espécies** → manter (Tiefling, Silva, Carcat, Goblin)
2. **Termos técnicos universais** → manter (mana, stamina)
3. **Nomes compostos** → traduzir partes (ex: "black hunter" → "caçador negro")
4. **Itens com apóstrofo** → adaptar (ex: "alchemist's cloak" → "capa de alquimista")
5. **Nomes de jobs** → já traduzidos, pular
6. **Latin termos de magia** → manter (Terra, Ignis, Aqua, etc.)

---

## Como Executar

1. **Traduzir stub existente**: Editar `Resources/Locale/pt-BR/_OE14/entities/*.ftl`
2. **Testar**: Rodar cliente em pt-BR, verificar nomes no inventário/examine
3. **Validar**: en-US ainda mostra nomes em inglês (fallback do YAML funciona)

---

## Arquivos Gerados

```
Resources/Locale/pt-BR/_OE14/entities/
├── weapons.ftl     # 22 stubs (traduzir)
├── tools.ftl       # 10 stubs (traduzir)
├── structures.ftl # 10 stubs (traduzir)
├── mobs.ftl        # 6 stubs (traduzir)
├── materials.ftl   # 43 stubs (traduzir)
└── misc.ftl        # 449 stubs (traduzir)
```

---

## Script de Geração

Localizado em: `scripts/generate_oe14_entity_ftl.py`

Para rodar novamente:
```bash
python3 scripts/generate_oe14_entity_ftl.py
```

---

## Referências

- Sistema de localização SS14: `RobustToolbox/Robust.Shared/Localization/LocalizationManager.Entity.cs`
- Padrão de chave: `ent-{EntityID}`
- Fallback: se não encontrar FTL, usa `name:` do YAML

---

## Histórico de Mudanças

| Data | Mudança |
|------|---------|
| 2026-04-03 | Script criado, stubs gerados |
| 2026-04-03 | UI strings traduzidas (vote, execution, antag, traits) |
