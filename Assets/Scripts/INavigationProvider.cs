using UnityEngine;

namespace Projeto1IA
{
    public interface INavigationProvider
    {
        NavigationArea[] GetAreasByType(AreaType _type);
        NavigationArea FindNearest(Vector3 _pos, AreaType _type);
    }
}
