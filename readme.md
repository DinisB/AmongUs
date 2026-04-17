# Projeto 1 - Cúpula Espacial - IA

#### Autoria:

- Dinis Barroso - A22405350
- Frederico Carvalho - A22406033
- Gonçalo Ribeiro - A22409619

#### Carga de Trabalho:

- Dinis Barroso:

  Programação dos FSM e Sistema de movimento dos agentes, bem como os seus controladores e scripts de construtor e factory.

- Frederico Carvalho:

- Gonçalo Ribeiro:

## Introdução

- Este projeto tenta simular uma colónia espacial que contem um número de agentes, cada um com as suas rotinas e valores.

    Para o conseguir, utilizámos FSM (Finite State Machines), com seleção de comportamentos por prioridade, consoante os valores do agente e os incidentes a ocorrer na cúpula.

    Para movimento, utilizá-mos o NavMesh da Unity, que obtém um ponto livre do sítio onde o agente se quer localizar.

- Na pesquisa usámos como referências os seguintes artigos:

    - **ABMU: An Agent-Based Modelling Framework for Unity3D** para desenvolver o movimento em NavMesh e comportamento dos agentes, onde cada um tem os seus "Tick" por segundo e as suas variáveis, que lhes atualizam a rotina.
    - **Evacuation Simulation Implemented by ABM-BIM of Unity** para desenvolver o comportamento dos agentes consoante um incidente esteja a acontecer.

## Metodologia

![Exemplo 1](ex1.png)

O projeto foi implementado em 2.5D, sem verticalidade por opção, com movimentação dinâmica, onde os agentes tentam encontrar caminhos para evitar colisões, com uma velocidade decidida aleatoriamente.

Os agentes navegam atravez do chão que contém ```NavMeshSurfaces```, em conjunto com os componentes ```Location.cs``` e ```NavigationArea.cs```, que lhes dizem que locais são.

Cada agente contem uma Máquina de Estados finitos (FSM), implementada com ```AgentStateMachine.cs``` que atualiza cada frame pelo método ```Tick.cs```. Inicialmente pensámos em fazer uma Behaviour Tree com o uso de ActiveLT mas mudámos para esta abordagem mais hábil.

Ambos os dois tipos de agentes têm máquinas de estado diferentes, baseados na mesma interface ```IAgentBehaviour.cs```.

### Diagrama UML da classe ```CrewmateStates.cs```

```mermaid
stateDiagram
    [*] --> Working

    Working --> Sleeping : Energy ≤ EnergyThreshold
    Working --> Restocking : RestockNeed ≥ RestockThreshold
    Working --> Emergency : IncidentTriggered()
    Working --> Evacuating : EvacuationTriggered()

    Sleeping --> Working : SleepDuration elapsed + Energy = 100
    Restocking --> Working : RestockDuration elapsed + RestockNeed = 0

    Emergency --> Working : IncidentResolved() && !Evacuating
    Emergency --> Evacuating : EvacuationTriggered()

    Evacuating --> [*] : Reached EscapePod

    note right of Working
        Work(random task)
        lab / greenhouse / warehouse / technical
    end note

    note right of Sleeping
        Move(habitation)
        Restore()
    end note

    note right of Restocking
        Move(warehouse)
        Restock()
    end note

    note right of Emergency
        Move(safe habitation)
        Evacuate()
    end note

    note right of Evacuating
        Move(nearest pod)
    end note
```

### Diagrama UML da classe ```RobotStates.cs```

```mermaid
stateDiagram
    [*] --> Working

    Working --> Recharging : Battery ≤ BatteryMin
    Working --> Emergency : IncidentTriggered()
    Working --> Evacuating : EvacuationTriggered()

    Recharging --> Working : RechargeDuration elapsed + Battery = 100

    Emergency --> Working : IncidentResolved() && !Evacuating
    Emergency --> Evacuating : EvacuationTriggered()
    Emergency --> Recharging : Battery ≤ BatteryMin(emergency)

    Evacuating --> [*] : reached pod

    note right of Working
        Work(random task)
        warehouse / technical / lab
        drains battery
    end note

    note right of Recharging
        Move(technical)
        Recharge()
    end note

    note right of Emergency
        battery = 100 → Move(incident zone)
        low battery → Recharge()
        IsLocationDangerous()
    end note

    note right of Evacuating
        RespondToIncident()
    end note
```

Os valores do simulador podem ser modificados, podendo serem mudados o número de agentes que nascem e quantos agentes podem estar numa ```Location.cs```.

Para os incidentes, decidimos usar a **Observer Pattern**, onde os agentes são avisados quando um incidente está a acontecer, se é uma emergência, se foi resolvida, e se matou algum agente, pelo ```IncidentManager.cs```.

Quando um agente recebe a informação de que aconteceu um incidente, ativa o seu **estado de emergência** e multiplica por 1.5 a sua velocidade.

Consoante o número de módulos agravados, os tripulantes entram em **estado de evacuação**, onde vão todos para uma **EscapePod**.