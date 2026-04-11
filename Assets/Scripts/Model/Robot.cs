namespace Projeto1IA
{
    public class Robot : IAgent
    {
        public int Energy { get; set; }
        public int Health { get; set; }

        public Robot()
        {
            Energy = 150;
            Health = 150;
        }
    }
}
