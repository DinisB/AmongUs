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

        public RobotStates(ILocationProvider _locations)
        {
            this._locations = _locations;
            _battery = Random.Range(60f, 100f);
            _batteryMin = Random.Range(15f, 25f);
            _batteryDrainWork = Random.Range(0.3f, 0.7f);
            _batteryDrainIdle = Random.Range(0.05f, 0.15f);
            _workDuration = Random.Range(60f, 120f);
            _rechargeDuration = Random.Range(30f, 60f);
            _currentTask = _workTasks[Random.Range(0, _workTasks.Length)];
        }

        public void SetController(AgentController _controller) => this._controller = _controller;

        public override void Tick()
        {
            if (_isEvacuating || _isInEmergency) { RespondToIncident(); return; }
            if (_battery <= _batteryMin) { Recharge(); return; }
            Work(_currentTask);
        }

        public status Idle()
        {
            _battery -= _batteryDrainIdle * Time.deltaTime;
            return new status();
        }

        public status Work(string _task)
        {
            LocationType _targetType = _task switch
            {
                "warehouse" => LocationType.Warehouse,
                "technical" => LocationType.Technical,
                "lab"       => LocationType.Laboratory,
                _           => LocationType.Technical
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
                Vector3 _pos = _locations.GetRandomPointInLocationType(LocationType.Technical);
                if (_pos == Vector3.zero) return new status();
                _controller.MoveTo(_pos);
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
            if (!_destinationSet)
            {
                Location _area = _locations.FindNearest(_controller.transform.position, LocationType.Technical);
                if (_area == null) return new status();

                Vector3 _pos = _area.GetRandomPointInLocation();
                if (_pos == Vector3.zero) return new status();

                _controller.MoveTo(_pos);
                _destinationSet = true;
            }

            _battery -= _batteryDrainWork * Time.deltaTime;
            return new status();
        }
        void ChooseAnotherTask()
        {
            _destinationSet = false;
            _workTimer = 0f;

            _currentTask = _workTasks[Random.Range(0, _workTasks.Length)];
        }

        public override status Evacuate() => RespondToIncident();

        public float BatteryLevel => _battery;
    }
}
