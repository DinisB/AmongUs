using UnityEngine;
using System.Collections.Generic;

namespace Projeto1IA
{
    /// <summary>
    /// Manages spawning, and removal of agents
    /// </summary>
    public class AgentManager : MonoBehaviour
    {
        [SerializeField] private int[] numbers;
        [SerializeField] private string[] typesOfAgents;
        [SerializeField] private GameObject[] agentPrefab;
        [SerializeField] private Transform[] dorms;
        [SerializeField] private Transform[] technicalAreas;
        [SerializeField] private float spawnRadius = 2f;

        private IList<AgentController> _controllers = new List<AgentController>();
        private AgentFactory _agentFactory;

        public static AgentManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _agentFactory = new AgentFactory();

            for (int i = 0; i < numbers.Length; i++)
            {
                for (int j = 0; j < numbers[i]; j++)
                {
                    SpawnAgent(i);
                }
            }
        }

        private void SpawnAgent(int typeIndex)
        {
            IStateMachineOwner owner = _agentFactory.CreateAgent(typesOfAgents[typeIndex]);
            Vector3 spawnPos = ResolveSpawnPosition(typeIndex, owner);

            GameObject go = Instantiate(agentPrefab[typeIndex], spawnPos, Quaternion.identity);
            AgentController controller = go.AddComponent<AgentController>();
            controller.Self = owner;
            _controllers.Add(controller);
        }

        /// <summary>
        /// Determine initial spawn position, with crewmates spawning in their assigned dorm, robots in technical areas
        /// </summary>
        /// <param name="typeIndex">Index of agent type</param>
        /// <param name="owner">Agent instance</param>
        /// <returns>Spawn position as Vector3</returns>
        private Vector3 ResolveSpawnPosition(int typeIndex, IStateMachineOwner owner)
        {
            Vector3 pos = Vector3.zero;

            if (typesOfAgents[typeIndex] == "Crewmate" && owner is Crewmate crewmate)
            {
                int dormIndex = Random.Range(0, dorms.Length);
                crewmate.AssignedDorm = dormIndex;
                string dormName = $"Habitation{dormIndex + 1}";

                for (int attempt = 0; attempt < 10 && (pos == Vector3.zero || Physics.CheckSphere(pos, 0.5f)); attempt++)
                    pos = LocationManager.GetRandomPointInLocation(dormName);

                if (pos == Vector3.zero)
                {
                    Vector3 offset = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius));
                    pos = dorms[dormIndex].position + offset;
                }
            }
            else
            {
                for (int attempt = 0; attempt < 10 && (pos == Vector3.zero || Physics.CheckSphere(pos, 0.5f)); attempt++)
                    pos = LocationManager.GetRandomPointInLocationType(LocationType.Technical);

                if (pos == Vector3.zero && technicalAreas.Length > 0)
                {
                    Transform fallback = technicalAreas[Random.Range(0, technicalAreas.Length)];
                    Vector3 offset = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius));
                    pos = fallback.position + offset;
                }
            }

            return pos;
        }

        public IList<AgentController> GetControllers() => _controllers;

        /// <summary>
        /// Remove a controller from the active list, for when agents die
        /// </summary>
        /// <param name="ctrl">Controller to remove</param>
        public void RemoveController(AgentController ctrl)
        {
            _controllers.Remove(ctrl);
        }
    }
}