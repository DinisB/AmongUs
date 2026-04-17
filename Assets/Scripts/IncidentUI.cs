using UnityEngine;
using TMPro;

namespace Projeto1IA
{
    /// <summary>
    /// Displays killed and alive counters
    /// </summary>
    public class IncidentUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text killedCounterText;
        [SerializeField] private TMP_Text aliveCounterText;
        [SerializeField] private GameObject evacuationBanner;

        private int killedCount = 0;

        private void Start()
        {
            if (IncidentManager.Instance == null) return;

            IncidentManager.Instance.OnAgentKilled += OnAgentKilled;
            IncidentManager.Instance.OnEvacuationTriggered += OnEvacuation;

            if (evacuationBanner != null)
                evacuationBanner.SetActive(false);

            RefreshCounters();
        }

        private void Update()
        {
            RefreshCounters();
        }

        private void OnAgentKilled(AgentController ctrl)
        {
            killedCount++;
            RefreshCounters();
        }

        private void OnEvacuation()
        {
            if (evacuationBanner != null)
                evacuationBanner.SetActive(true);
        }

        private void RefreshCounters()
        {
            if (killedCounterText != null)
                killedCounterText.text = "Eliminated: " + killedCount;

            RefreshAliveCounter();
        }

        private void RefreshAliveCounter()
        {
            if (aliveCounterText == null || AgentManager.Instance == null) return;

            int alive = AgentManager.Instance.GetControllers().Count;
            aliveCounterText.text = "Alive: " + alive;
        }

        private void OnDestroy()
        {
            if (IncidentManager.Instance != null)
            {
                IncidentManager.Instance.OnAgentKilled -= OnAgentKilled;
                IncidentManager.Instance.OnEvacuationTriggered -= OnEvacuation;
            }
        }
    }
}