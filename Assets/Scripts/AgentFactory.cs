namespace Projeto1IA
{
    public class AgentFactory
    {
        public IStateMachineOwner CreateAgent(string type, int dorm = 0) => type switch
        {
            "Crewmate" => new Crewmate(dorm),
            "Robot"    => new Robot(),
            _          => throw new System.ArgumentException("Unknown agent type.")
        };
    }
}
