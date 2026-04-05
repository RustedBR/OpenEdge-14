### UI

# Shown when a stack is examined in details range
comp-stack-examine-detail-count = {$count ->
    [one] Existe [color={$markupCountColor}]{$count}[/color] objeto
    *[other] Existem [color={$markupCountColor}]{$count}[/color] objetos
} na pilha.

# Stack status control
comp-stack-status = Quantidade: [color=white]{$count}[/color]

### Interaction Messages

# Shown when attempting to add to a stack that is full
comp-stack-already-full = A pilha já está cheia.

# Shown when a stack becomes full
comp-stack-becomes-full = A pilha está cheia agora.

# Text related to splitting a stack
comp-stack-split = Você dividiu a pilha.
comp-stack-split-halve = Dividir
comp-stack-split-too-small = A pilha é muito pequena para dividir.