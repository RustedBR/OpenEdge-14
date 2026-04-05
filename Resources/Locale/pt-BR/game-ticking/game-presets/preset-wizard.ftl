## Survivor

roles-antag-survivor-name = Sobrevivente
# É uma referência ao Halo
roles-antag-survivor-objective = Objetivo Atual: Sobreviva

survivor-role-greeting =
    Você é um Sobrevivente.
    Acima de tudo você precisa voltar ao CentComm vivo.
    Reúna o máximo de poder de fogo necessário para garantir sua sobrevivência.
    Não confie em ninguém.

survivor-round-end-dead-count =
{
    $deadCount ->
        [one] [color=red]{$deadCount}[/color] sobrevivente morreu.
        *[other] [color=red]{$deadCount}[/color] sobreviventes morreram.
}

survivor-round-end-alive-count =
{
    $aliveCount ->
        [one] [color=yellow]{$aliveCount}[/color] sobrevivente foi abandonado na estação.
        *[other] [color=yellow]{$aliveCount}[/color] sobreviventes foram abandonados na estação.
}

survivor-round-end-alive-on-shuttle-count =
{
    $aliveCount ->
        [one] [color=green]{$aliveCount}[/color] sobreviveu.
        *[other] [color=green]{$aliveCount}[/color] sobreviveram.
}

## Wizard

objective-issuer-swf = [color=turquoise]A Federação dos Magos Espaciais[/color]

wizard-title = Mago
wizard-description = Há um Mago na estação! Você nunca sabe o que eles podem fazer.

roles-antag-wizard-name = Mago
roles-antag-wizard-objective = Ensine-lhes uma lição que nunca esquecerão.

wizard-role-greeting =
    TU ÉS UM MAGO!
    Houve tensões entre a Federação dos Magos Espaciais e a NanoTrasen.
    Então você foi selecionado pela Federação dos Magos Espaciais para fazer uma visita à estação.
    Dê-lhes uma boa demonstração de seus poderes.
    O que você faz é com você, só lembre que os Magos Espaciais querem que você saia vivo.

wizard-round-end-name = wizard

## TODO: Wizard Apprentice (Coming sometime post-wizard release)
