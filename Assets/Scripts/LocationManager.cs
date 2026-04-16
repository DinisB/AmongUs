using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Projeto1IA
{
    public class LocationManager : MonoBehaviour
    {
        private static IDictionary<string, Location> _locations = new Dictionary<string, Location>();
        [SerializeField] private LayerMask _agentLayer;

        private static Dictionary<string, List<string>> _adjacency = new Dictionary<string, List<string>>();

        public static void RegisterLocation(Location _location)
        {
            if (!_locations.ContainsKey(_location.LocationName))
                _locations.Add(_location.LocationName, _location);
            else
                Debug.LogWarning($"Duplicate location name: {_location.LocationName}");
        }

        public static void UnregisterLocation(Location _location)
        {
            _locations.Remove(_location.LocationName);
        }

        public static Location GetRandomLocation() => _locations.Values.ElementAt(Random.Range(0, _locations.Count));

        public static Vector3 GetRandomPointInLocation(string _locationName)
        {
            if (_locations.TryGetValue(_locationName, out Location _loc) && !_loc.IsFull)
                return _loc.GetRandomPointInLocation();
            return Vector3.zero;
        }

        public static Vector3 GetRandomPointInLocationType(LocationType _locationType)
        {
            Location[] _ofType = GetLocationsByType(_locationType);
            Location[] _available = _ofType.Where(_l => !_l.IsFull).ToArray();
            if (_available.Length == 0) return Vector3.zero;
            return _available[Random.Range(0, _available.Length)].GetRandomPointInLocation();
        }

        public static Location GetLocation(string _locationName)
        {
            _locations.TryGetValue(_locationName, out Location _loc);
            return _loc;
        }

        public static Location[] GetLocationsByType(LocationType _locationType)
        {
            return _locations.Values.Where(_l => _l.LocationType == _locationType).ToArray();
        }

        public static Location FindNearestLocation(Vector3 _position, LocationType _locationType)
        {
            return GetLocationsByType(_locationType)
                .OrderBy(_l => Vector3.Distance(_position, _l.transform.position))
                .FirstOrDefault();
        }

        public static string[] GetAllLocationNames() => _locations.Keys.ToArray();

        public LayerMask AgentLayer => _agentLayer;

        public static void AddAdjacency(string a, string b)
        {
            if (!_adjacency.ContainsKey(a)) _adjacency[a] = new List<string>();
            if (!_adjacency.ContainsKey(b)) _adjacency[b] = new List<string>();
            if (!_adjacency[a].Contains(b)) _adjacency[a].Add(b);
            if (!_adjacency[b].Contains(a)) _adjacency[b].Add(a);
        }

        public static Location[] GetAdjacentLocations(Location location)
        {
            if (_adjacency.TryGetValue(location.LocationName, out List<string> names))
            {
                return names
                    .Select(n => GetLocation(n))
                    .Where(l => l != null)
                    .ToArray();
            }

            return _locations.Values
                .Where(l => l != location &&
                            Vector3.Distance(l.transform.position, location.transform.position) < 20f)
                .ToArray();
        }
    }
}
