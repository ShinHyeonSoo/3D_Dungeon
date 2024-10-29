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

    private void Start()
    {
        var monster = Resources.Load<GameObject>("Prefabs/Monster");
        Instantiate(monster);
    }

    public void InitMapBake()
    {
        var temp = Resources.Load<GameObject>("Prefabs/Maps");
        _map = Instantiate(temp);

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
