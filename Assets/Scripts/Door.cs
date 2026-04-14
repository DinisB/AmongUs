using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    [SerializeField] private bool _vertical = false;
    [SerializeField] private GameObject _doorVisual;

    [SerializeField] private bool _useObstacle = false;

    private Vector3 _ogPosition;

    private NavMeshObstacle _obstacle;
    private Renderer _renderer;

    void Start()
    {
        _ogPosition = _doorVisual.transform.position;

        _obstacle = _doorVisual.GetComponent<NavMeshObstacle>();
        _renderer = _doorVisual.GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        CheckForAgents();

        if (_useObstacle)
        {
            _obstacle.enabled = true;
        }
        else
        {
            _obstacle.enabled = false;
        }
    }

    private void CheckForAgents()
    {
        float radius = _renderer.bounds.size.x * 2f;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            radius,
            LayerMask.GetMask("Agent")
        );

        bool agentDetected = hits.Length > 0;

        if (agentDetected)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        _doorVisual.transform.position =
            _ogPosition +
            (_vertical ? Vector3.up : Vector3.right) * 5f;
    }

    private void CloseDoor()
    {
        _doorVisual.transform.position = _ogPosition;
    }

    public void ActivateObstacle(bool active)
    {
        _useObstacle = true;
    }
}