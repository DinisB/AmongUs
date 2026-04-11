using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Projeto1IA
{
    public class AgentController : MonoBehaviour
    {
        private IAgent self;
        private NavMeshAgent navMeshAgent;
        private int health;
        private int energy;

        public IAgent Self
        {
            get { return self; }
            set { self = value; }
        }

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();

            health = self.Health;
            energy = self.Energy;
        }

        public void MoveTo(Vector3 targetPosition)
        {
            navMeshAgent.SetDestination(targetPosition);
        }
    }
}