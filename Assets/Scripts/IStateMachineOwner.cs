namespace Projeto1IA
{
    public interface IStateMachineOwner : IAgent
    {
        AgentStateMachine StateMachine { get; }
    }
}
