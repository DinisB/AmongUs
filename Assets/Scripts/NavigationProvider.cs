using UnityEngine;

namespace Projeto1IA
{
    public class NavigationProvider : INavigationProvider
    {
        public NavigationArea[] GetAreasByType(AreaType _type) =>
            NavigationManager.GetAreasByType(_type);

        public NavigationArea FindNearest(Vector3 _pos, AreaType _type) =>
            NavigationManager.FindNearestArea(_pos, _type);
    }
}
