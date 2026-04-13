using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Projeto1IA
{
    public class LocationManager : MonoBehaviour
    {
        private static IDictionary<string, Location> _locations = new Dictionary<string, Location>();
        [SerializeField] private LayerMask _agentLayer;

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
    }
}
