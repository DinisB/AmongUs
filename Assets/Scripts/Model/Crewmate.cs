namespace Projeto1IA
{
    public class Crewmate : IAgent
    {
        public int Energy { get; set; }
        public int Health { get; set; }

        public Crewmate()
        {
            Energy = 100;
            Health = 100;
        }
    }
}
