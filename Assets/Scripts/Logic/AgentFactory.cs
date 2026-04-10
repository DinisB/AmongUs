namespace Projeto1IA
{
    public class AgentFactory
    {
        public IAgent CreateAgent(string type)
        {
            switch (type)
            {
                case "Crewmate":
                    return new Crewmate();
                case "Robot":
                    return new Robot();
                default:
                    throw new System.ArgumentException("Invalid agent type");
            }
        }
    }
}