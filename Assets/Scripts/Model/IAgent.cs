using UnityEngine;

namespace Projeto1IA
{
    public interface IAgent
    {
        public int Energy { get; set; }
        public int Health { get; set; }
        public IAgentStates StateMachine { get; set; }
    }
}
