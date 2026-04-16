using UnityEngine;

public class Incident : MonoBehaviour
{
    public IncidentType Type;
    public Vector3 Position;

    public Incident(IncidentType type, Vector3 pos)
    {
        Type = type;
        Position = pos;
    }
}
