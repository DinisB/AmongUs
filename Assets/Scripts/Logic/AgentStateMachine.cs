using UnityEngine;
using Active.Core;
using static Active.Raw;

namespace Projeto1IA
{
    public abstract class AgentStateMachine : MonoBehaviour, IAgentStates
    {
        protected status currentState;

        protected virtual void Update()
        {
            currentState = Sleep() || Working("") || Fix("");
            if (!currentState.running)
            {
                enabled = false;
            }
        }

        public abstract status Sleep();
        public abstract status Working(string task);
        public abstract status Fix(string task);
    }
}