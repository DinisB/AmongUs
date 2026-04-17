using UnityEngine;
using UnityEngine.AI;

namespace Projeto1IA
{
    /// <summary>
    /// Area manager with capacity and random point generation
    /// </summary>
    public class NavigationArea : MonoBehaviour
    {
        [SerializeField] private string areaName;
        [SerializeField] private AreaType areaType;
        [SerializeField] private int capacity = 10;

        private Collider areaCollider;
        private int currentOccupancy = 0;

        public string AreaName => areaName;
        public AreaType AreaType => areaType;
        public int Capacity => capacity;
        public int CurrentOccupancy => currentOccupancy;
        public bool IsFull => currentOccupancy >= capacity;

        private void Awake()
        {
            areaCollider = GetComponent<Collider>();
            if (areaCollider == null)
            {
                Debug.LogError($"NavigationArea {areaName} needs a Collider component!");
            }
        }

        /// <summary>
        /// Get a random point inside this area
        /// </summary>
        /// <returns>Random point within the area bounds</returns>
        public Vector3 GetRandomPointInArea()
        {
            if (areaCollider == null) return transform.position;

            Bounds bounds = areaCollider.bounds;
            Vector3 randomPoint;
            int attempts = 0;
            const int maxAttempts = 10;

            do
            {
                randomPoint = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    bounds.center.y,
                    Random.Range(bounds.min.z, bounds.max.z)
                );
                attempts++;
            } while (!IsPointOnNavMesh(randomPoint) && attempts < maxAttempts);

            return randomPoint;
        }

        private bool IsPointOnNavMesh(Vector3 point)
        {
            NavMeshHit hit;
            return NavMesh.SamplePosition(point, out hit, 1f, NavMesh.AllAreas);
        }

        public void AgentEntered()
        {
            currentOccupancy++;
        }

        public void AgentExited()
        {
            currentOccupancy = Mathf.Max(0, currentOccupancy - 1);
        }
    }
}