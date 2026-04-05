## UI
cargo-console-menu-title = Console de solicitação de carga
cargo-console-menu-account-name-label = Conta:{" "}
cargo-console-menu-account-name-none-text = Nenhuma
cargo-console-menu-account-name-format = [bold][color={$color}]{$name}[/color][/bold] [font="Monospace"]\[{$code}\][/font]
cargo-console-menu-shuttle-name-label = Nome do ônibus:{" "}
cargo-console-menu-shuttle-name-none-text = Nenhum
cargo-console-menu-points-label = Saldo:{" "}
cargo-console-menu-points-amount = ${$amount}
cargo-console-menu-shuttle-status-label = Status do ônibus:{" "}
cargo-console-menu-shuttle-status-away-text = Ausente
cargo-console-menu-order-capacity-label = Capacidade de pedidos:{" "}
cargo-console-menu-call-shuttle-button = Ativar telepad
cargo-console-menu-permissions-button = Permissões
cargo-console-menu-categories-label = Categorias:{" "}
cargo-console-menu-search-bar-placeholder = Buscar
cargo-console-menu-requests-label = Solicitações
cargo-console-menu-orders-label = Pedidos
cargo-console-menu-order-reason-description = Motivos: {$reason}
cargo-console-menu-populate-categories-all-text = Todos
cargo-console-menu-populate-orders-cargo-order-row-product-name-text = {$productName} (x{$orderAmount}) por {$orderRequester} de [color={$accountColor}]{$account}[/color]
cargo-console-menu-cargo-order-row-approve-button = Aprovar
cargo-console-menu-cargo-order-row-cancel-button = Cancelar
cargo-console-menu-tab-title-orders = Pedidos
cargo-console-menu-tab-title-funds = Transferências
cargo-console-menu-account-action-transfer-limit = [bold]Limite de Transferência:[/bold] ${$limit}
cargo-console-menu-account-action-transfer-limit-unlimited-notifier = [color=gold](Ilimitado)[/color]
cargo-console-menu-account-action-select = [bold]Ação da Conta:[/bold]
cargo-console-menu-account-action-amount = [bold]Valor:[/bold] $
cargo-console-menu-account-action-button = Transferir
cargo-console-menu-toggle-account-lock-button = Alternar Limite de Transferência
cargo-console-menu-account-action-option-withdraw = Retirar Dinheiro
cargo-console-menu-account-action-option-transfer = Transferir Fundos para {$code}

# Orders
cargo-console-order-not-allowed = Acesso não permitido
cargo-console-station-not-found = Nenhuma estação disponível
cargo-console-invalid-product = ID de produto inválido
cargo-console-too-many = Muitos pedidos aprovados
cargo-console-snip-snip = Pedido cortado para capacidade
cargo-console-insufficient-funds = Fundos insuficientes (requer {$cost})
cargo-console-unfulfilled = Sem espaço para atender pedido
cargo-console-trade-station = Enviado para {$destination}
cargo-console-unlock-approved-order-broadcast = [bold]{$productName} x{$orderAmount}[/bold], que custou [bold]{$cost}[/bold], foi aprovado por [bold]{$approver}[/bold]
cargo-console-fund-withdraw-broadcast = [bold]{$name} retirou {$amount} spesos de {$name1} \[{$code1}\]
cargo-console-fund-transfer-broadcast = [bold]{$name} transferiu {$amount} spesos de {$name1} \[{$code1}\] para {$name2} \[{$code2}\][/bold]
cargo-console-fund-transfer-user-unknown = Desconhecido

cargo-console-paper-reason-default = Nenhum
cargo-console-paper-approver-default = Próprio
cargo-console-paper-print-name = Pedido #{$orderNumber}
cargo-console-paper-print-text = [head=2]Pedido #{$orderNumber}[/head]
    {"[bold]Item:[/bold]"} {$itemName} (x{$orderQuantity})
    {"[bold]Solicitado por:[/bold]"} {$requester}

    {"[head=3]Informações do Pedido[/head]"}
    {"[bold]Pagador[/bold]:"} {$account} [font="Monospace"]\[{$accountcode}\][/font]
    {"[bold]Aprovado por:[/bold]"} {$approver}
    {"[bold]Motivo:[/bold]"} {$reason}

# Cargo shuttle console
cargo-shuttle-console-menu-title = Console do ônibus de carga
cargo-shuttle-console-station-unknown = Desconhecido
cargo-shuttle-console-shuttle-not-found = Não encontrado
cargo-shuttle-console-organics = Formas de vida orgânicas detectadas no ônibus
cargo-no-shuttle = Nenhum ônibus de carga encontrado!

# Funding allocation console
cargo-funding-alloc-console-menu-title = Console de Alocação de Fundos
cargo-funding-alloc-console-label-account = [bold]Conta[/bold]
cargo-funding-alloc-console-label-code = [bold] Código [/bold]
cargo-funding-alloc-console-label-balance = [bold] Saldo [/bold]
cargo-funding-alloc-console-label-cut = [bold] Divisão de Receita (%) [/bold]

cargo-funding-alloc-console-label-primary-cut = Parcela do Carga dos fundos de fontes não-lockbox (%):
cargo-funding-alloc-console-label-lockbox-cut = Parcela do Carga dos fundos de vendas lockbox (%):

cargo-funding-alloc-console-label-help-non-adjustible = Carga recebe {$percent}% dos lucros de vendas não-lockbox. O restante é dividido conforme especificado abaixo:
cargo-funding-alloc-console-label-help-adjustible = Os fundos restantes de fontes não-lockbox são distribuídos conforme especificado abaixo:
cargo-funding-alloc-console-button-save = Salvar Alteranças
cargo-funding-alloc-console-label-save-fail = [bold]Divisões de Receita Inválidas![/bold] [color=red]({$pos ->
    [1] +
    *[-1] -
}{$val}%)[/color]

# Slip template
cargo-acquisition-slip-body = [head=3]Detalhe do Ativo[/head]
    {"[bold]Produto:[/bold]"} {$product}
    {"[bold]Descrição:[/bold]"} {$description}
    {"[bold]Custo unitário:[/bold"}] ${$unit}
    {"[bold]Quantidade:[/bold]"} {$amount}
    {"[bold]Custo:[/bold]"} ${$cost}

    {"[head=3]Detalhe da Compra[/head]"}
    {"[bold]Pedido por:[/bold]"} {$orderer}
    {"[bold]Motivo:[/bold]"} {$reason}
