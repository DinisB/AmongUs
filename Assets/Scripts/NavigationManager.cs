using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Projeto1IA
{
    /// <summary>
    /// Central registry for navigation areas
    /// </summary>
    public class NavigationManager : MonoBehaviour
    {
        private static Dictionary<string, NavigationArea> areas = new Dictionary<string, NavigationArea>();
        private static NavigationManager instance;

        private void Awake()
        {
            RegisterAllAreas();

            if (instance == null)
            {
                instance = this;
            }
        }

        private void RegisterAllAreas()
        {
            NavigationArea[] foundAreas = FindObjectsByType<NavigationArea>(FindObjectsSortMode.None);
            foreach (NavigationArea area in foundAreas)
            {
                if (!areas.ContainsKey(area.AreaName))
                {
                    areas.Add(area.AreaName, area);
                }
                else
                {
                    Debug.LogWarning($"Duplicate area name: {area.AreaName}");
                }
            }
        }

        /// <summary>
        /// Get a random point inside a named area if its not full
        /// </summary>
        /// <param name="areaName">Name of the area</param>
        /// <returns>Random point or nothing if full</returns>
        public static Vector3 GetRandomPointInArea(string areaName)
        {
            if (areas.ContainsKey(areaName))
            {
                NavigationArea area = areas[areaName];
                if (!area.IsFull)
                {
                    return area.GetRandomPointInArea();
                }
            }
            return Vector3.zero;
        }

        public static NavigationArea GetArea(string areaName)
        {
            if (areas.ContainsKey(areaName))
            {
                return areas[areaName];
            }
            return null;
        }

        public static NavigationArea[] GetAreasByType(AreaType areaType)
        {
            return areas.Values.Where(area => area.AreaType == areaType).ToArray();
        }

        /// <summary>
        /// Find the nearest area of a given type to a position
        /// </summary>
        /// <param name="position">Reference position</param>
        /// <param name="areaType">Type of area to find</param>
        /// <returns>Nearest area or null</returns>
        public static NavigationArea FindNearestArea(Vector3 position, AreaType areaType)
        {
            NavigationArea[] areasOfType = GetAreasByType(areaType);
            NavigationArea nearest = null;
            float minDistance = float.MaxValue;

            foreach (NavigationArea area in areasOfType)
            {
                float distance = Vector3.Distance(position, area.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = area;
                }
            }

            return nearest;
        }

        public static NavigationArea FindNearestArea(Vector3 position, NavigationArea[] searchAreas)
        {
            NavigationArea nearest = null;
            float minDistance = float.MaxValue;

            foreach (NavigationArea area in searchAreas)
            {
                float distance = Vector3.Distance(position, area.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = area;
                }
            }

            return nearest;
        }

        public static string[] GetAllAreaNames()
        {
            return areas.Keys.ToArray();
        }

        public static void AgentEnteringArea(string areaName)
        {
            if (areas.ContainsKey(areaName))
            {
                areas[areaName].AgentEntered();
            }
        }

        public static void AgentExitingArea(string areaName)
        {
            if (areas.ContainsKey(areaName))
            {
                areas[areaName].AgentExited();
            }
        }
    }
}