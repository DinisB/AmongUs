using UnityEngine;
using System.Collections.Generic;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private int numberOfCrewmates;
    [SerializeField] private int numberOfRobots;
    private List<IAgent> agents;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        agents = new List<IAgent>();
        for (int i = 0; i < numberOfCrewmates; i++)
        {
            agents.Add(new Crewmate());
        }
        for (int i = 0; i < numberOfRobots; i++)
        {
            agents.Add(new Robot());
        }

        Debug.Log("Number of crewmates: " + agents.FindAll(agent => agent is Crewmate).Count);
        Debug.Log("Number of robots: " + agents.FindAll(agent => agent is Robot).Count);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
