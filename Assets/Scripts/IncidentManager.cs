using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Projeto1IA
{
    public class IncidentManager : MonoBehaviour
    {
        public static IncidentManager Instance { get; private set; }
        [SerializeField] private float spreadInterval = 15f;
        [SerializeField] private float maxIncidentDuration = 120f;
        [SerializeField] private int maxAffectedModules = 4;

        [SerializeField] private float killCheckInterval = 0.5f;
        [SerializeField] private float oxygenDangerTolerance = 5f;

        private List<Incident> activeIncidents = new List<Incident>();
        private bool evacuationTriggered = false;

        private Dictionary<AgentController, float> oxygenExposure = new Dictionary<AgentController, float>();

        public event Action<Incident> OnIncidentTriggered;
        public event Action<Incident> OnIncidentSpread;
        public event Action<Incident> OnIncidentResolved;
        public event Action OnEvacuationTriggered;
        public event Action<AgentController> OnAgentKilled;

        private AgentManager agentManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            agentManager = FindFirstObjectByType<AgentManager>();
            StartCoroutine(KillCheckRoutine());
            StartCoroutine(SpreadRoutine());
            StartCoroutine(EscalationCheckRoutine());
        }

        public void TriggerFire() => TriggerIncident(IncidentType.Fire, LocationManager.GetRandomLocation().LocationName);
        public void TriggerOxygenLeak() => TriggerIncident(IncidentType.OxygenLeak, LocationManager.GetRandomLocation().LocationName);
        public void TriggerElectricalFailure() => TriggerIncident(IncidentType.ElectricalFailure, LocationManager.GetRandomLocation().LocationName);
        public void TriggerIncident(IncidentType type, string locationName)
        {
            Location location = LocationManager.GetLocation(locationName);
            if (location.LocationType == LocationType.EscapePod)
            {
                Debug.Log("Can't start incidents in pods");
                return;
            }

            Incident incident = new Incident(type, locationName);
            activeIncidents.Add(incident);

            ApplyOriginEffects(incident);
            NotifyAllAgents(incident);

            OnIncidentTriggered?.Invoke(incident);

            Debug.Log($"[IncidentManager] {type} triggered at {locationName}");
        }

        public void ResolveIncident(Incident incident)
        {
            if (!activeIncidents.Contains(incident)) return;

            if (incident.Type == IncidentType.ElectricalFailure)
                SetDoorsInLocation(incident.OriginLocationName, false);

            incident.Resolve();
            activeIncidents.Remove(incident);

            if (activeIncidents.Count == 0 && !evacuationTriggered)
                ResolveAllAgents();

            OnIncidentResolved?.Invoke(incident);
        }

        public List<Incident> GetActiveIncidents() => activeIncidents;

        public bool IsLocationImpassable(string locationName) =>
            activeIncidents.Any(i => i.IsLocationImpassable(locationName));

        public bool IsLocationAffected(string locationName) =>
            activeIncidents.Any(i => i.IsLocationAffected(locationName));

        private void ApplyOriginEffects(Incident incident)
        {
            switch (incident.Type)
            {
                case IncidentType.Fire:
                    incident.MakeImpassable(incident.OriginLocationName);
                    KillAgentsInLocation(incident.OriginLocationName, killRobots: true);
                    break;

                case IncidentType.OxygenLeak:
                    incident.MakeImpassable(incident.OriginLocationName);
                    KillAgentsInLocation(incident.OriginLocationName, killRobots: false);
                    break;

                case IncidentType.ElectricalFailure:
                    incident.SpreadTo(incident.OriginLocationName);
                    SetDoorsInLocation(incident.OriginLocationName, true);
                    break;
            }
        }

        private IEnumerator SpreadRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spreadInterval);

                foreach (Incident incident in activeIncidents.ToList())
                {
                    if (incident.IsResolved) continue;
                    SpreadIncident(incident);
                }
            }
        }

        private void SpreadIncident(Incident incident)
        {
            List<string> toSpread = new List<string>();

            foreach (string locationName in incident.AffectedLocations.ToList())
            {
                Location loc = LocationManager.GetLocation(locationName);
                if (loc == null) continue;

                Location[] adjacent = LocationManager.GetAdjacentLocations(loc);
                foreach (Location adj in adjacent)
                {
                    if (!incident.IsLocationAffected(adj.LocationName))
                        toSpread.Add(adj.LocationName);
                }
            }

            foreach (string newLocation in toSpread)
            {
                switch (incident.Type)
                {
                    case IncidentType.Fire:
                        incident.MakeImpassable(newLocation);
                        KillAgentsInLocation(newLocation, killRobots: true);
                        break;

                    case IncidentType.OxygenLeak:
                        incident.SpreadTo(newLocation);
                        break;

                    case IncidentType.ElectricalFailure:
                        break;
                }
            }

            if (toSpread.Count > 0)
            {
                NotifyAllAgents(incident);
                OnIncidentSpread?.Invoke(incident);
            }
        }

        private IEnumerator KillCheckRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(killCheckInterval);

                if (agentManager == null) continue;

                foreach (AgentController ctrl in agentManager.GetControllers().ToList())
                {
                    if (ctrl == null) continue;

                    string currentLocation = GetAgentCurrentLocation(ctrl);
                    if (string.IsNullOrEmpty(currentLocation)) continue;

                    foreach (Incident incident in activeIncidents.ToList())
                    {
                        if (incident.IsResolved) continue;

                        if (incident.Type == IncidentType.Fire &&
                            incident.IsLocationImpassable(currentLocation))
                        {
                            KillAgent(ctrl);
                            break;
                        }

                        if (incident.Type == IncidentType.OxygenLeak)
                        {
                            if (ctrl.Self is Crewmate)
                            {
                                if (incident.IsLocationImpassable(currentLocation))
                                {
                                    KillAgent(ctrl);
                                    break;
                                }
                                else if (incident.IsLocationAffected(currentLocation))
                                {
                                    if (!oxygenExposure.ContainsKey(ctrl))
                                        oxygenExposure[ctrl] = 0f;

                                    oxygenExposure[ctrl] += killCheckInterval;

                                    if (oxygenExposure[ctrl] >= oxygenDangerTolerance)
                                    {
                                        KillAgent(ctrl);
                                        break;
                                    }
                                }
                                else
                                {
                                    oxygenExposure.Remove(ctrl);
                                }
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator EscalationCheckRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);

                if (evacuationTriggered) break;

                foreach (Incident incident in activeIncidents.ToList())
                {
                    if (incident.IsResolved) continue;

                    if (incident.AffectedLocations.Count >= maxAffectedModules ||
                        incident.ElapsedTime >= maxIncidentDuration)
                    {
                        TriggerEvacuation();
                        break;
                    }
                }
            }
        }

        private void TriggerEvacuation()
        {
            evacuationTriggered = true;
            Debug.Log("[IncidentManager] EVACUATION triggered.");

            if (agentManager == null) return;

            foreach (AgentController ctrl in agentManager.GetControllers())
            {
                if (ctrl != null)
                    ctrl.TriggerEvacuation();
            }

            OnEvacuationTriggered?.Invoke();
        }

        private void KillAgentsInLocation(string locationName, bool killRobots)
        {
            if (agentManager == null) return;

            foreach (AgentController ctrl in agentManager.GetControllers().ToList())
            {
                if (ctrl == null) continue;
                if (!killRobots && ctrl.Self is Robot) continue;

                if (GetAgentCurrentLocation(ctrl) == locationName)
                    KillAgent(ctrl);
            }
        }

        private void KillAgent(AgentController ctrl)
        {
            if (ctrl == null) return;
            oxygenExposure.Remove(ctrl);

            Debug.Log($"[IncidentManager] Agent killed: {ctrl.gameObject.name}");

            UnityEngine.AI.NavMeshAgent nav = ctrl.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (nav != null)
            {
                nav.isStopped = true;
                nav.enabled = false;
            }

            ctrl.enabled = false;

            if (ctrl.GetComponent<UnityEngine.AI.NavMeshObstacle>() == null)
            {
                UnityEngine.AI.NavMeshObstacle obs = ctrl.gameObject.AddComponent<UnityEngine.AI.NavMeshObstacle>();
                obs.carving = true;
                obs.radius = 0.4f;
                obs.height = 1.8f;
            }

            agentManager.RemoveController(ctrl);
            OnAgentKilled?.Invoke(ctrl);
            ctrl.gameObject.SetActive(false);
        }
        private string GetAgentCurrentLocation(AgentController ctrl)
        {
            foreach (string name in LocationManager.GetAllLocationNames())
            {
                Location loc = LocationManager.GetLocation(name);

                Collider col = loc.GetComponent<Collider>();

                Vector3 closestPoint = col.ClosestPoint(ctrl.transform.position);

                if (Vector3.Distance(closestPoint, ctrl.transform.position) < 0.1f)
                {
                    return name;
                }
            }

            return null;
        }
        private void NotifyAllAgents(Incident incident)
        {
            if (agentManager == null) return;

            foreach (AgentController ctrl in agentManager.GetControllers())
            {
                if (ctrl == null || evacuationTriggered) continue;

                ctrl.TriggerEmergency();
                ctrl.CurrentIncident = incident;
            }
        }

        private void ResolveAllAgents()
        {
            if (agentManager == null) return;

            foreach (AgentController ctrl in agentManager.GetControllers())
            {
                if (ctrl != null)
                {
                    ctrl.ResolveEmergency();
                    ctrl.CurrentIncident = null;
                }
            }
        }

        private void SetDoorsInLocation(string locationName, bool locked)
        {
            Location loc = LocationManager.GetLocation(locationName);

            Collider locationCollider = loc.GetComponent<Collider>();
            if (locationCollider == null) return;

            Door door = loc.Door;
            if (door != null)
                door.ActivateObstacle(locked);

        }
    }
}