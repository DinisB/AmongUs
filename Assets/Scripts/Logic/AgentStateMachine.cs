using UnityEngine;
using Active.Core;
using static Active.Raw;

namespace Projeto1IA
{
    public abstract class AgentStateMachine : IAgentStates
    {
        protected status currentState;
        protected bool isInEmergency = false;
        protected float emergencyMultiplier = 1.5f;
        protected bool isEnabled = true;

        public virtual void UpdateState()
        {
            if (isInEmergency)
            {
                currentState = RespondToIncident() || Evacuate();
            }
            else
            {
                currentState = Idle() || Work("") || Sleep() || Restock() || Recharge();
            }

            if (!currentState.running)
            {
                isEnabled = false;
            }
        }

        public abstract status Idle();
        public abstract status Work(string task);
        public abstract status Sleep();
        public abstract status Restock();
        public abstract status Recharge();
        public abstract status RespondToIncident();
        public abstract status Evacuate();

        public void TriggerEmergency()
        {
            isInEmergency = true;
            isEnabled = true;
        }

        public void ResolveEmergency()
        {
            isInEmergency = false;
        }
    }
}