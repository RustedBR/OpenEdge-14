game-ticker-restart-round = Reiniciando a rodada...
game-ticker-start-round = A rodada está começando agora...
game-ticker-start-round-cannot-start-game-mode-fallback = Falha ao iniciar o modo {$failedGameMode}! Usando padrão {$fallbackMode}...
game-ticker-start-round-cannot-start-game-mode-restart = Falha ao iniciar o modo {$failedGameMode}! Reiniciando a rodada...
game-ticker-start-round-invalid-map = O mapa selecionado {$map} é elegível para o modo de jogo {$mode}. O modo de jogo pode não funcionar como esperado...
game-ticker-unknown-role = Desconhecido
game-ticker-delay-start = O início da rodada foi atrasado por {$seconds} segundos.
game-ticker-pause-start = O início da rodada foi pausado.
game-ticker-pause-start-resumed = A contagem regressiva para o início da rodada foi retomada.
game-ticker-player-join-game-message = Bem-vindo ao Space Station 14! Se é sua primeira vez jogando, leia as regras do jogo e não tenha medo de pedir ajuda no LOOC (OOC local) ou OOC (geralmente disponível apenas entre rodadas).
game-ticker-get-info-text =Olá e bem-vindo ao [color=white]Space Station 14![/color]
                            A rodada atual é: [color=white]#{$roundId}[/color]
                            O número atual de jogadores é: [color=white]{$playerCount}[/color]
                            O mapa atual é: [color=white]{$mapName}[/color]
                            O modo de jogo atual é: [color=white]{$gmTitle}[/color]
                            >[color=yellow]{$desc}[/color]
game-ticker-get-info-preround-text =Olá e bem-vindo ao [color=white]Space Station 14![/color]
                            A rodada atual é: [color=white]#{$roundId}[/color]
                            O número atual de jogadores é: [color=white]{$playerCount}[/color] ([color=white]{$readyCount}[/color] {$readyCount ->
                                [one] está
                                *[other] estão
                            } prontos)
                            O mapa atual é: [color=white]{$mapName}[/color]
                            O modo de jogo atual é: [color=white]{$gmTitle}[/color]
                            >[color=yellow]{$desc}[/color]
game-ticker-no-map-selected = [color=yellow]Mapa ainda não selecionado![/color]
game-ticker-player-no-jobs-available-when-joining = Ao tentar entrar no jogo, nenhum cargo estava disponível.

# Displayed in chat to admins when a player joins
player-join-message = Jogador {$name} entrou.
player-first-join-message = Jogador {$name} entrou pela primeira vez.

# Displayed in chat to admins when a player leaves
player-leave-message = Jogador {$name} saiu.

latejoin-arrival-announcement = {$character} ({$job}) chegou à estação!
latejoin-arrival-announcement-special = {$job} {$character} no deck!
latejoin-arrival-sender = Estação
latejoin-arrivals-direction = Uma nave de transporte até sua estação chegará em breve.
latejoin-arrivals-direction-time = Uma nave de transporte até sua estação chegará em {$time}.
latejoin-arrivals-dumped-from-shuttle = Uma força misteriosa impede você de sair com a nave de chegada.
latejoin-arrivals-teleport-to-spawn = Uma força misteriosa teletransporta você para fora da nave de chegada. Tenha um turno seguro!

preset-not-enough-ready-players = Não é possível iniciar {$presetName}. Requer {$minimumPlayers} jogadores mas temos {$readyPlayersCount}.
preset-no-one-ready = Não é possível iniciar {$presetName}. Nenhum jogador está pronto.

game-run-level-PreRoundLobby = Lobby pré-roda
game-run-level-InRound = Na rodada
game-run-level-PostRound = Pós-roda
