# Space Station 14 — Referência Completa da Wiki

## Sumário

1. [Visão Geral do Projeto](#visão-geral-do-projeto)
2. [História e Origem](#história-e-origem)
3. [Arquitetura Técnica](#arquitetura-técnica)
4. [Gameplay e Mecânicas](#gameplay-e-mecânicas)
5. [Departamentos da Estação](#departamentos-da-estação)
6. [Estrutura do Código e ECS](#estrutura-do-código-e-ecs)
7. [Motor Robust Toolbox](#motor-robust-toolbox)
8. [Como Contribuir](#como-contribuir)
9. [Forks e Variantes Notáveis](#forks-e-variantes-notáveis)
10. [Jogos Similares e Inspirações](#jogos-similares-e-inspirações)
11. [Links e Recursos](#links-e-recursos)

---

## Visão Geral do Projeto

**Space Station 14 (SS14)** é um jogo multiplayer open-source de RPG baseado em turnos/rodadas, inspirado diretamente no clássico cult **Space Station 13**. O jogo se passa em uma estação espacial operada pela megacorporação fictícia **Nanotrasen**, onde cada rodada simula um turno de trabalho que invariavelmente sai do controle.

- **Gênero:** RPG multiplayer, simulação, sandbox, ação cooperativa
- **Engine:** Robust Toolbox (motor próprio em C#)
- **Licença:** Código sob MIT; assets majoritariamente sob CC-BY-SA 3.0
- **Plataformas:** Windows, Linux, macOS (via launcher standalone ou Steam)
- **Preço:** Gratuito (free-to-play), inclusive no Early Access
- **Repositório principal:** [github.com/space-wizards/space-station-14](https://github.com/space-wizards/space-station-14)

Os jogadores escolhem (ou são designados para) diferentes funções a bordo da estação — de engenheiro a capitão, ou até um traidor — cada uma com equipamentos e responsabilidades únicas. Mecânicas complexas de inventário, reator nuclear, atmosfera, química e medicina interagem entre si, criando situações emergentes de caos, sabotagem e sobrevivência.

---

## História e Origem

O SS14 nasceu do desejo da comunidade de recriar o Space Station 13 em uma engine moderna, abandonando as limitações do BYOND (motor original do SS13, de 2003).

- **2003:** Space Station 13 original lançado por "exadv1" como simulação atmosférica.
- **2006:** Código-fonte do SS13 vaza e a comunidade assume o desenvolvimento.
- **~2014–2015:** Primeiras tentativas de remake do SS13 começam e fracassam, tornando "SS13 remakes" um meme interno da comunidade.
- **2017 (Maio):** Novos desenvolvedores se juntam ao SS14, criando o Discord e acelerando o desenvolvimento.
- **2021 (final):** SS14 atinge player count consistente 24/7 nos servidores oficiais, com grande adoção na comunidade russa.
- **2022 (Novembro):** O número de jogadores do SS14 no Steam ultrapassa o do SS13 no BYOND.
- **Atual:** O jogo está em Early Access no Steam, com desenvolvimento ativo e contínuo.

O projeto é mantido pela **Space Wizards Federation**, um grupo de desenvolvedores voluntários.

---

## Arquitetura Técnica

### Visão Geral

O SS14 utiliza uma arquitetura **cliente-servidor** com o motor **Robust Toolbox**:

- **Linguagem:** C# (.NET)
- **Build:** `dotnet build`
- **Estrutura:** O repositório principal (space-station-14) é o "content pack" — contém todo o conteúdo do jogo. O Robust Toolbox é a engine base carregada como submódulo.
- **UI:** Sistema de interface baseado em XAML customizado com controles definidos em `.xaml` + `.xaml.cs` (code-behind com `RobustXamlLoader`)
- **Física:** Baseada no Box2D (port Farseer adaptado), com solver paralelo de constraints, broadphase customizado, e suporte a fixtures e joints
- **Rede/Networking:** Sincronização client-server com dirty-checking de componentes
- **Documentação técnica:** Escrita em Markdown usando mdbook, open-source no GitHub

### Preprocessor Defines Importantes

| Define         | Descrição                                              |
|----------------|--------------------------------------------------------|
| `DEBUG`        | Habilita checks de debug como `DebugTools.Assert()`   |
| `TOOLS`        | Habilita ferramentas de desenvolvimento               |
| `RELEASE`      | Build na configuração Release                         |
| `FULL_RELEASE` | Build final para distribuição a jogadores             |

---

## Gameplay e Mecânicas

### Estrutura de Rodada

Cada partida é uma **rodada** representando um turno de trabalho na estação. Os jogadores:

1. Customizam seu personagem (aparência, preferências de cargo)
2. Recebem um cargo baseado em suas preferências
3. Executam tarefas do cargo enquanto lidam com desastres, antagonistas e caos
4. A rodada termina quando condições específicas são atingidas

### Sistemas Simulados

O jogo simula de forma completa:

- **Energia elétrica** (geração, distribuição, manutenção)
- **Atmosfera** (pressão, composição gasosa, temperatura)
- **Biologia** (saúde, doenças, cirurgias)
- **Química** (misturas, reações, medicamentos, explosivos)
- **Inventário e interação com objetos** detalhados

### Cargos Recomendados para Iniciantes

- **Passageiro:** Sem responsabilidades, ideal para aprender controles e explorar
- **Cargos de Serviço:** Bartender, Chef, Botânico, Zelador — bons para roleplay social
- **Assistentes técnicos:** Estagiário Médico, Assistente de Pesquisa, Assistente Técnico — para aprender sistemas mais complexos com apoio de veteranos

### Antagonistas

Alguns jogadores são designados como **antagonistas** a cada rodada (ex: Traidor, Operativos Nucleares). Eles possuem objetivos secretos como matar alvos específicos ou roubar itens valiosos. Jogadores que não foram designados como antagonistas não devem praticar violência sem razão ("self-antag" e "RDM" são infrações que levam a ban).

---

## Departamentos da Estação

A estação é dividida em departamentos, cada um com mecânicas, fantasias de gameplay e pilares de design próprios:

### Comando
Liderança da estação. O Capitão dirige os chefes de departamento, que coordenam suas equipes.

### Segurança
Manutenção da ordem, resposta a emergências e ameaças. Foco em patrulha, detenção e investigação.

### Engenharia
Geração de energia, manutenção de infraestrutura (portas, atmosfera interna), reparo de danos. A "fantasia" é resolver problemas práticos do grid elétrico e estrutural.

### Ciência
Pesquisa de tecnologias, xenoarqueologia (artefatos alienígenas) e pesquisa anômala. Dividida em subcategorias:
- **Xenoarqueologia:** Análise de artefatos alienígenas para gerar pontos de pesquisa
- **Pesquisa Anômala:** Estudo de anomalias geradas ou espontâneas
- **Robótica:** Construção de drones e equipamentos

### Médico
Saúde da tripulação, cirurgias, pesquisa de doenças, clonagem. Foco em manter todos vivos durante o caos.

### Serviço / Civil
Bar, cozinha, botânica, limpeza. Foco em interação social e suporte à estação.

### Suprimentos / Carga
Logística, entrega de correio, mineração de asteroides. Interface com todos os outros departamentos.

---

## Estrutura do Código e ECS

O SS14 utiliza uma arquitetura **ECS (Entity-Component-System)** rigorosa, diferente do modelo E/C (Entity/Component) popularizado pelo Unity.

### Princípios Fundamentais

- **Entidades:** Apenas bundles de componentes, sem lógica própria
- **Componentes:** Containers de dados puros (sem encapsulamento, sem lógica). Idealmente structs para performance
- **Entity Systems:** Singletons que contêm toda a lógica e comportamento. Recebem callbacks via event subscriptions

### Tipos de Eventos

- **Directed Events:** Levantados em entidades específicas, com ordem de callback configurável. Mais performáticos e preferidos.
- **Broadcast Events:** Levantados sem vínculo a entidades específicas. Entity Systems assinam e recebem callbacks globais.

### Ciclo de Vida dos Entity Systems

- **Servidor:** Lifetime igual ao do programa
- **Cliente:** Criados ao conectar a um servidor, destruídos ao desconectar. Limpeza de shutdown deve ser feita com cuidado.

### Dependências entre Systems

Entity Systems podem declarar dependências entre si usando o atributo `[Dependency]`, similar ao padrão IoC.

### Exemplo: StackSystem

O `StackSystem` ilustra o padrão ECS: em vez de colocar lógica no componente `StackComponent`, métodos como `Use()` e `Split()` ficam no sistema, que cuida de dirty-checking para rede, aparência visual, e eventos.

---

## Motor Robust Toolbox

O **Robust Toolbox** é o motor de jogo customizado do SS14, escrito em C#.

- **Repositório:** [github.com/space-wizards/RobustToolbox](https://github.com/space-wizards/RobustToolbox)
- **Licença:** MPL v2.0
- **Analogia:** Funciona como o BYOND funciona para o SS13 — é a base sobre a qual o conteúdo do jogo é construído
- **Suporte:** Projetado para jogos multiplayer, mas com ambições de suporte singleplayer
- **Física:** Baseada no Box2D, com broadphase customizado, solver paralelo, e suporte a sleep de corpos físicos
- **UI:** Sistema XAML próprio com code-behind em C#
- **Networking:** Sincronização automática de componentes com dirty-checking

---

## Como Contribuir

O projeto é aberto a contribuições de qualquer pessoa:

1. **Discord:** Canal mais ativo para comunicação entre desenvolvedores
2. **Issues no GitHub:** Lista de tarefas disponíveis para pickup
3. **Documentação:** Wiki open-source no GitHub, editável via webedit PR
4. **Traduções:** Aceitas no repositório principal
5. **Guias úteis:** "Acronyms & Common Nomenclature", artigos de Setup e Tips

### Setup de Desenvolvimento

```bash
# 1. Clone o repositório
git clone https://github.com/space-wizards/space-station-14.git

# 2. Inicialize submódulos e baixe a engine
python RUN_THIS.py

# 3. Compile
dotnet build
```

---

## Forks e Variantes Notáveis

O SS14 sendo open-source gerou diversos forks com experiências de gameplay distintas:

### Frontier Station
Overhaul completo focado em pilotar naves próprias. Jogadores usam uma carteira persistente in-game para comprar naves, explorar o setor de fronteira, e ganhar dinheiro.
- Wiki: [frontierstation.wiki.gg](https://frontierstation.wiki.gg/)

### Einstein Engines
Hard fork baseado nos ideais da família Baystation do SS13. Foco em código modular para servidores de RP. Governança democrática com contribuidores e downstreams tendo voto.
- Repositório: [github.com/Simple-Station/Einstein-Engines](https://github.com/Simple-Station/Einstein-Engines)

### Monolith
Fork do Frontier Station com foco em combate nave-a-nave e liberdade do jogador. Regras de engajamento mais permissivas.

### Hullrot
Fork HRP (High Role-Play) do Frontier Station, com forte foco em combate entre jogadores e facções.

---

## Jogos Similares e Inspirações

### Predecessores Diretos

| Jogo | Descrição |
|------|-----------|
| **Space Station 13** | O original (2003). RPG multiplayer em BYOND com simulação profunda de estação espacial. Múltiplos servidores com codebases distintas (Goonstation, /tg/station, Baystation, Paradise Station, CM-SS13). |
| **Unitystation** | Remake open-source do SS13 em Unity3D, usando /tg/station como guia. |
| **RE:SS3D** | Remake 3D open-source do SS13, desenvolvimento comunitário. |

### Jogos com Mecânicas Similares

| Jogo | Similaridade com SS14 |
|------|----------------------|
| **Barotrauma** | Cooperação multiplayer em submarino em Europa (lua de Júpiter). Mecânicas muito similares ao SS13, com ênfase em horror. |
| **Stationeers** | Simulação detalhada de estação espacial (temperatura, pressão, gases, agricultura). Foco em logística realista de sobrevivência. |
| **Dwarf Fortress** | Caos emergente e complexidade sistêmica. Gerenciamento de colônia anã com desastres imprevisíveis. |
| **Rimworld** | Gerenciamento de colônia com personagens autônomos. Visão macro similar ao SS13. |
| **Cataclysm: Dark Days Ahead** | Sobrevivência pós-apocalíptica com gráficos e mecânicas estilo SS13, porém singleplayer. |
| **Town of Salem** | Foco em paranoia e dedução social, similar ao aspecto de traidor/antagonista do SS13. |

### Remakes Notáveis (Histórico)

- **Standalone SS13 (2011–2015):** Remake em C#/XNA pela Goonstation. Fechado e open-sourced.
- **Complex/Ion (2011):** Tentativa de port para C++/OpenGL pela Baystation12.
- **Centration:** Remake 3D, inicialmente em Unity, depois Unreal Engine.
- **SpessVR:** Remake focado em VR, código fechado, status incerto.

---

## Links e Recursos

### Oficiais do SS14

| Recurso | URL |
|---------|-----|
| Site oficial | [spacestation14.com](https://spacestation14.com/) |
| Wiki do jogador | [wiki.spacestation14.com](https://wiki.spacestation14.com/wiki/Main_Page) |
| Documentação dev (The Robust Book) | [docs.spacestation14.com](https://docs.spacestation14.com/) |
| GitHub - Game | [github.com/space-wizards/space-station-14](https://github.com/space-wizards/space-station-14) |
| GitHub - Engine | [github.com/space-wizards/RobustToolbox](https://github.com/space-wizards/RobustToolbox) |
| GitHub - Docs | [github.com/space-wizards/docs](https://github.com/space-wizards/docs) |
| Steam | [store.steampowered.com/app/1255460](https://store.steampowered.com/app/1255460/Space_Station_14/) |
| Discord | Acessível via site oficial |
| Fandom Wiki | [space-station-14.fandom.com](https://space-station-14.fandom.com/wiki/Space_Station_14_Wiki) |

### Wikis de Projetos Similares

| Projeto | URL |
|---------|-----|
| /tg/station 13 Wiki | [wiki.tgstation13.org](https://wiki.tgstation13.org/) |
| BeeStation Wiki | [wiki.beestation13.com](https://wiki.beestation13.com/) |
| Frontier Station Wiki | [frontierstation.wiki.gg](https://frontierstation.wiki.gg/) |
| SS13 Wikipedia | [en.wikipedia.org/wiki/Space_Station_13](https://en.wikipedia.org/wiki/Space_Station_13) |
| Barotrauma Wiki | [barotraumagame.com/wiki](https://barotraumagame.com/wiki/) |

---

*Documento compilado em Março de 2026 a partir de fontes públicas: docs.spacestation14.com, wiki.spacestation14.com, GitHub, Steam, Fandom, Wikipedia e comunidades relacionadas.*
