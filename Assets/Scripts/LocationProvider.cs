using UnityEngine;
using System.Linq;

namespace Projeto1IA
{
    public class LocationProvider : ILocationProvider
    {
        public Vector3 GetRandomPointInLocationType(LocationType _type)
        {
            Location loc;

            if (TryGetAvailableLocation(_type, out loc))
            {
                return loc.GetRandomPointInLocation();
            }

            return Vector3.zero;
        }

        public Vector3 GetRandomPointInLocation(string _name) =>
            LocationManager.GetRandomPointInLocation(_name);

        public Location FindNearest(Vector3 _pos, LocationType _type) =>
            LocationManager.FindNearestLocation(_pos, _type);

        public bool TryGetAvailableLocation(LocationType type, out Location location)
        {
            Location[] locations = LocationManager.GetLocationsByType(type);

            System.Collections.Generic.IList<Location> available =
                locations.Where(l => l.CanEnter()).ToList();

            if (available.Count > 0)
            {
                location = available[Random.Range(0, available.Count)];
                return true;
            }

            location = null;
            return false;
        }

    }
}
