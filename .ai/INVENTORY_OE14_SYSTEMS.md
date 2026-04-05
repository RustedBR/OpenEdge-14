# 📊 Inventário de Sistemas OpenEdge 14 (Parte 3A)

**Data**: 2026-03-22
**Status**: Mapeamento completo de arquivos _OE14
**Propósito**: Classificar sistemas por categoria para geração de documentação (Parte 3B)

---

## 1. SISTEMAS DE MAGIA (Magic System)

### Spells / Ações Mágicas
```
Resources/Prototypes/_OE14/Entities/Actions/Spells/
├── base.yml (configuração base de spells)
├── Athletic/ (magia de movimento/força)
│   ├── dash.yml
│   ├── keen_eye.yml
│   ├── kick.yml
│   ├── second_wind.yml
│   └── sprint.yml
├── Dimension/ (magia de transporte/dimensão)
│   ├── portal_to_city.yml
│   ├── shadow_grab.yml
│   └── shadow_step.yml
├── Nature/ (magia da natureza)
├── Evocation/ (magia ofensiva)
└── Abjuration/ (magia defensiva)
```

### Reagentes e Alquimia
```
Resources/Prototypes/_OE14/Reagents/
├── magic_essence.yml
└── Target Effects/
    ├── damage_effects.yml
    ├── negative_effects.yml
    └── side_effects.yml
```

**Subcategoria**: Alquimia
- `Chemistry/mixing_types.yml` - tipos de reações químicas
- `Chemistry/metabolizer_types.yml` - metabolismo mágico

---

## 2. SISTEMAS DE CRAFTING (Crafting & Production)

### Crafting Recipes
```
Resources/Prototypes/_OE14/Recipes/
├── Alchemy/
├── Smithing/
├── Cooking/
├── Carpentry/
└── Tailoring/
```

### Lojas e Vendedores
```
Resources/Prototypes/_OE14/Vendors/
├── Merchants/
├── Tavern/
└── Blacksmith/
```

**Status**: Estrutura base configurada, detalhes em arquivos específicos

---

## 3. SISTEMA DE CORPOS E RAÇAS (Body & Species)

### Raças Humanoides
```
Resources/Prototypes/_OE14/Body/Prototypes/
├── human.yml (humano base)
├── elf.yml (elfo - agilidade)
├── dwarf.yml (anão - força)
├── tiefling.yml (tiefling - magia)
├── carcat.yml (felino - reflexos)
├── silva.yml (criatura da natureza)
├── skeleton.yml (esqueleto - não-morto)
└── zombie.yml (zumbi - não-morto)
```

### Órgãos Corporais
```
Resources/Prototypes/_OE14/Body/Organs/
├── dwarf.yml (órgãos anão)
└── silva.yml (órgãos natura)
```

### Partes do Corpo
```
Resources/Prototypes/_OE14/Body/Parts/
├── human.yml
├── skeleton.yml
└── zombie.yml
```

---

## 4. SISTEMA DE DANO E SAÚDE (Damage & Health)

### Configuração de Dano
```
Resources/Prototypes/_OE14/Damage/
├── types.yml (tipos de dano: cortante, contundente, mágico)
├── groups.yml (agrupamento de danos)
├── modifier_sets.yml (modificadores de dano)
├── containers.yml (contenção de dano)
└── examine_messages.yml (mensagens ao examinar ferimentos)
```

**Tipos de Dano**: Cortante, Contundente, Queimadura, Frio, Elétrico, Tóxico, Radiação, Mágico

---

## 5. SISTEMA DE JOBS (Trabalhos & Papéis)

### Jobs Disponíveis
```
Resources/Prototypes/_OE14/Jobs/
├── Guard.yml (guarda - combate)
├── Innkeeper.yml (hospedeiro - social)
├── Merchant.yml (mercador - comércio)
├── Alchemist.yml (alquimista - magia)
├── Adventurer.yml (aventureiro - exploração)
└── Apprentice.yml (aprendiz - qualquer coisa)
```

**Status**: Cada job tem spawn, equipamento e autoridades específicas

---

## 6. SISTEMA DE MAPAS (World & Maps)

### Mapas Principais
```
Resources/Maps/_OE14/
├── comoss.yml (2.8MB - mapa principal, ilha grande)
├── comoss_d.yml (dados derivados)
├── venicialis.yml (1.0MB - mapa secundário, água)
├── venicialis_d.yml (dados derivados)
├── dev_map.yml (77KB - mapa de desenvolvimento, testes)
├── nautilus_ship.yml (125KB - navio navegável)
├── island.yml (39KB - ilha pequena)
└── template.yml (9.9KB - template para criar mapas)
```

**Características**:
- Comoss: Castelos, vilas, florestas, minas
- Venicialis: Cidades aquáticas, pontes, canais
- Dev Map: Pequeno, para testes rápidos

---

## 7. SISTEMA DE ENTIDADES (Entities & Objects)

### Ações e Habilidades
```
Resources/Prototypes/_OE14/Entities/Actions/
├── ghost.yml (habilidades espectrais)
├── nightVision.yml (visão noturna)
└── Spells/ (ver seção Magia)
```

### NPCs e Criaturas
```
Resources/Prototypes/_OE14/Entities/Mobs/
├── Animals/
│   ├── boar.yml (javali)
│   ├── pig.yml (porco)
│   ├── rabbit.yml (coelho)
│   └── deer.yml (cervo)
├── Humanoid/
│   ├── guard.yml (guarda NPC)
│   ├── merchant.yml (mercador NPC)
│   └── beggar.yml (mendigo)
└── Fantastical/
    ├── ghost.yml (fantasma)
    └── golem.yml (golem)
```

### Itens e Objetos
```
Resources/Prototypes/_OE14/Entities/Objects/
├── Weapons/
│   ├── sword.yml
│   ├── bow.yml
│   ├── mace.yml
│   └── staff.yml
├── Armor/
│   ├── leather_armor.yml
│   ├── plate_armor.yml
│   └── robe.yml
├── Tools/
│   ├── pickaxe.yml
│   ├── axe.yml
│   └── hoe.yml
└── Consumables/
    ├── bread.yml
    ├── potion.yml
    └── scroll.yml
```

---

## 8. SISTEMA DE INTERFACE (UI & Display)

### Alertas e Status
```
Resources/Prototypes/_OE14/Alerts/
├── alerts.yml (avisos gerais)
└── status_effect.yml (efeitos de status)
```

### Datasets (Dados)
```
Resources/Prototypes/_OE14/Datasets/
├── Names/
│   └── species-names.yml (nomes por raça)
└── tips.yml (dicas do lobby)
```

### Configuração de Chat
```
Resources/Prototypes/_OE14/Accent/
└── word_replacements.yml (substituições de sotaque/fala)
```

---

## 9. SISTEMA DE AUDIO (Música & Sons)

```
Resources/Prototypes/_OE14/
├── audio_loops.yml (sons em loop - ambiente, combate)
└── audio_music.yml (música de lobby e mapas)
```

---

## 10. SISTEMAS DIVERSOS

### Decorações
```
Resources/Prototypes/_OE14/Decal/
├── bricktile_stone.yml (tijolos coloridos)
├── bricktile_stone_Grayscale.yml (versão cinza)
├── wood_trim_thin_Grayscale.yml (madeira)
├── nature.yml (natureza - flores, grama)
└── crayon.yml (desenhos)
```

### Factores de IA
```
Resources/Prototypes/_OE14/ai_factions.yml (facções para NPCs)
```

### Gravidade e Física
```
Resources/Prototypes/_OE14/Gravity/ (não configurado ainda)
```

### Device Linking (Automação)
```
Resources/Prototypes/_OE14/DeviceLinking/
├── source_ports.yml
└── sink_ports.yml
```

### Severidade de Contrabando
```
Resources/Prototypes/_OE14/contraband_severities.yml
```

### Preenchimentos de Mobília
```
Resources/Prototypes/_OE14/Catalog/Fills/
├── closets.yml (armários com itens)
├── crates.yml (caixas com itens)
└── dresser.yml (cômoda com roupas)
```

---

## 📊 Resumo Estatístico

| Categoria | Arquivos | Status |
|-----------|----------|--------|
| Magia | 15+ | ✅ Completo |
| Crafting | 8+ | 🟡 Parcial |
| Corpos/Raças | 17 | ✅ Completo |
| Dano/Saúde | 5 | ✅ Completo |
| Jobs | 6 | ✅ Completo |
| Mapas | 8 | ✅ Completo |
| Entidades | 40+ | ✅ Completo |
| Interface | 6 | ✅ Completo |
| Audio | 2 | ✅ Completo |
| Diversos | 12+ | ✅ Completo |

**Total Estimado**: 100+ arquivos YAML configurados

---

## 🔄 Próximo Passo (Parte 3B)

Usar este inventário para gerar XMLs de Guidebook com:
1. **Magic Guide** - detalhes de cada escola de magia
2. **Crafting Guide** - receitas e producción
3. **Species Guide** - características de cada raça
4. **Items Guide** - armas, armaduras, ferramentas
5. **Maps Guide** - descrição de cada mundo
6. **Jobs Guide** - salários e responsabilidades

---

**Gerado por**: Claude Haiku 4.5
**Repositório**: https://github.com/RustedBR/OpenEdge-14
