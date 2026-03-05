using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TestGridPerformance : MonoBehaviour
{
    [Header("测试参数")]
    [SerializeField] private GameObject enemyPrefab; // Enemy Prefab（需挂 EnemyIndexer）
    [SerializeField] private int[] scales = {1000, 10000, 100000}; // 测试规模
    [SerializeField] private float queryRadius = 20f; // 查询半径
    [SerializeField] private int queryCount = 100; // 查询次数
    [SerializeField] private int stateUpdateCount = 100; // 状态更新次数
    [SerializeField] private bool useSpawnerPooling = true; // 使用 Spawner Pooling

    private void Start()
    {
        if (enemyPrefab == null)
        {
            UnityEngine.Debug.LogError("EnemyPrefab 未赋值！测试中止。");
            return;
        }

        // 确保 GridManager 和 Player 存在
        if (GridManager.Instance == null)
        {
            GameObject gridGO = new GameObject("GridManager");
            gridGO.AddComponent<GridManager>();
        }
        if (Player.Instance == null)
        {
            GameObject playerGO = new GameObject("Player");
            playerGO.AddComponent<Player>();
            playerGO.transform.position = Vector3.zero;
        }

        // 运行测试
        StartCoroutine(RunAllTests());
    }

    private IEnumerator RunAllTests()
    {
        foreach (int n in scales)
        {
            yield return StartCoroutine(TestScale(n));
            yield return new WaitForSeconds(1f); // 间隔防过热
        }
        UnityEngine.Debug.Log("所有规模测试完成！");
    }

    private IEnumerator TestScale(int n)
    {
        UnityEngine.Debug.Log($"开始测试规模: {n}");

        // 清理旧 Enemy 和 Spawner
        CleanupEnemies();

        // 创建 Spawner
        GameObject spawnerGO = new GameObject("EnemySpawner");
        EnemySpawner spawner = spawnerGO.AddComponent<EnemySpawner>();
        spawner.enemyPrefab = enemyPrefab;
        spawner.enemyCount = n;
        spawner.spawnArea = new Vector2(200f, 200f); // 匹配 Grid worldSize
        spawner.usePooling = useSpawnerPooling;

        // 等待生成完成（监听 AllEnemies 计数）
        yield return new WaitUntil(() => GridManager.Instance.AllEnemies.Count >= n);

        long insertTime = 0; // Spawner 已处理插入，估算为生成时间（实际插入在 EnemyIndexer Start）

        // 查询测试
        Stopwatch swQuery = new Stopwatch();
        swQuery.Start();
        for (int i = 0; i < queryCount; i++)
        {
            Vector3 queryPos = new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f));
            GridManager.Instance.QueryNearby(queryPos, queryRadius);
        }
        long queryTime = swQuery.ElapsedMilliseconds / queryCount;
        UnityEngine.Debug.Log($"规模 {n}: 平均查询时间: {queryTime} ms");

        // 状态更新测试
        Player player = FindObjectOfType<Player>();
        Stopwatch swState = new Stopwatch();
        swState.Start();
        for (int i = 0; i < stateUpdateCount; i++)
        {
            player.UpdateEnemyStates();
        }
        long stateTime = swState.ElapsedMilliseconds / stateUpdateCount;
        UnityEngine.Debug.Log($"规模 {n}: 平均状态更新时间: {stateTime} ms");

        // 额外分析：附近 Enemy 数统计
        int nearbyCount = GridManager.Instance.QueryNearby(Vector3.zero, queryRadius).Count;
        UnityEngine.Debug.Log($"规模 {n}: Player 中心附近 Enemy 数: {nearbyCount} (预期 ~ (pi*r^2 / area) * n)");

        // 清理
        Destroy(spawnerGO);
        CleanupEnemies();

        yield return null;
    }

    private void CleanupEnemies()
    {
        foreach (var ei in new List<EnemyIndexer>(GridManager.Instance.AllEnemies))
        {
            if (ei != null) Destroy(ei.gameObject);
        }
        GridManager.Instance.AllEnemies.Clear();
        GridManager.Instance.grid.Clear();
    }
}
