using UnityEngine;
using System;

public class IncidentManager : MonoBehaviour
{
    public static IncidentManager Instance;

    public event Action<Incident> OnIncidentTriggered;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerIncident(IncidentType type)
    {
        // Example incident, replace with actual logic to determine incident type and position
        Incident incident = new Incident(type, Vector3.zero);
        OnIncidentTriggered?.Invoke(incident);
    }
}