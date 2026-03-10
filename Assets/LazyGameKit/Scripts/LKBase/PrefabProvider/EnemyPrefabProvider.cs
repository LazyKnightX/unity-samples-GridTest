using UnityEngine;
using System.Collections.Generic;

class EnemyPrefabProvider : MonoBehaviour, IPrefabProvider
{
    private static EnemyPrefabProvider _Instance;
    public static EnemyPrefabProvider Instance
    {
        get
        {
            return _Instance;
        }
    }

    [SerializeField] private List<GameObject> prefabs = new();

    void Awake()
    {
        _Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameObject Get()
    {
        return prefabs[Random.Range(0, prefabs.Count - 1)];
    }
}
