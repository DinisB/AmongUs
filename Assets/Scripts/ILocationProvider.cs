using UnityEngine;

namespace Projeto1IA
{
    public interface ILocationProvider
    {
        Vector3 GetRandomPointInLocationType(LocationType _type);
        Vector3 GetRandomPointInLocation(string _name);
        Location FindNearest(Vector3 _pos, LocationType _type);
        bool TryGetAvailableLocation(LocationType type, out Location location);
    }
}
