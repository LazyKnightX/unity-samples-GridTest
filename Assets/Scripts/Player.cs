using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("索引半径")]
    [SerializeField] private float radius = 20f;
    public float Radius => radius;

    [Header("更新频率")]
    [SerializeField] private float updateInterval = 0.1f; // 每0.1s更新一次，优化性能
    private float lastUpdateTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // 动态调整radius：键盘 +/- 或鼠标滚轮
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f) radius += scroll * 10f;
        radius = Mathf.Clamp(radius, 1f, 100f);

        if (Input.GetKeyDown(KeyCode.Equals)) radius += 5f; // =
        if (Input.GetKeyDown(KeyCode.Minus)) radius -= 5f;  // -

        // 批量更新状态
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateEnemyStates();
            lastUpdateTime = Time.time;
        }
    }

    public void UpdateEnemyStates()
    {
        // 1. 重置所有为未索引（高效，纯赋值）
        foreach (var ei in GridManager.Instance.AllEnemies)
        {
            ei.SetIndexed(false);
        }

        // 2. 查询附近，标记为已索引
        List<EnemyIndexer> nearby = GridManager.Instance.QueryNearby(transform.position, radius);
        foreach (var ei in nearby)
        {
            ei.SetIndexed(true);
        }

        // Debug: 日志附近数量
        Debug.Log($"Player附近已索引Enemy: {nearby.Count} (radius={radius:F1})");
    }
}