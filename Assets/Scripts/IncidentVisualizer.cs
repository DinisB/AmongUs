using UnityEngine;
using System.Collections.Generic;

namespace Projeto1IA
{
    /// <summary>
    /// Changes the color of location renderers based on active incidents
    /// </summary>
    public class IncidentVisualizer : MonoBehaviour
    {
        [SerializeField] private Color fireColor = Color.red;
        [SerializeField] private Color oxygenColor = Color.blue;
        [SerializeField] private Color electricColor = Color.yellow;
        [SerializeField] private Color defaultColor = Color.white;

        private IDictionary<Location, Renderer> locationRenderers = new Dictionary<Location, Renderer>();
        private IDictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

        private void Start()
        {
            AutoPopulateLocationRenderers();

            if (IncidentManager.Instance != null)
            {
                IncidentManager.Instance.OnIncidentTriggered += OnIncidentChanged;
                IncidentManager.Instance.OnIncidentSpread += OnIncidentChanged;
                IncidentManager.Instance.OnIncidentResolved += OnIncidentChanged;
            }
        }

        private void AutoPopulateLocationRenderers()
        {
            locationRenderers.Clear();
            originalColors.Clear();

            Location[] allLocations = FindObjectsByType<Location>(FindObjectsSortMode.None);

            foreach (Location loc in allLocations)
            {
                if (loc == null) continue;

                Renderer rend = loc.GetComponent<Renderer>();
                if (rend == null)
                {
                    rend = loc.GetComponentInChildren<Renderer>();
                }

                if (rend != null)
                {
                    locationRenderers[loc] = rend;
                    originalColors[rend] = rend.material.color;
                }
            }
        }

        private void OnIncidentChanged(Incident incident)
        {
            RefreshLocationHighlights();
        }

        private void Update()
        {
            RefreshLocationHighlights();
        }


        /// <summary>
        /// Refreshes color of locations
        /// </summary>
        private void RefreshLocationHighlights()
        {
            if (IncidentManager.Instance == null) return;

            foreach (KeyValuePair<Location, Renderer> kvp in locationRenderers)
            {
                Location loc = kvp.Key;
                Renderer rend = kvp.Value;
                if (rend == null || loc == null) continue;

                Color targetColor = originalColors.ContainsKey(rend) ? originalColors[rend] : defaultColor;

                foreach (Incident inc in IncidentManager.Instance.GetActiveIncidents())
                {
                    if (inc.IsLocationAffected(loc.LocationName))
                    {
                        targetColor = inc.Type switch
                        {
                            IncidentType.Fire => fireColor,
                            IncidentType.OxygenLeak => oxygenColor,
                            IncidentType.ElectricalFailure => electricColor,
                            _ => targetColor
                        };
                        break;
                    }
                }

                rend.material.color = targetColor;
            }
        }

        private void OnDestroy()
        {
            if (IncidentManager.Instance != null)
            {
                IncidentManager.Instance.OnIncidentTriggered -= OnIncidentChanged;
                IncidentManager.Instance.OnIncidentSpread -= OnIncidentChanged;
                IncidentManager.Instance.OnIncidentResolved -= OnIncidentChanged;
            }
        }
    }
}