using UnityEngine;
using Active.Core;

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
            _locations = locations;
            _navigation = navigation;
            _energy = Random.Range(60f, 100f);
            _restockNeed = Random.Range(0f, 50f);
            _sleepDuration = Random.Range(20f, 40f);
            _workDuration = Random.Range(40f, 80f);
            _restockDuration = Random.Range(10f, 25f);
            _energyDrainRate = Random.Range(1f, 3f);
            _restockDrainRate = Random.Range(0.5f, 1.5f);
            _energyThreshold = Random.Range(20f, 35f);
            _restockThreshold = Random.Range(60f, 80f);
            _currentTask = _workTasks[Random.Range(0, _workTasks.Length)];
        }

        public void SetController(AgentController _controller, int _assignedDorm)
        {
            this._controller = _controller;
            this._assignedDorm = _assignedDorm;
        }

        public override void Tick()
        {
            _energy -= _energyDrainRate * Time.deltaTime;
            _restockNeed += _restockDrainRate * Time.deltaTime;

            if (_isEvacuating) { Evacuate(); return; }
            if (_isInEmergency) { RespondToIncident(); return; }
            if (_energy <= _energyThreshold) { Sleep(); return; }
            if (_restockNeed >= _restockThreshold) { Restock(); return; }
            Work(_currentTask);
        }

        public status Idle() => new status();

        public status Work(string _task)
        {
        
            LocationType _targetType = _task switch
            {
                "lab" => LocationType.Laboratory,
                "greenhouse" => LocationType.Greenhouse,
                "warehouse" => LocationType.Warehouse,
                "technical" => LocationType.Technical,
                _ => LocationType.Laboratory
            };

            if (!_destinationSet)
            {
                Location loc;

                if (_locations.TryGetAvailableLocation(_targetType, out loc))
                {
                    Vector3 _pos = loc.GetRandomPointInLocation();
                    _controller.MoveTo(_pos);
                    _destinationSet = true;
                }
                else
                {
                    _destinationSet = false;
                    return new status();
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

            return new status();
        }

        public status Sleep()
        {
            if (!_destinationSet)
            {
                string dormName = $"Habitation{_assignedDorm + 1}";

                Location dorm = LocationManager.GetLocation(dormName);

                if (dorm != null && dorm.CanEnter())
                {
                    Vector3 _pos = dorm.GetRandomPointInLocation();
                    _controller.MoveTo(_pos);
                    _destinationSet = true;
                }
                else
                {
                    _destinationSet = false;
                    return new status();
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

            return new status();
        }

        public status Restock()
        {
            if (!_destinationSet)
            {
                Location loc;

                if (_locations.TryGetAvailableLocation(LocationType.Warehouse, out loc))
                {
                    Vector3 _pos = loc.GetRandomPointInLocation();
                    _controller.MoveTo(_pos);
                    _destinationSet = true;
                }
                else
                {
                    _destinationSet = false;
                    return new status();
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

            return new status();
        }

        public override status RespondToIncident()
        {
            if (!_destinationSet)
            {
                Location loc;

                if (_locations.TryGetAvailableLocation(LocationType.Habitation, out loc))
                {
                    Vector3 _pos = loc.GetRandomPointInLocation();
                    _controller.MoveTo(_pos);
                    _destinationSet = true;
                }
                else
                {
                    return new status();
                }
            }
            return new status();
        }

        public override status Evacuate()
        {
            if (!_destinationSet)
            {
                NavigationArea _pod = _navigation.FindNearest(_controller.transform.position, AreaType.EscapePod);
                if (_pod == null) return new status();

                Vector3 _pos = _pod.GetRandomPointInArea();
                if (_pos == Vector3.zero) return new status();

                _controller.MoveTo(_pos);
                _destinationSet = true;
            }
            return new status();
        }
    }
}
