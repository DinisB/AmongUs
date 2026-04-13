using UnityEngine;

namespace Projeto1IA
{
    public class Crewmate : IStateMachineOwner
    {
        public int Energy { get; set; }
        public int Health { get; set; }
        public int AssignedDorm { get; set; }
        public AgentStateMachine StateMachine { get; }

        public Crewmate(int dorm)
        {
            Energy = Random.Range(60, 100);
            Health = 100;
            AssignedDorm = dorm;
            StateMachine = new CrewmateStates(new LocationProvider(), new NavigationProvider());
        }
    }
}
