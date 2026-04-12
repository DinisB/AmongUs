using UnityEngine;
using Active.Core;
using static Active.Raw;

namespace Projeto1IA
{
    public class CrewmateStates : AgentStateMachine
    {
        private float sleepTimer = 0f;
        private float restRequiredDuration = 30f;
        private float workTimer = 0f;
        private float workDuration = 60f;
        private float restockTimer = 0f;
        private float restockDuration = 20f;

        public override status Idle()
        {
            return new status();
        }

        public override status Work(string task)
        {
            workTimer += Time.deltaTime;
            
            if (workTimer >= workDuration)
            {
                workTimer = 0f;
                return new status();
            }
            
            return new status();
        }

        public override status Sleep()
        {
            sleepTimer += Time.deltaTime;
            
            if (sleepTimer >= restRequiredDuration)
            {
                sleepTimer = 0f;
                return new status();
            }
            
            return new status();
        }

        public override status Restock()
        {
            restockTimer += Time.deltaTime;
            
            if (restockTimer >= restockDuration)
            {
                restockTimer = 0f;
                return new status();
            }
            
            return new status();
        }

        public override status Recharge()
        {
            return new status();
        }

        public override status RespondToIncident()
        {
            if (isInEmergency)
            {
                return new status();
            }
            
            return new status();
        }

        public override status Evacuate()
        {
            if (isInEmergency)
            {
                return new status();
            }
            
            return new status();
        }
    }
}