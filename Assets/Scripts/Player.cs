using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("索引半径")]
    [SerializeField] private float radius = 20f;
    public float Radius => radius;

    [Header("更新频率")]
    [SerializeField] private float updateInterval = 0.1f;
    private float lastUpdateTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        bool radiusChanged = false;

        if (Mathf.Abs(scroll) > 0.001f)
        {
            radius += scroll * 10f;
            radiusChanged = true;
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            radius += 5f;
            radiusChanged = true;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            radius -= 5f;
            radiusChanged = true;
        }

        if (radiusChanged)
        {
            radius = Mathf.Clamp(radius, 1f, 100f);
        }

        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateEnemyStates();
            lastUpdateTime = Time.time;
        }
    }

    public void UpdateEnemyStates()
    {
        foreach (var ei in GridManager.Instance.AllEnemies)
        {
            ei.SetIndexed(false);
        }

        List<EnemyIndexer> nearby = GridManager.Instance.QueryNearby(transform.position, radius);
        foreach (var ei in nearby)
        {
            ei.SetIndexed(true);
        }

        Debug.Log($"Player附近已索引Enemy: {nearby.Count} (radius={radius:F1})");
    }
}