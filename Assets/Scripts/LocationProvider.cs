using UnityEngine;
using System.Linq;

namespace Projeto1IA
{
    /// <summary>
    /// Provides random points using LocationManager
    /// </summary>
    public class LocationProvider : ILocationProvider
    {
        /// <summary>
        /// Get a random point inside an available location of the given type
        /// </summary>
        /// <param name="_type">Type of location to search</param>
        /// <returns>Random point in location</returns>
        public Vector3 GetRandomPointInLocationType(LocationType _type)
        {
            Location loc;

            if (TryGetAvailableLocation(_type, out loc))
            {
                return loc.GetRandomPointInLocation();
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Get a random point inside a specific named location
        /// </summary>
        /// <param name="_name">Name of the location</param>
        /// <returns>Random point in location</returns>
        public Vector3 GetRandomPointInLocation(string _name) =>
            LocationManager.GetRandomPointInLocation(_name);

        /// <summary>
        /// Find the nearest location of a given type to a position
        /// </summary>
        /// <param name="_pos">Reference position</param>
        /// <param name="_type">Type of location to find</param>
        /// <returns>Nearest location or null if none found</returns>
        public Location FindNearest(Vector3 _pos, LocationType _type) =>
            LocationManager.FindNearestLocation(_pos, _type);

        /// <summary>
        /// Try to get an available (not full) location of a given type
        /// </summary>
        /// <param name="type">Type of location to search</param>
        /// <param name="location">Location if found</param>
        /// <returns>True if found</returns>
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