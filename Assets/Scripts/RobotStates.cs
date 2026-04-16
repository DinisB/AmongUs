using UnityEngine;
using Active.Core;

namespace Projeto1IA
{
    public class RobotStates : AgentStateMachine, IWorkable, IRechargeable
    {
        private readonly ILocationProvider _locations;
        private AgentController _controller;

        private float _battery;
        private float _workTimer;
        private float _rechargeTimer;

        private readonly float _batteryMin;
        private readonly float _batteryDrainWork;
        private readonly float _batteryDrainIdle;
        private readonly float _workDuration;
        private readonly float _rechargeDuration;

        private bool _destinationSet;

        private static readonly string[] _workTasks = { "warehouse", "technical", "lab" };
        private string _currentTask;

        public RobotStates(ILocationProvider locations)
        {
            _locations = locations;
            _battery           = Random.Range(60f, 100f);
            _batteryMin        = Random.Range(15f, 25f);
            _batteryDrainWork  = Random.Range(0.3f, 0.7f);
            _batteryDrainIdle  = Random.Range(0.05f, 0.15f);
            _workDuration      = Random.Range(60f, 120f);
            _rechargeDuration  = Random.Range(30f, 60f);
            _currentTask       = _workTasks[Random.Range(0, _workTasks.Length)];
        }

        public void SetController(AgentController controller) => _controller = controller;

        public override void Tick()
        {
            if (_isEvacuating)   { Evacuate();           return; }
            if (_isInEmergency)  { RespondToIncident();  return; }
            if (_battery <= _batteryMin) { Recharge();   return; }
            Work(_currentTask);
        }

        public status Idle()
        {
            _battery -= _batteryDrainIdle * Time.deltaTime;
            return new status();
        }

        public status Work(string task)
        {
            LocationType targetType = task switch
            {
                "warehouse" => LocationType.Warehouse,
                "technical" => LocationType.Technical,
                "lab"       => LocationType.Laboratory,
                _           => LocationType.Technical
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
                    ChooseAnotherTask();
                    return new status();
                }
            }

            _battery -= _batteryDrainWork * Time.deltaTime;

            if (_controller.HasReachedDestination())
            {
                _workTimer += Time.deltaTime;
                if (_workTimer >= _workDuration)
                {
                    _workTimer = 0f;
                    _destinationSet = false;
                    _currentTask = _workTasks[Random.Range(0, _workTasks.Length)];
                }
            }

            return new status();
        }

        public status Recharge()
        {
            if (!_destinationSet)
            {
                Vector3 pos = _locations.GetRandomPointInLocationType(LocationType.Technical);
                if (pos == Vector3.zero) return new status();
                _controller.MoveTo(pos);
                _destinationSet = true;
            }

            if (_controller.HasReachedDestination())
            {
                _rechargeTimer += Time.deltaTime;
                if (_rechargeTimer >= _rechargeDuration)
                {
                    _battery = 100f;
                    _rechargeTimer = 0f;
                    _destinationSet = false;
                }
            }

            return new status();
        }

        public override status RespondToIncident()
        {
            if (_battery <= _batteryMin)
            {
                if (!_destinationSet)
                {
                    Location area = _locations.FindNearest(_controller.transform.position, LocationType.Technical);
                    if (area == null || IsLocationDangerous(area.LocationName)) return new status();
                    _controller.MoveTo(area.GetRandomPointInLocation());
                    _destinationSet = true;
                }
                _battery -= _batteryDrainWork * Time.deltaTime;
                return new status();
            }

            if (!_destinationSet)
            {
                Incident incident = _controller.CurrentIncident;
                if (incident == null) return new status();

                Location target = LocationManager.GetLocation(incident.OriginLocationName);

                if (target != null && !IsLocationDangerous(target.LocationName))
                {
                    _controller.MoveTo(target.GetRandomPointInLocation());
                }
                else if (target != null)
                {
                    Location[] adjacent = LocationManager.GetAdjacentLocations(target);
                    foreach (Location adj in adjacent)
                    {
                        if (!IsLocationDangerous(adj.LocationName))
                        {
                            _controller.MoveTo(adj.GetRandomPointInLocation());
                            break;
                        }
                    }
                }

                _destinationSet = true;
            }

            _battery -= _batteryDrainWork * Time.deltaTime;

            if (_controller.HasReachedDestination())
                _destinationSet = false;

            return new status();
        }

        public override status Evacuate() => RespondToIncident();

        private bool IsLocationDangerous(string locationName)
        {
            if (IncidentManager.Instance == null) return false;
            return IncidentManager.Instance.IsLocationImpassable(locationName);
        }

        private void ChooseAnotherTask()
        {
            _destinationSet = false;
            _workTimer = 0f;
            _currentTask = _workTasks[Random.Range(0, _workTasks.Length)];
        }

        public float BatteryLevel => _battery;
    }
}
