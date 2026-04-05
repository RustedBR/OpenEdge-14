# Displayed as initiator of vote when no user creates the vote
ui-vote-initiator-server = O servidor

## Default.Votes

ui-vote-restart-title = Reiniciar rodada
ui-vote-restart-succeeded = Votação de reinício bem-sucedida.
ui-vote-restart-failed = Votação de reinício falhou (necessário { TOSTRING($ratio, "P0") }).
ui-vote-restart-fail-not-enough-ghost-players = Votação de reinício falhou: Um mínimo de { $ghostPlayerRequirement }% de jogadores fantasma é necessário para iniciar uma votação de reinício. Atualmente, não há jogadores fantasma suficientes.
ui-vote-restart-yes = Sim
ui-vote-restart-no = Não
ui-vote-restart-abstain = Abster-se

ui-vote-gamemode-title = Próximo modo de jogo
ui-vote-gamemode-tie = Empate na votação de modo de jogo! Escolhendo... { $picked }
ui-vote-gamemode-win = { $winner } venceu a votação de modo de jogo!

ui-vote-map-title = Próximo mapa
ui-vote-map-tie = Empate na votação de mapa! Escolhendo... { $picked }
ui-vote-map-win = { $winner } venceu a votação de mapa!
ui-vote-map-notlobby = Votação para mapas é válida apenas no lobby pré-rodada!
ui-vote-map-notlobby-time = Votação para mapas é válida apenas no lobby pré-rodada com { $time } restante!


# Votekick votes
ui-vote-votekick-unknown-initiator = Um jogador
ui-vote-votekick-unknown-target = Jogador Desconhecido
ui-vote-votekick-title = { $initiator } iniciou um votekick para o usuário: { $targetEntity }. Motivo: { $reason }
ui-vote-votekick-yes = Sim
ui-vote-votekick-no = Não
ui-vote-votekick-abstain = Abster-se
ui-vote-votekick-success = Votekick para { $target } bem-sucedido. Motivo do votekick: { $reason }
ui-vote-votekick-failure = Votekick para { $target } falhou. Motivo do votekick: { $reason }
ui-vote-votekick-not-enough-eligible = Eleitores elegíveis insuficientes online para iniciar um votekick: { $voters }/{ $requirement }
ui-vote-votekick-server-cancelled = Votekick para { $target } foi cancelado pelo servidor.