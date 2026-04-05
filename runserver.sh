#!/bin/bash

# Verifica se a porta 1212 está em uso
pids=$(lsof -ti:1212 2>/dev/null)

if [ -z "$pids" ]; then
    # Tenta fuser como fallback
    pids=$(fuser 1212/tcp 2>/dev/null)
fi

if [ -n "$pids" ]; then
    echo "Porta 1212 em uso (PID: $pids)."
    printf "Pressione Enter para matar e reiniciar, ou qualquer outra tecla para cancelar: "
    # Lê um caractere sem esperar newline, sem eco
    old_tty=$(stty -g)
    stty -icanon -echo min 1 time 0
    key=$(dd bs=1 count=1 2>/dev/null)
    stty "$old_tty"
    echo ""

    if [ -z "$key" ] || [ "$key" = $'\n' ] || [ "$key" = $'\r' ]; then
        echo "$pids" | xargs kill -9 2>/dev/null
        echo "Processo(s) encerrado(s). Iniciando servidor..."
    else
        echo "Cancelado. Use o servidor já aberto ou feche-o manualmente."
        exit 0
    fi
fi

dotnet run --project Content.Server -c Release -- --cvar loc.culture_name=pt-BR
read -p "Pressione Enter para fechar..."
