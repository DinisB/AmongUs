
using UnityEngine;
using System.Collections.Generic;

namespace Projeto1IA
{
    public class AgentManager : MonoBehaviour
    {
        [SerializeField] private int[] numbers;
        [SerializeField] private string[] typesOfAgents;
        private List<IAgent> agents;
        private AgentFactory agentFactory;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            agentFactory = new AgentFactory();
            agents = new List<IAgent>();

            for (int i = 0; i < numbers.Length; i++)
            {
                for (int j = 0; j < numbers[i]; j++)
                {
                    IAgent agent = agentFactory.CreateAgent(typesOfAgents[i]);
                    agents.Add(agent);
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
}