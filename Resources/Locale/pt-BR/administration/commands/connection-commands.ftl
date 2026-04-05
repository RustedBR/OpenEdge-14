## Strings for the "grant_connect_bypass" command.

cmd-grant_connect_bypass-desc = Permite temporariamente um usuário bypassing verificações regulares de conexão.
cmd-grant_connect_bypass-help = Uso: grant_connect_bypass <usuário> [duração em minutos]
    Concede temporariamente a um usuário a capacidade de bypassing restrições de conexão regulares.
    O bypass se aplica apenas a este servidor de jogo e expira após (por padrão) 1 hora.
    Eles poderão participar independentemente de whitelist, panic bunker, ou limite de jogadores.

cmd-grant_connect_bypass-arg-user = <usuário>
cmd-grant_connect_bypass-arg-duration = [duração em minutos]

cmd-grant_connect_bypass-invalid-args = Esperado 1 ou 2 argumentos
cmd-grant_connect_bypass-unknown-user = Incapaz de encontrar usuário '{$user}'
cmd-grant_connect_bypass-invalid-duration = Duração inválida '{$duration}'

cmd-grant_connect_bypass-success = Successfully added bypass for user '{$user}'
