using UnityEngine;
using UnityEngine.AI;

namespace Projeto1IA
{
    /// <summary>
    /// Acts as controller for agents
    /// </summary>
    public class AgentController : MonoBehaviour
    {
        private IStateMachineOwner _owner;
        private NavMeshAgent _nav;
        private AgentStateMachine _stateMachine;
        public Location CurrentLocation { get; set; }

        public Incident CurrentIncident { get; set; }

        public IStateMachineOwner Self
        {
            get => _owner;
            set
            {
                _owner = value;
                _stateMachine = value.StateMachine;
                InitStateMachine(value);
            }
        }

        public IAgent Agent => _owner;

        private void Start()
        {
            _nav = GetComponent<NavMeshAgent>();
            _nav.speed = Random.Range(2.5f, 4f);
            _stateMachine.SetNavAgent(_nav);
        }

        private void Update() => _stateMachine.Tick();

        /// <summary>
        /// Set the agents destination on the NavMesh
        /// </summary>
        /// <param name="_pos">Target position</param>
        public void MoveTo(Vector3 _pos)
        {
            if (_nav != null && _nav.isOnNavMesh)
                _nav.SetDestination(_pos);
        }

        /// <summary>
        /// Check if the agent has reached its current destination
        /// </summary>
        /// <returns>True if path is complete</returns>
        public bool HasReachedDestination()
        {
            return _nav != null &&
                   !_nav.pathPending &&
                   _nav.remainingDistance <= _nav.stoppingDistance + 0.1f;
        }

        public void TriggerEmergency() => _stateMachine.TriggerEmergency();
        public void TriggerEvacuation() => _stateMachine.TriggerEvacuation();
        public void ResolveEmergency() => _stateMachine.ResolveEmergency();

        private void InitStateMachine(IStateMachineOwner _owner)
        {
            switch (_stateMachine)
            {
                case CrewmateStates _cs when _owner is Crewmate _c:
                    _cs.SetController(this, _c.AssignedDorm);
                    break;
                case RobotStates _rs:
                    _rs.SetController(this);
                    break;
            }
        }
    }
}