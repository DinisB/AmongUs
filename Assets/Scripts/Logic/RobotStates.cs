using UnityEngine;
using Active.Core;
using static Active.Raw;

namespace Projeto1IA
{
    public class RobotStates : AgentStateMachine
    {
        private float batteryLevel = 100f;
        private float batteryMin = 20f;
        private float rechargeTimer = 0f;
        private float rechargeDuration = 45f;
        private float workTimer = 0f;
        private float workDuration = 90f;

        public override status Idle()
        {
            batteryLevel -= Time.deltaTime * 0.1f;
            return new status();
        }

        public override status Work(string task)
        {
            batteryLevel -= Time.deltaTime * 0.5f;
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
            return new status();
        }

        public override status Restock()
        {
            return new status();
        }

        public override status Recharge()
        {
            if (batteryLevel < batteryMin)
            {
                rechargeTimer += Time.deltaTime;
                
                if (rechargeTimer >= rechargeDuration)
                {
                    batteryLevel = 100f;
                    rechargeTimer = 0f;
                    return new status();
                }
                
                return new status();
            }
            
            return new status();
        }

        public override status RespondToIncident()
        {
            if (isInEmergency)
            {
                batteryLevel -= Time.deltaTime * 1.0f;
                return new status();
            }
            
            return new status();
        }

        public override status Evacuate()
        {
            if (isInEmergency)
            {
                return RespondToIncident();
            }
            
            return new status();
        }

        public float GetBatteryLevel()
        {
            return batteryLevel;
        }
    }
}