using UnityEngine;

namespace Projeto1IA
{
    public class LocationProvider : ILocationProvider
    {
        public Vector3 GetRandomPointInLocationType(LocationType _type) =>
            LocationManager.GetRandomPointInLocationType(_type);

        public Vector3 GetRandomPointInLocation(string _name) =>
            LocationManager.GetRandomPointInLocation(_name);

        public Location FindNearest(Vector3 _pos, LocationType _type) =>
            LocationManager.FindNearestLocation(_pos, _type);
    }
}
