-create-3rd-person =
    { $chance ->
        [1] Cria
        *[other] criar
    }

-cause-3rd-person =
    { $chance ->
        [1] Causa
        *[other] causar
    }

-satiate-3rd-person =
    { $chance ->
        [1] Sacia
        *[other] saciar
    }

reagent-effect-guidebook-create-entity-reaction-effect =
    { $chance ->
        [1] Cria
        *[other] criar
    } { $amount ->
        [1] {INDEFINITE($entname)}
        *[other] {$amount} {MAKEPLURAL($entname)}
    }

reagent-effect-guidebook-explosion-reaction-effect =
    { $chance ->
        [1] Causa
        *[other] causar
    } uma explosão

reagent-effect-guidebook-emp-reaction-effect =
    { $chance ->
        [1] Causa
        *[other] causar
    } um pulso eletromagnético

reagent-effect-guidebook-foam-area-reaction-effect =
    { $chance ->
        [1] Cria
        *[other] criar
    } grandes quantidades de espuma

reagent-effect-guidebook-smoke-area-reaction-effect =
    { $chance ->
        [1] Cria
        *[other] criar
    } grandes quantidades de fumaça

reagent-effect-guidebook-satiate-thirst =
    { $chance ->
        [1] Sacia
        *[other] saciar
    } { $relative ->
        [1] a sede de forma média
        *[other] a sede a {NATURALFIXED($relative, 3)}x da taxa média
    }

reagent-effect-guidebook-satiate-hunger =
    { $chance ->
        [1] Sacia
        *[other] saciar
    } { $relative ->
        [1] a fome de forma média
        *[other] a fome a {NATURALFIXED($relative, 3)}x da taxa média
    }

reagent-effect-guidebook-health-change =
    { $chance ->
        [1] { $healsordeals ->
                [heals] Cura
                [deals] Causa
                *[both] Modifica a saúde em
             }
        *[other] { $healsordeals ->
                    [heals] curar
                    [deals] causar
                    *[both] modificar a saúde em
                 }
    } { $changes }

reagent-effect-guidebook-even-health-change =
    { $chance ->
        [1] { $healsordeals ->
            [heals] Cura uniformemente
            [deals] Causa uniformemente
            *[both] Modifica uniformemente a saúde em
        }
        *[other] { $healsordeals ->
            [heals] curar uniformemente
            [deals] causar uniformemente
            *[both] modificar uniformemente a saúde em
        }
    } { $changes }


reagent-effect-guidebook-status-effect =
    { $type ->
        [add]   { $chance ->
                    [1] Causa
                    *[other] causar
                } {LOC($key)} por pelo menos {NATURALFIXED($time, 3)} {MANY("segundo", $time)} com acumulação
        *[set]  { $chance ->
                    [1] Causa
                    *[other] causar
                } {LOC($key)} por pelo menos {NATURALFIXED($time, 3)} {MANY("segundo", $time)} sem acumulação
        [remove]{ $chance ->
                    [1] Remove
                    *[other] remover
                } {NATURALFIXED($time, 3)} {MANY("segundo", $time)} de {LOC($key)}
    }

reagent-effect-guidebook-set-solution-temperature-effect =
    { $chance ->
        [1] Define
        *[other] definir
    } a temperatura da solução exatamente para {NATURALFIXED($temperature, 2)}k

reagent-effect-guidebook-adjust-solution-temperature-effect =
    { $chance ->
        [1] { $deltasign ->
                [1] Adiciona
                *[-1] Remove
            }
        *[other]
            { $deltasign ->
                [1] adicionar
                *[-1] remover
            }
    } calor da solução até atingir { $deltasign ->
                [1] no máximo {NATURALFIXED($maxtemp, 2)}k
                *[-1] pelo menos {NATURALFIXED($mintemp, 2)}k
            }

reagent-effect-guidebook-adjust-reagent-reagent =
    { $chance ->
        [1] { $deltasign ->
                [1] Adiciona
                *[-1] Remove
            }
        *[other]
            { $deltasign ->
                [1] adicionar
                *[-1] remover
            }
    } {NATURALFIXED($amount, 2)}u de {$reagent} { $deltasign ->
        [1] à
        *[-1] da
    } solução

reagent-effect-guidebook-adjust-reagent-group =
    { $chance ->
        [1] { $deltasign ->
                [1] Adiciona
                *[-1] Remove
            }
        *[other]
            { $deltasign ->
                [1] adicionar
                *[-1] remover
            }
    } {NATURALFIXED($amount, 2)}u de reagentes do grupo {$group} { $deltasign ->
            [1] à
            *[-1] da
        } solução

reagent-effect-guidebook-adjust-temperature =
    { $chance ->
        [1] { $deltasign ->
                [1] Adiciona
                *[-1] Remove
            }
        *[other]
            { $deltasign ->
                [1] adicionar
                *[-1] remover
            }
    } {POWERJOULES($amount)} de calor { $deltasign ->
            [1] ao
            *[-1] do
        } corpo em que está

reagent-effect-guidebook-chem-cause-disease =
    { $chance ->
        [1] Causa
        *[other] causar
    } a doença { $disease }

reagent-effect-guidebook-chem-cause-random-disease =
    { $chance ->
        [1] Causa
        *[other] causar
    } as doenças { $diseases }

reagent-effect-guidebook-jittering =
    { $chance ->
        [1] Causa
        *[other] causar
    } tremores

reagent-effect-guidebook-chem-clean-bloodstream =
    { $chance ->
        [1] Limpa
        *[other] limpar
    } a corrente sanguínea de outros químicos

reagent-effect-guidebook-cure-disease =
    { $chance ->
        [1] Cura
        *[other] curar
    } doenças

reagent-effect-guidebook-cure-eye-damage =
    { $chance ->
        [1] { $deltasign ->
                [1] Causa
                *[-1] Cura
            }
        *[other]
            { $deltasign ->
                [1] causar
                *[-1] curar
            }
    } dano ocular

reagent-effect-guidebook-chem-vomit =
    { $chance ->
        [1] Causa
        *[other] causar
    } vômito

reagent-effect-guidebook-create-gas =
    { $chance ->
        [1] Cria
        *[other] criar
    } { $moles } { $moles ->
        [1] mol
        *[other] moles
    } de { $gas }

reagent-effect-guidebook-drunk =
    { $chance ->
        [1] Causa
        *[other] causar
    } embriaguez

reagent-effect-guidebook-electrocute =
    { $chance ->
        [1] Eletrocuta
        *[other] eletrocutar
    } o metabolizador por {NATURALFIXED($time, 3)} {MANY("segundo", $time)}

reagent-effect-guidebook-emote =
    { $chance ->
        [1] Forçará
        *[other] forçar
    } o metabolizador a [bold][color=white]{$emote}[/color][/bold]

reagent-effect-guidebook-extinguish-reaction =
    { $chance ->
        [1] Extingue
        *[other] extinguir
    } fogo

reagent-effect-guidebook-flammable-reaction =
    { $chance ->
        [1] Aumenta
        *[other] aumentar
    } a inflamabilidade

reagent-effect-guidebook-ignite =
    { $chance ->
        [1] Inflama
        *[other] inflamar
    } o metabolizador

reagent-effect-guidebook-make-sentient =
    { $chance ->
        [1] Torna
        *[other] tornar
    } o metabolizador senciente

reagent-effect-guidebook-make-polymorph =
    { $chance ->
        [1] Transforma
        *[other] transformar
    } o metabolizador em { $entityname }

reagent-effect-guidebook-modify-bleed-amount =
    { $chance ->
        [1] { $deltasign ->
                [1] Induz
                *[-1] Reduz
            }
        *[other] { $deltasign ->
                    [1] induzir
                    *[-1] reduzir
                 }
    } sangramento

reagent-effect-guidebook-modify-blood-level =
    { $chance ->
        [1] { $deltasign ->
                [1] Aumenta
                *[-1] Diminui
            }
        *[other] { $deltasign ->
                    [1] aumentar
                    *[-1] diminuir
                 }
    } o nível de sangue

reagent-effect-guidebook-paralyze =
    { $chance ->
        [1] Paralisa
        *[other] paralisar
    } o metabolizador por pelo menos {NATURALFIXED($time, 3)} {MANY("segundo", $time)}

reagent-effect-guidebook-movespeed-modifier =
    { $chance ->
        [1] Modifica
        *[other] modificar
    } a velocidade de movimento em {NATURALFIXED($walkspeed, 3)}x por pelo menos {NATURALFIXED($time, 3)} {MANY("segundo", $time)}

reagent-effect-guidebook-reset-narcolepsy =
    { $chance ->
        [1] Adia temporariamente
        *[other] adiar temporariamente
    } a narcolepsia

reagent-effect-guidebook-wash-cream-pie-reaction =
    { $chance ->
        [1] Remove
        *[other] remover
    } torta de creme do rosto

reagent-effect-guidebook-cure-zombie-infection =
    { $chance ->
        [1] Cura
        *[other] curar
    } uma infecção zumbi em andamento

reagent-effect-guidebook-cause-zombie-infection =
    { $chance ->
        [1] Transmite
        *[other] transmitir
    } a infecção zumbi ao indivíduo

reagent-effect-guidebook-innoculate-zombie-infection =
    { $chance ->
        [1] Cura
        *[other] curar
    } uma infecção zumbi em andamento e fornece imunidade a infecções futuras

reagent-effect-guidebook-reduce-rotting =
    { $chance ->
        [1] Regenera
        *[other] regenerar
    } {NATURALFIXED($time, 3)} {MANY("segundo", $time)} de decomposição

reagent-effect-guidebook-area-reaction =
    { $chance ->
        [1] Causa
        *[other] causar
    } uma reação de fumaça ou espuma por {NATURALFIXED($duration, 3)} {MANY("segundo", $duration)}

reagent-effect-guidebook-add-to-solution-reaction =
    { $chance ->
        [1] Faz
        *[other] fazer
    } os químicos aplicados a um objeto serem adicionados ao seu recipiente de solução interno

reagent-effect-guidebook-artifact-unlock =
    { $chance ->
        [1] Ajuda
        *[other] ajudar
        } a desbloquear um artefato alienígena.

reagent-effect-guidebook-artifact-durability-restore =
    Restaura {$restored} de durabilidade nos nós ativos de artefatos alienígenas.

reagent-effect-guidebook-plant-attribute =
    { $chance ->
        [1] Ajusta
        *[other] ajustar
    } {$attribute} em [color={$colorName}]{$amount}[/color]

reagent-effect-guidebook-plant-cryoxadone =
    { $chance ->
        [1] Envelhece de volta
        *[other] envelhecer de volta
    } a planta, dependendo da idade e do tempo de crescimento

reagent-effect-guidebook-plant-phalanximine =
    { $chance ->
        [1] Restaura
        *[other] restaurar
    } a viabilidade de uma planta tornada inviável por mutação

reagent-effect-guidebook-plant-diethylamine =
    { $chance ->
        [1] Aumenta
        *[other] aumentar
    } a vida útil e/ou saúde base da planta com 10% de chance para cada

reagent-effect-guidebook-plant-robust-harvest =
    { $chance ->
        [1] Aumenta
        *[other] aumentar
    } a potência da planta em {$increase} até o máximo de {$limit}. Faz a planta perder suas sementes quando a potência atingir {$seedlesstreshold}. Tentar adicionar potência acima de {$limit} pode diminuir a produção com 10% de chance

reagent-effect-guidebook-plant-seeds-add =
    { $chance ->
        [1] Restaura as
        *[other] restaurar as
    } sementes da planta

reagent-effect-guidebook-plant-seeds-remove =
    { $chance ->
        [1] Remove as
        *[other] remover as
    } sementes da planta
