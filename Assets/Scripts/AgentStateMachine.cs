using UnityEngine;
using UnityEngine.AI;

namespace Projeto1IA
{
    /// <summary>
    /// Base class for agent FSM with emergency and evacuation states
    /// </summary>
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
        public abstract void RespondToIncident();
        public abstract void Evacuate();
    }
}