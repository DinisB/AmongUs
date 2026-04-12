
using UnityEngine;
using System.Collections.Generic;

namespace Projeto1IA
{
    public class AgentManager : MonoBehaviour
    {
        [SerializeField] private int[] numbers;
        [SerializeField] private string[] typesOfAgents;
        [SerializeField] private GameObject[] agentPrefab;
        [SerializeField] private Transform[] dorms;
        [SerializeField] private Transform workstations;
        [SerializeField] private float spawnRadius;
        private List<IAgent> agents;
        private List<AgentController> controllers;
        private AgentFactory agentFactory;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            agentFactory = new AgentFactory();
            agents = new List<IAgent>();
            controllers = new List<AgentController>();

            for (int i = 0; i < numbers.Length; i++)
            {
                for (int j = 0; j < numbers[i]; j++)
                {
                    IAgent agent = agentFactory.CreateAgent(typesOfAgents[i]);
                    agents.Add(agent);

                    Transform spawnpoint = i == 0 ? dorms[Random.Range(0, dorms.Length)] : workstations;

                    Vector3 spawnPosition;
                    int attempts = 0;
                    do // cria numa posição que não esteja perto de outro agente
                    {
                        Vector3 randomOffset = new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius));
                        spawnPosition = spawnpoint.position + randomOffset;
                        attempts++;
                    } 
                    
                    while (Physics.CheckSphere(spawnPosition, 0.5f) && attempts < 10);

                    GameObject go = Instantiate(agentPrefab[i], spawnPosition, Quaternion.identity);
                    AgentController controller = go.AddComponent<AgentController>();
                    controller.Self = agent;
                    controllers.Add(controller);
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
        
        }
    }
}