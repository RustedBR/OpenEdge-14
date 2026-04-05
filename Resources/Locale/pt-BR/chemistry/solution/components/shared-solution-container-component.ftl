shared-solution-container-component-on-examine-main-text = Contém {INDEFINITE($desc)} [color={$color}]{$desc}[/color] { $chemCount ->
    [1] químico.
   *[other] mistura de químicos.
     }

examinable-solution-has-recognizable-chemicals = Você reconhece {$recognizedString} na solução.
examinable-solution-recognized = [color={$color}]{$chemical}[/color]

examinable-solution-on-examine-volume = A solução contida está { $fillLevel ->
    [exact] segurando [color=white]{$current}/{$max}u[/color].
   *[other] [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
}

examinable-solution-on-examine-volume-no-max = A solução contida está { $fillLevel ->
    [exact] segurando [color=white]{$current}u[/color].
   *[other] [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
}

examinable-solution-on-examine-volume-puddle = A poça está { $fillLevel ->
    [exact] [color=white]{$current}u[/color].
    [full] enorme e transbordando!
    [mostlyfull] enorme e transbordando!
    [halffull] profunda e fluindo.
    [halfempty] muito profunda.
   *[mostlyempty] acumulando-se.
    [empty] formando várias poças pequenas.
}

-solution-vague-fill-level =
    { $fillLevel ->
        [full] [color=white]Cheia[/color]
        [mostlyfull] [color=#DFDFDF]Quase Cheia[/color]
        [halffull] [color=#C8C8C8]Metade[/color]
        [halfempty] [color=#C8C8C8]Metade Vazia[/color]
        [mostlyempty] [color=#A4A4A4]Quase Vazia[/color]
       *[empty] [color=gray]Vazia[/color]
    }
