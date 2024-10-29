using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    GameObject _map;
    NavMeshSurface[] _navSurfaces;

    protected override void Awake()
    {
        base.Awake();
    }

    public void InitMapBake()
    {
        _map = Resources.Load<GameObject>("Prefabs/Maps");
        Instantiate(_map);

        _navSurfaces = _map.GetComponentsInChildren<NavMeshSurface>();

        UpdateMapBake();
    }

    public void UpdateMapBake()
    {
        foreach (NavMeshSurface surface in _navSurfaces)
        {
            surface.BuildNavMesh();
        }
    }
}
