generator-clogged = {CAPITALIZE(THE($generator))} desliga abruptamente!

portable-generator-verb-start = Ligar gerador
portable-generator-verb-start-msg-unreliable = Ligar o gerador. Pode levar algumas tentativas.
portable-generator-verb-start-msg-reliable = Ligar o gerador.
portable-generator-verb-start-msg-unanchored = O gerador deve ser fixado primeiro!
portable-generator-verb-stop = Desligar gerador
portable-generator-start-fail = Você puxa a corda, mas ele não liga.
portable-generator-start-success = Você puxa a corda, e ele zumbe para a vida.

portable-generator-ui-title = Gerador Portátil
portable-generator-ui-status-stopped = Parado:
portable-generator-ui-status-starting = Ligando:
portable-generator-ui-status-running = Rodando:
portable-generator-ui-start = Ligar
portable-generator-ui-stop = Desligar
portable-generator-ui-target-power-label = Potência Alvo (kW):
portable-generator-ui-efficiency-label = Eficiência:
portable-generator-ui-fuel-use-label = Uso de combustível:
portable-generator-ui-fuel-left-label = Combustível restante:
portable-generator-ui-clogged = Contaminantes detectados no tanque de combustível!
portable-generator-ui-eject = Ejetar
portable-generator-ui-eta = (~{ $minutes } min)
portable-generator-ui-unanchored = Não fixado
portable-generator-ui-current-output = Saída atual: {$voltage}
portable-generator-ui-network-stats = Rede:
portable-generator-ui-network-stats-value = { POWERWATTS($supply) } / { POWERWATTS($load) }
portable-generator-ui-network-stats-not-connected = Não conectado

power-switchable-generator-examine = A saída de energia está configurada para {$voltage}.
power-switchable-generator-switched = Saída alterada para {$voltage}!

power-switchable-voltage = { $voltage ->
    [HV] [color=orange]HV[/color]
    [MV] [color=yellow]MV[/color]
    *[LV] [color=green]LV[/color]
}
power-switchable-switch-voltage = Alternar para {$voltage}

fuel-generator-verb-disable-on = Desligue o gerador primeiro!