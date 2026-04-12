namespace Projeto1IA
{
    public class Crewmate : IAgent
    {
        public int Energy { get; set; }
        public int Health { get; set; }
        public int AssignedDorm { get; set; }
        public IAgentStates StateMachine { get; set; }

        public Crewmate(int dorm)
        {
            Energy = 100;
            Health = 100;
            StateMachine = new CrewmateStates();
            AssignedDorm = dorm;
        }
    }
}
