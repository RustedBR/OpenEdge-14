## Rev Head

roles-antag-rev-head-name = Chefe Revolucionário
roles-antag-rev-head-objective = Seu objetivo é tomar a estação convertendo pessoas para sua causa e matando toda a equipe de Comando na estação.

head-rev-role-greeting =
    Você é um Chefe Revolucionário.
    Sua tarefa é remover todo o Comando da estação por meio de conversão, morte ou imprisonment.
    O Syndicate financiou você com um flash que converte a tripulação para seu lado.
    Cuidado, isso não funcionará naqueles com um mindshield ou usando proteção ocular.
    Viva la revolución!

head-rev-briefing =
    Use flashes para converter pessoas para sua causa.
    Elimine ou converta todos os chefes para tomar a estação.

head-rev-break-mindshield = O Mindshield foi destruído!

## Rev

roles-antag-rev-name = Revolucionário
roles-antag-rev-objective = Seu objetivo é garantir a segurança e seguir as ordens dos Chefes Revolucionários, além de eliminar ou converter toda a equipe de Comando na estação.

rev-break-control = {$name} lembrou-se de sua verdadeira lealdade!

rev-role-greeting =
    Você é um Revolucionário.
    Sua tarefa é tomar a estação e proteger os Chefes Revolucionários.
    Elimine ou converta toda a equipe de Comando.
    Viva la revolución!

rev-briefing = Ajude seus chefes revolucionários a eliminar todos os chefes para tomar a estação.

## General

rev-title = Revolucionários
rev-description = Os revolucionários estão entre nós.

rev-not-enough-ready-players = Jogadores insuficientes se prepararam para o jogo. Havia {$readyPlayersCount} jogadores preparados de {$minimumPlayers} necessários. Não é possível iniciar uma Revolução.
rev-no-one-ready = Nenhum jogador se preparou! Não é possível iniciar uma Revolução.
rev-no-heads = Não havia Chefes Revolucionários para serem selecionados. Não é possível iniciar uma Revolução.

rev-won = Os Chefes Revolutionários sobreviveram e tomaram o controle da estação com sucesso.

rev-lost = O Comando sobreviveu e matou todos os Chefes Revolucionários.

rev-stalemate = Todos os Chefes Revolucionários e o Comando morreram. É um empate.

rev-reverse-stalemate = Tanto o Comando quanto os Chefes Revolucionários sobreviveram.

rev-headrev-count = {$initialCount ->
    [one] Havia um Chefe Revolucionário:
    *[other] Havia {$initialCount} Chefes Revolucionários:
}

rev-headrev-name-user = [color=#5e9cff]{$name}[/color] ([color=gray]{$username}[/color]) converteu {$count} {$count ->
    [one] pessoa
    *[other] pessoas
}

rev-headrev-name = [color=#5e9cff]{$name}[/color] converteu {$count} {$count ->
    [one] pessoa
    *[other] pessoas
}

## Deconverted window

rev-deconverted-title = Desconvertido!
rev-deconverted-text =
    Como o último Chefe Revolucionário morreu, a revolução acabou.

    Você não é mais um revolucionário, então seja legal.
rev-deconverted-confirm = Confirmar
