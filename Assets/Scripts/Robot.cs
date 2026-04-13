using UnityEngine;

namespace Projeto1IA
{
    public class Robot : IStateMachineOwner
    {
        public int Energy { get; set; }
        public int Health { get; set; }
        public AgentStateMachine StateMachine { get; }

        public Robot()
        {
            Energy = Random.Range(80, 150);
            Health = 150;
            StateMachine = new RobotStates(new LocationProvider());
        }
    }
}
