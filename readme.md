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