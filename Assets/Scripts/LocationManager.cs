using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Projeto1IA
{
    /// <summary>
    /// Central registry for all locations, providing queries and adjacency management
    /// </summary>
    public class LocationManager : MonoBehaviour
    {
        private static IDictionary<string, Location> _locations = new Dictionary<string, Location>();
        [SerializeField] private LayerMask _agentLayer;

        private static IDictionary<string, IList<string>> _adjacency = new Dictionary<string, IList<string>>();

        /// <summary>
        /// Register a location
        /// </summary>
        /// <param name="_location">Location to register</param>
        public static void RegisterLocation(Location _location)
        {
            if (!_locations.ContainsKey(_location.LocationName))
                _locations.Add(_location.LocationName, _location);
            else
                Debug.LogWarning($"Duplicate location name: {_location.LocationName}");
        }

        /// <summary>
        /// Remove a location from the registry
        /// </summary>
        /// <param name="_location">Location to unregister</param>
        public static void UnregisterLocation(Location _location)
        {
            _locations.Remove(_location.LocationName);
        }

        public static Location GetRandomLocation() => _locations.Values.ElementAt(Random.Range(0, _locations.Count));

        /// <summary>
        /// Get a random point inside a named location if the location isn't full
        /// </summary>
        /// <param name="_locationName">Name of the target location</param>
        /// <returns>Random point</returns>
        public static Vector3 GetRandomPointInLocation(string _locationName)
        {
            if (_locations.TryGetValue(_locationName, out Location _loc) && !_loc.IsFull)
                return _loc.GetRandomPointInLocation();
            return Vector3.zero;
        }

        /// <summary>
        /// Get a random point inside any location of a given type that isn't full
        /// </summary>
        /// <param name="_locationType">Type of location to search</param>
        /// <returns>Random point</returns>
        public static Vector3 GetRandomPointInLocationType(LocationType _locationType)
        {
            Location[] _ofType = GetLocationsByType(_locationType);
            Location[] _available = _ofType.Where(_l => !_l.IsFull).ToArray();
            if (_available.Length == 0) return Vector3.zero;
            return _available[Random.Range(0, _available.Length)].GetRandomPointInLocation();
        }

        /// <summary>
        /// Get a location by its name
        /// </summary>
        /// <param name="_locationName">Name of the location</param>
        /// <returns>Location instance or null</returns>
        public static Location GetLocation(string _locationName)
        {
            _locations.TryGetValue(_locationName, out Location _loc);
            return _loc;
        }

        /// <summary>
        /// Get all locations of a specific type
        /// </summary>
        /// <param name="_locationType">Type to filter by</param>
        /// <returns>Array of matching locations</returns>
        public static Location[] GetLocationsByType(LocationType _locationType)
        {
            return _locations.Values.Where(_l => _l.LocationType == _locationType).ToArray();
        }

        /// <summary>
        /// Find the location of a given type closest to the position
        /// </summary>
        /// <param name="_position">Reference position</param>
        /// <param name="_locationType">Type of location to search</param>
        /// <returns>Nearest location or null</returns>
        public static Location FindNearestLocation(Vector3 _position, LocationType _locationType)
        {
            return GetLocationsByType(_locationType)
                .OrderBy(_l => Vector3.Distance(_position, _l.transform.position))
                .FirstOrDefault();
        }

        public static string[] GetAllLocationNames() => _locations.Keys.ToArray();

        public LayerMask AgentLayer => _agentLayer;

        /// <summary>
        /// Register that two locations are adjacent
        /// </summary>
        /// <param name="a">First location name</param>
        /// <param name="b">Second location name</param>
        public static void AddAdjacency(string a, string b)
        {
            if (!_adjacency.ContainsKey(a)) _adjacency[a] = new List<string>();
            if (!_adjacency.ContainsKey(b)) _adjacency[b] = new List<string>();
            if (!_adjacency[a].Contains(b)) _adjacency[a].Add(b);
            if (!_adjacency[b].Contains(a)) _adjacency[b].Add(a);
        }

        /// <summary>
        /// Get locations adjacent to the given location, using explicit adjacency or distance fallback
        /// </summary>
        /// <param name="location">Reference location</param>
        /// <returns>Array of adjacent locations</returns>
        public static Location[] GetAdjacentLocations(Location location)
        {
            if (_adjacency.TryGetValue(location.LocationName, out IList<string> names))
            {
                return names
                    .Select(n => GetLocation(n))
                    .Where(l => l != null)
                    .ToArray();
            }

            // Fallback: locations within n units
            return _locations.Values
                .Where(l => l != location &&
                            Vector3.Distance(l.transform.position, location.transform.position) < 20f)
                .ToArray();
        }
    }
}