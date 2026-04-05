### Comandos de console relacionados ao sistema de votação

## Comando 'createvote'

cmd-createvote-desc = Cria uma votação
cmd-createvote-help = Uso: createvote <'restart'|'preset'|'map'>
cmd-createvote-cannot-call-vote-now = Você não pode chamar uma votação agora!
cmd-createvote-invalid-vote-type = Tipo de votação inválido
cmd-createvote-arg-vote-type = <tipo de votação>

## Comando 'customvote'

cmd-customvote-desc = Cria uma votação personalizada
cmd-customvote-help = Uso: customvote <título> <opção1> <opção2> [opção3...]
cmd-customvote-on-finished-tie = A votação '{$title}' terminou: empate entre {$ties}!
cmd-customvote-on-finished-win = A votação '{$title}' terminou: {$winner} venceu!
cmd-customvote-arg-title = <título>
cmd-customvote-arg-option-n = <opção{ $n }>

## Comando 'vote'

cmd-vote-desc = Vota em uma votação ativa
cmd-vote-help = vote <voteId> <opção>
cmd-vote-cannot-call-vote-now = Você não pode chamar uma votação agora!
cmd-vote-on-execute-error-must-be-player = Deve ser um jogador
cmd-vote-on-execute-error-invalid-vote-id = ID de votação inválido
cmd-vote-on-execute-error-invalid-vote-options = Opções de votação inválidas
cmd-vote-on-execute-error-invalid-vote = Votação inválida
cmd-vote-on-execute-error-invalid-option = Opção inválida

## Comando 'listvotes'

cmd-listvotes-desc = Lista as votações ativas atualmente
cmd-listvotes-help = Uso: listvotes

## Comando 'cancelvote'

cmd-cancelvote-desc = Cancela uma votação ativa
cmd-cancelvote-help = Uso: cancelvote <id>
                      Você pode obter o ID pelo comando listvotes.
cmd-cancelvote-error-invalid-vote-id = ID de votação inválido
cmd-cancelvote-error-missing-vote-id = ID ausente
cmd-cancelvote-arg-id = <id>
