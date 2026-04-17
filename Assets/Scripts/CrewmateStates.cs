using UnityEngine;

namespace Projeto1IA
{
    public class CrewmateStates : AgentStateMachine, IWorkable, IRestable
    {
        private readonly ILocationProvider _locations;
        private readonly INavigationProvider _navigation;

        private AgentController _controller;
        private int _assignedDorm;

        private float _energy;
        private float _restockNeed;

        private float _sleepTimer;
        private float _workTimer;
        private float _restockTimer;

        private readonly float _sleepDuration;
        private readonly float _workDuration;
        private readonly float _restockDuration;
        private readonly float _energyDrainRate;
        private readonly float _restockDrainRate;
        private readonly float _energyThreshold;
        private readonly float _restockThreshold;

        private bool _destinationSet;
        private string[] _workTasks = { "lab", "greenhouse", "warehouse", "technical" };
        private string _currentTask;

        public CrewmateStates(ILocationProvider locations, INavigationProvider navigation)
        {
            _locations   = locations;
            _navigation  = navigation;
            _energy          = Random.Range(60f, 100f);
            _restockNeed     = Random.Range(0f, 50f);
            _sleepDuration   = Random.Range(20f, 40f);
            _workDuration    = Random.Range(40f, 80f);
            _restockDuration = Random.Range(10f, 25f);
            _energyDrainRate = Random.Range(1f, 3f);
            _restockDrainRate = Random.Range(0.5f, 1.5f);
            _energyThreshold = Random.Range(20f, 35f);
            _restockThreshold = Random.Range(60f, 80f);
            _currentTask     = _workTasks[Random.Range(0, _workTasks.Length)];
        }

        public void SetController(AgentController controller, int assignedDorm)
        {
            _controller  = controller;
            _assignedDorm = assignedDorm;
        }

        public override void Tick()
        {
            _energy      -= _energyDrainRate  * Time.deltaTime;
            _restockNeed += _restockDrainRate * Time.deltaTime;

            if (_isEvacuating)  { Evacuate();           return; }
            if (_isInEmergency) { RespondToIncident();  return; }
            if (_energy <= _energyThreshold)   { Sleep();    return; }
            if (_restockNeed >= _restockThreshold) { Restock(); return; }
            Work(_currentTask);
        }

        public void Idle()
        {
            return;
        }

        public void Work(string task)
        {
            LocationType targetType = task switch
            {
                "lab"        => LocationType.Laboratory,
                "greenhouse" => LocationType.Greenhouse,
                "warehouse"  => LocationType.Warehouse,
                "technical"  => LocationType.Technical,
                _            => LocationType.Laboratory
            };

            if (!_destinationSet)
            {
                if (_locations.TryGetAvailableLocation(targetType, out Location loc) &&
                    !IsLocationDangerous(loc.LocationName))
                {
                    _controller.MoveTo(loc.GetRandomPointInLocation());
                    _destinationSet = true;
                }
                else
                {
                    _destinationSet = false;
                    return;
                }
            }

            if (_controller.HasReachedDestination())
            {
                _workTimer += Time.deltaTime;
                if (_workTimer >= _workDuration)
                {
                    _workTimer = 0f;
                    _destinationSet = false;
                }
            }

            return;
        }

        public void Sleep()
        {
            if (!_destinationSet)
            {
                string dormName = $"Habitation{_assignedDorm + 1}";
                Location dorm = LocationManager.GetLocation(dormName);

                if (dorm != null && dorm.CanEnter() && !IsLocationDangerous(dormName))
                {
                    _controller.MoveTo(dorm.GetRandomPointInLocation());
                    _destinationSet = true;
                }
                else
                {
                    _destinationSet = false;
                    return;
                }
            }

            if (_controller.HasReachedDestination())
            {
                _sleepTimer += Time.deltaTime;
                if (_sleepTimer >= _sleepDuration)
                {
                    _energy = 100f;
                    _sleepTimer = 0f;
                    _destinationSet = false;
                }
            }

            return;
        }

        public void Restock()
        {
            if (!_destinationSet)
            {
                if (_locations.TryGetAvailableLocation(LocationType.Warehouse, out Location loc) &&
                    !IsLocationDangerous(loc.LocationName))
                {
                    _controller.MoveTo(loc.GetRandomPointInLocation());
                    _destinationSet = true;
                }
                else
                {
                    _destinationSet = false;
                    return;
                }
            }

            if (_controller.HasReachedDestination())
            {
                _restockTimer += Time.deltaTime;
                if (_restockTimer >= _restockDuration)
                {
                    _restockNeed = 0f;
                    _restockTimer = 0f;
                    _destinationSet = false;
                }
            }

            return;
        }

        public override void RespondToIncident()
        {
            if (!_destinationSet)
            {
                Location safeHab = FindSafeHabitation();

                if (safeHab != null)
                {
                    _controller.MoveTo(safeHab.GetRandomPointInLocation());
                    _destinationSet = true;
                }
            }

            if (_controller.HasReachedDestination())
                _destinationSet = false;

            return;
        }

        public override void Evacuate()
        {
            if (!_destinationSet)
            {
                NavigationArea pod = FindSafeEscapePod();
                if (pod == null) return;

                Vector3 pos = pod.GetRandomPointInArea();
                if (pos == Vector3.zero) return;

                _controller.MoveTo(pos);
                _destinationSet = true;
            }
            return;
        }


        private bool IsLocationDangerous(string locationName)
        {
            if (IncidentManager.Instance == null) return false;
            return IncidentManager.Instance.IsLocationImpassable(locationName) ||
                   IncidentManager.Instance.IsLocationAffected(locationName);
        }

        private Location FindSafeHabitation()
        {
            Location[] habs = LocationManager.GetLocationsByType(LocationType.Habitation);
            Location nearest = null;
            float minDist = float.MaxValue;

            foreach (Location hab in habs)
            {
                if (IsLocationDangerous(hab.LocationName)) continue;
                float dist = Vector3.Distance(_controller.transform.position, hab.transform.position);
                if (dist < minDist) { minDist = dist; nearest = hab; }
            }

            return nearest;
        }

        private NavigationArea FindSafeEscapePod()
        {
            NavigationArea[] pods = _navigation.GetAreasByType(AreaType.EscapePod);
            NavigationArea nearest = null;
            float minDist = float.MaxValue;

            foreach (NavigationArea pod in pods)
            {
                float dist = Vector3.Distance(_controller.transform.position, pod.transform.position);
                if (dist < minDist) { minDist = dist; nearest = pod; }
            }

            return nearest;
        }
    }
}
