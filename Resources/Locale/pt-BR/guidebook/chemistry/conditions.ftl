reagent-effect-condition-guidebook-total-damage =
    { $max ->
        [2147483648] tem pelo menos {NATURALFIXED($min, 2)} de dano total
        *[other] { $min ->
                    [0] tem no máximo {NATURALFIXED($max, 2)} de dano total
                    *[other] tem entre {NATURALFIXED($min, 2)} e {NATURALFIXED($max, 2)} de dano total
                 }
    }

reagent-effect-condition-guidebook-total-hunger =
    { $max ->
        [2147483648] o alvo tem pelo menos {NATURALFIXED($min, 2)} de fome total
        *[other] { $min ->
                    [0] o alvo tem no máximo {NATURALFIXED($max, 2)} de fome total
                    *[other] o alvo tem entre {NATURALFIXED($min, 2)} e {NATURALFIXED($max, 2)} de fome total
                 }
    }

reagent-effect-condition-guidebook-reagent-threshold =
    { $max ->
        [2147483648] há pelo menos {NATURALFIXED($min, 2)}u de {$reagent}
        *[other] { $min ->
                    [0] há no máximo {NATURALFIXED($max, 2)}u de {$reagent}
                    *[other] há entre {NATURALFIXED($min, 2)}u e {NATURALFIXED($max, 2)}u de {$reagent}
                 }
    }

reagent-effect-condition-guidebook-mob-state-condition =
    o mob está { $state }

reagent-effect-condition-guidebook-job-condition =
    o trabalho do alvo é { $job }

reagent-effect-condition-guidebook-solution-temperature =
    a temperatura da solução é { $max ->
            [2147483648] pelo menos {NATURALFIXED($min, 2)}k
            *[other] { $min ->
                        [0] no máximo {NATURALFIXED($max, 2)}k
                        *[other] entre {NATURALFIXED($min, 2)}k e {NATURALFIXED($max, 2)}k
                     }
    }

reagent-effect-condition-guidebook-body-temperature =
    a temperatura corporal é { $max ->
            [2147483648] pelo menos {NATURALFIXED($min, 2)}k
            *[other] { $min ->
                        [0] no máximo {NATURALFIXED($max, 2)}k
                        *[other] entre {NATURALFIXED($min, 2)}k e {NATURALFIXED($max, 2)}k
                     }
    }

reagent-effect-condition-guidebook-organ-type =
    o órgão metabolizador { $shouldhave ->
                                [true] é
                                *[false] não é
                           } um órgão {$name}

reagent-effect-condition-guidebook-has-tag =
    o alvo { $invert ->
                 [true] não possui
                 *[false] possui
                } a tag {$tag}

reagent-effect-condition-guidebook-this-reagent = este reagente

reagent-effect-condition-guidebook-breathing =
    o metabolizador está { $isBreathing ->
                [true] respirando normalmente
                *[false] sufocando
               }

reagent-effect-condition-guidebook-internals =
    o metabolizador está { $usingInternals ->
                [true] usando internos
                *[false] respirando ar atmosférico
               }
