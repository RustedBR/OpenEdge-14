# ⚔️ OpenEdge 14

Um fork medieval de **Space Station 14** com foco em sandbox económico, roleplay medieval e desenvolvimento comunitário.

> 🤖 **Este projeto é desenvolvido com auxílio de IA via OpenCode.** Veja a seção [Desenvolvimento com OpenCode](#-desenvolvimento-com-opencode) abaixo para configurar seu ambiente de desenvolvimento com IA.

## 🎮 Sobre o Jogo

OpenEdge 14 reimagina o conceito de Space Station 14 em um cenário de fantasia medieval. Em vez de uma estação espacial, os jogadores exploram e desenvolvem uma cidade medieval com sistemas de crafting, magia, economia e roleplay social.

### Características Principais

- 🏰 **Mundo Medieval**: Castelos, tavernas, minas, florestas
- 🔨 **Crafting**: Ferraria, alquimia, agricultura, culinária
- ✨ **Magia**: Sistema de magia medieval com runas e encantamentos
- 💰 **Economia**: Comércio, moedas, jobs e sistemas de salário
- 📖 **Roleplay**: Chat OOC/IC, nomes personalizados, emotes
- 🗺️ **Múltiplos Mapas**: Comoss, Venicialis e mais

## 🚀 Quick Start

### Para Jogadores

1. **Baixe o launcher**: [Space Station 14 - Official](https://spacestation14.io/)
2. **Adicione servidor customizado**:
   - Host: `localhost` (desenvolvimento)
   - Port: `1212`
3. **Conecte e divirta-se!**

### Para Desenvolvedores

Recomendamos usar o **OpenCode** para desenvolver neste projeto. Veja a seção completa abaixo: [Desenvolvimento com OpenCode](#-desenvolvimento-com-opencode).

Para build manual sem IA:
```bash
git submodule update --init --recursive
dotnet build -c Release
bash runserver.sh
```

## 🤖 Desenvolvimento com OpenCode

OpenEdge 14 usa **OpenCode** como ferramenta principal de desenvolvimento assistido por IA. Todos os comandos, workflow e configurações já estão prontos no repositório.

### Instalação

#### Linux (Arch, Ubuntu, Debian, Fedora...)

```bash
curl -fsSL https://opencode.ai/install | bash
```

Adicione ao PATH se necessário:
```bash
echo 'export PATH="$HOME/.opencode/bin:$PATH"' >> ~/.bashrc
source ~/.bashrc
```

#### Windows (via WSL2)

1. Instale o WSL2 com Ubuntu:
   ```powershell
   wsl --install
   ```
2. Abra o terminal Ubuntu e siga os passos do Linux acima.
3. Clone o repositório dentro do WSL (`~/` ou `/home/usuario/`), **não** em `/mnt/c/` (muito mais lento).

> **Requisitos**: Node.js 18+ ou Bun instalado na máquina.

### Plugins Recomendados (opcionais)

Estes plugins ampliam as capacidades do OpenCode neste projeto:

```bash
# Rodar servidor em background sem bloquear a conversa
opencode plugin install @howaboua/opencode-background-process

# Aplicação rápida de patches de código (mais eficiente)
opencode plugin install @jredeker/opencode-morph-fast-apply
```

### Iniciando uma Sessão

```bash
cd OpenEdge-14
opencode
```

O OpenCode vai carregar o `CLAUDE.md` automaticamente com o contexto completo do projeto.

### Comandos Disponíveis

Estes comandos já estão configurados globalmente para este projeto:

| Comando | Descrição |
|---|---|
| `/build` | Compila o projeto em Release mode |
| `/server` | Inicia o servidor do jogo |
| `/kill-server` | Para o servidor em execução |
| `/doctor` | Diagnóstico do projeto (erros, warnings) |
| `/review` | Code review de um arquivo ou mudança |
| `/plan` | Análise read-only antes de implementar |
| `/status` | Git status atual |
| `/diff` | Ver todas as mudanças pendentes |
| `/test` | Rodar testes do projeto |

### Workflow Recomendado

1. Abra o terminal na pasta do projeto e rode `opencode`
2. Use `/build` para confirmar que o projeto compila sem erros
3. Diga "inicie o servidor" ou use `/server` para subir o game server
4. Faça suas alterações com ajuda da IA
5. Use `/build` novamente para validar (0 erros obrigatório antes de commitar)
6. Use `/review` para revisar o código gerado
7. Faça o commit com a mensagem e o co-author (veja Política de IA abaixo)

### Servidor em Background

Com o plugin `opencode-background-process` instalado, o servidor roda sem bloquear a conversa:

- **"inicie o servidor"** ou `/server` — sobe o servidor em background
- **"mostre os logs do servidor"** — exibe o output em tempo real
- **"pare o servidor"** ou `/kill-server` — encerra o processo

Sem o plugin, use o script diretamente em outro terminal:
```bash
bash runserver.sh
```

### Alternativa: Claude Code

[Claude Code](https://claude.ai/code) também funciona com este projeto — o arquivo `CLAUDE.md` na raiz configura o contexto automaticamente. Instale via:
```bash
npm install -g @anthropic-ai/claude-code
```

## 📚 Documentação

### Para Jogadores
- [Gameplay Guide](DOCS/Gameplay/01-Getting-Started.md) - Como jogar
- [Crafting Guide](DOCS/Gameplay/02-Crafting.md) - Sistemas de crafting
- [Magic Guide](DOCS/Gameplay/03-Magic.md) - Magia e runas
- [Combat Guide](DOCS/Gameplay/04-Combat.md) - Combate e parying

### Para Desenvolvedores
- [Development Setup](DOCS/Development/00-Getting-Started.md) - Configurar ambiente
- [Project Structure](DOCS/Development/01-Project-Structure.md) - Arquitetura
- [Adding Features](DOCS/Development/02-Adding-Features.md) - Como adicionar features
- [Map Creation](DOCS/Development/03-Map-Creation.md) - Como criar mapas
- [Entity System](DOCS/Development/04-Entity-System.md) - Sistema de entidades

## 🏗️ Estrutura do Projeto

```
OpenEdge-14/
├── CLAUDE.md                 # Contexto para IA (OpenCode/Claude Code)
├── Content.Server/           # Código do servidor
├── Content.Client/           # Código do cliente
├── Content.Shared/           # Código compartilhado
├── Resources/
│   ├── Prototypes/          # Definições YAML
│   ├── Prototypes/_OE14/    # Específico OpenEdge
│   ├── Maps/                # Mapas do jogo
│   └── ServerInfo/Guidebook/ # Guidebook in-game
├── DOCS/                    # Documentação do projeto
│   ├── Gameplay/
│   └── Development/
└── RobustToolbox/           # Framework base (submodule)
```

## 🛠️ Build & Deployment

### Requisitos
- .NET 9.0+
- Git com submodules

### Build
```bash
# Download de submodules (primeira vez)
git submodule update --init --recursive

# Debug build (rápido, com info de debug)
dotnet build -c Debug

# Release build (otimizado, mais lento)
dotnet build -c Release
```

### Rodar Servidor Desenvolvimento
```bash
bash runserver.sh
```

Acesse: `localhost:1212` (no launcher do SS14)

## 📊 Status do Projeto

### 🎮 Sistemas de Jogo Implementados

| Sistema | Status | Descrição |
|---------|--------|-----------|
| **Personagem** | ✅ Completo | Stats (STR, VIT, DEX, INT), base 5, limit 1-10 |
| **Sistema de Magia** | ✅ Completo | Árvores de magia, custos de mana, spells |
| **Sistema de Crafting** | ✅ Completo | Workbench, receitas, ferramentas |
| **Sistema de Cooking** | ✅ Completo | Fornalhas, temperatura, receitas |
| **Sistema Modular** | ✅ Completo | Crafting modular com modificadores |
| **Economia** | ✅ Completo | Moedas, salários, trading platform |
| **Farming** | ✅ Completo | Plantas, crescimento, colheitas |
| **Skills** | ✅ Completo | XP por tipo de combate |
| **Status Effects** | ✅ Completo | Queimando, Congelado, Envenenado, Sangrando |
| **Vampiro** | ✅ Completo | Role de vampiro, habilidades específicas |
| **Demiplano** | ✅ Completo | Sistema de viagem dimensional |
| **Combate** | ✅ Completo | Estamina, armas melees, armas ranged, magia de combate |

### 🌍 Mapas

| Mapa | Status |
|------|--------|
| Comoss | ✅ Completo |
| Venicialis | ✅ Completo |
| dev_map | ✅ Completo |

### 👥 Raças (Species)

| Raça | Status |
|------|--------|
| Human | ✅ Completo (5 pontos de stats) |
| Elf | ✅ Completo (+2 INT árvores magia) |
| Dwarf | ✅ Completo |
| Tiefling | ✅ Completo (+INT árvores magia) |
| Silva | ✅ Completo |
| Carcat | ✅ Completo |
| Skeleton | ✅ Completo |
| Zombie | ✅ Completo |

### 🔧 Recursos

| Recurso | Status |
|---------|--------|
| Tradução PT-BR | ✅ Completa (UI, entidades, magias, livros, guidebook) |
| Documentação Desenvolvimento | ✅ Completa (passo a passo) |
| Documentação Gameplay | ✅ Completa |
| CLAUDE.md | ✅ Completo (contexto para IA) |
| Guidebook In-Game | ✅ Completo (múltiplas páginas) |

### 🛠️ Infraestrutura

| Recurso | Status |
|---------|--------|
| OpenCode Commands | ✅ Configurados (/build, /server, /review, etc.) |
| Build System | ✅ Dotnet 9.0+ |
| Servidor | ✅ Porta 1212 |
| git | ✅ Com commits Co-Authored-By |

### 📋 Em Desenvolvimento

- Novos sistemas conforme necessidade
- Novos mapas e áreas
- Novos conteúdos de Guidebook
- Sprites e assets visuais (manual)

## 🤝 Contribuindo

Este é um projeto pessoal para exploração e aprendizado. Sinta-se livre para:
- Adicionar novas features
- Melhorar documentação
- Corrigir bugs
- Sugerir ideias

## ⚠️ Política de IA

Este projeto usa IA como ferramenta de desenvolvimento. As regras são:

- **Código gerado por IA é permitido** — este é um fork pessoal, não há restrições internas
- **Não envie código AI para o upstream** — nunca faça PR com código AI para o repositório Ss14-Medieval original([Cristall-Edge-14](https://github.com/crystallpunk-14/crystall-punk-14))
- **Build obrigatório antes de commitar** — `dotnet build` deve retornar 0 erros

- **Revise antes de commitar** — use `/review` no OpenCode para validar o que foi gerado

## 📝 Licença

Baseado em [Space Station 14](https://github.com/space-wizards/space-station-14) sob MIT License.

## 🔗 Links Importantes

- **Space Station 14**: https://spacestation14.io/
- **SS14 Docs**: https://docs.spacestation14.io/
- **OpenCode**: https://opencode.ai/
- **Discord**: [Seu servidor discord aqui]

---

**Desenvolvido com ❤️ por RustedBR**

Última atualização: 2026-04-05
