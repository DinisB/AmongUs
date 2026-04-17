using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Projeto1IA
{
    /// <summary>
    /// Represents a physical area in the base with capacity, occupancy tracking, and random point generation
    /// </summary>
    public class Location : MonoBehaviour
    {
        [SerializeField] private LocationType _locationType;
        [SerializeField] private int _locationId = 1;
        [SerializeField] private int _capacity = 10;
        [SerializeField] private Door _door;

        [SerializeField] private IList<Location> _adjacentLocations = new List<Location>();

        private Collider _locationCollider;
        private int _currentOccupancy;
        private float _scanInterval = 0.5f;
        private float _scanTimer;

        public LocationType LocationType => _locationType;
        public int LocationId => _locationId;
        public string LocationName => $"{_locationType}{_locationId}";
        public int Capacity => _capacity;
        public int CurrentOccupancy => _currentOccupancy;
        public bool IsFull => _currentOccupancy >= _capacity;
        public Door Door => _door;
        public bool CanEnter()
        {
            return !IsFull;
        }

        private LayerMask _agentLayer;

        private void Awake()
        {
            _locationCollider = GetComponent<Collider>();
            _agentLayer = FindFirstObjectByType<LocationManager>().GetComponent<LocationManager>().AgentLayer;
            LocationManager.RegisterLocation(this);

            foreach (Location adjacent in _adjacentLocations)
            {
                if (adjacent != null)
                {
                    LocationManager.AddAdjacency(this.LocationName, adjacent.LocationName);
                }
            }
        }

        private void OnDestroy()
        {
            LocationManager.UnregisterLocation(this);
        }

        private void Update()
        {
            _scanTimer += Time.deltaTime;
            if (_scanTimer < _scanInterval) return;
            _scanTimer = 0f;

            Bounds _bounds = _locationCollider.bounds;
            float _radius = Mathf.Max(_bounds.extents.x, _bounds.extents.y, _bounds.extents.z);
            Collider[] _colliders = Physics.OverlapSphere(_bounds.center, _radius, _agentLayer);

            int _count = 0;
            foreach (Collider _col in _colliders)
            {
                if (_bounds.Contains(_col.transform.position))
                    _count++;
            }
            _currentOccupancy = _count;
        }

        /// <summary>
        /// Generate a random point inside this location bounds that lies on the NavMesh
        /// </summary>
        /// <returns>Valid NavMesh point, or zero if none found</returns>
        public Vector3 GetRandomPointInLocation()
        {
            Bounds _bounds = _locationCollider.bounds;
            Vector3 _randomPoint = Vector3.zero;
            int _attempts = 0;

            do
            {
                _randomPoint = new Vector3(
                    Random.Range(_bounds.min.x, _bounds.max.x),
                    _bounds.center.y,
                    Random.Range(_bounds.min.z, _bounds.max.z)
                );
                _attempts++;
            } while (!IsPointOnNavMesh(_randomPoint) && _attempts < 10);

            return _randomPoint;
        }
        private void OnTriggerEnter(Collider other)
        {
            AgentController ctrl = other.GetComponent<AgentController>();
            if (ctrl != null)
            {
                ctrl.CurrentLocation = this;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            AgentController ctrl = other.GetComponent<AgentController>();
            if (ctrl != null && ctrl.CurrentLocation == this)
            {
                ctrl.CurrentLocation = null;
            }
        }

        private bool IsPointOnNavMesh(Vector3 _point)
        {
            NavMeshHit _hit;
            return NavMesh.SamplePosition(_point, out _hit, 1f, NavMesh.AllAreas);
        }
    }
}