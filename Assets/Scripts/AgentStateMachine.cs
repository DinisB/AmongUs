using UnityEngine;
using UnityEngine.AI;
using Active.Core;

namespace Projeto1IA
{
    public abstract class AgentStateMachine : IEmergencyResponder
    {
        protected bool _isInEmergency;
        protected bool _isEvacuating;
        protected float _emergencyMultiplier = 1.5f;
        protected NavMeshAgent _navAgent;

        public void SetNavAgent(NavMeshAgent _agent) => _navAgent = _agent;

        public void TriggerEmergency()
        {
            _isInEmergency = true;
            if (_navAgent != null) _navAgent.speed *= _emergencyMultiplier;
        }

        public void TriggerEvacuation()
        {
            _isEvacuating = true;
            if (_navAgent != null) _navAgent.speed *= _emergencyMultiplier;
        }

        public void ResolveEmergency()
        {
            _isInEmergency = false;
            _isEvacuating = false;
            if (_navAgent != null) _navAgent.speed /= _emergencyMultiplier;
        }

        public abstract void Tick();
        public abstract status RespondToIncident();
        public abstract status Evacuate();
    }
}
