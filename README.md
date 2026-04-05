# ⚔️ OpenEdge 14

Um fork medieval de **Space Station 14** com foco em sandbox económico, roleplay medieval e desenvolvimento comunitário.

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

Veja [Development Setup](DOCS/Development/00-Getting-Started.md)

```bash
# Compilar
dotnet build -c Debug

# Rodar servidor (com lobby)
DOTNET_ConfigPresets="_OE14/Dev" dotnet run --project Content.Server

# Rodar cliente
dotnet run --project Content.Client
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

### API Reference
- [Systems](DOCS/API/Systems.md) - Principais sistemas
- [Components](DOCS/API/Components.md) - Componentes disponíveis
- [Events](DOCS/API/Events.md) - Eventos do sistema

## 🏗️ Estrutura do Projeto

```
OpenEdge-14/
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
│   ├── Development/
│   └── API/
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
DOTNET_ConfigPresets="_OE14/Dev" dotnet run --project Content.Server
```

Acesse: `localhost:1212` (no launcher do SS14)

## 📊 Status do Projeto

- ✅ Setup inicial
- ✅ Mapas: Comoss, Venicialis
- ✅ Servidor com lobby
- 🚧 Documentação completa
- 🚧 Tutoriais de desenvolvimento
- 📋 Mais features em planejamento

## 🤝 Contribuindo

Este é um projeto pessoal para exploração e aprendizado. Sinta-se livre para:
- Adicionar novas features
- Melhorar documentação
- Corrigir bugs
- Sugerir ideias

## ⚠️ Política de IA

- **Código**: Pode usar IA como assistência, mas revise bem antes de commitar
- **Documentação**: IA é uma excelente ferramenta, use à vontade
- **Conceito**: Criatividade e ideias devem vir de você

## 📝 Licença

Baseado em [Space Station 14](https://github.com/space-wizards/space-station-14) sob MIT License.

## 🔗 Links Importantes

- **Space Station 14**: https://spacestation14.io/
- **SS14 Docs**: https://docs.spacestation14.io/
- **Discord**: [Seu servidor discord aqui]

---

**Desenvolvido com ❤️ por RustedBR**

Última atualização: 2026-03-22
