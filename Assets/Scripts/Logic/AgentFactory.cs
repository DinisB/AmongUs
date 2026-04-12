namespace Projeto1IA
{
    public class AgentFactory
    {
        public IAgent CreateAgent(string type, int dorm = 0)
        {
            switch (type)
            {
                case "Crewmate":
                    return new Crewmate(dorm);
                case "Robot":
                    return new Robot();
                default:
                    throw new System.ArgumentException("Invalid agent type");
            }
        }
    }
}