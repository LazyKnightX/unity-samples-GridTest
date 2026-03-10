using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

using LazyGameKit.Core;
using LazyGameKit.Game;
using LazyGameKit.Base.Grid;
using LazyGameKit.Base.Pool;

public class TestGridPerformance : MonoBehaviour
{
    [Header("测试参数")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int[] scales = {1000, 10000, 100000};
    [SerializeField] private float queryRadius = 20f;
    [SerializeField] private int queryCount = 100;
    [SerializeField] private int stateUpdateCount = 100;
    [SerializeField] private bool useSpawnerPooling = true;

    private void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyPrefab 未赋值！测试中止。");
            return;
        }

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

        StartCoroutine(RunAllTests());
    }

    private IEnumerator RunAllTests()
    {
        foreach (int n in scales)
        {
            yield return StartCoroutine(TestScale(n));
            yield return new WaitForSeconds(1f);
        }
        Debug.Log("所有规模测试完成！");
    }

    private IEnumerator TestScale(int n)
    {
        Debug.Log($"开始测试规模: {n}");

        CleanupEnemies();

        GameObject spawnerGO = new GameObject("EnemySpawner");
        EnemySpawner spawner = spawnerGO.AddComponent<EnemySpawner>();
        spawner.enemyPrefab = enemyPrefab;
        spawner.enemyCount = n;
        // spawner.spawnArea = new Vector2(200f, 200f); // TODO
        spawner.usePooling = useSpawnerPooling;

        Stopwatch swInsert = new Stopwatch();
        swInsert.Start();

        yield return new WaitUntil(() => GridManager.Instance.AllEnemies.Count >= n);

        long insertTime = swInsert.ElapsedMilliseconds;
        Debug.Log($"规模 {n}: 生成+索引时间: {insertTime} ms");

        Stopwatch swQuery = new Stopwatch();
        swQuery.Start();
        for (int i = 0; i < queryCount; i++)
        {
            Vector3 queryPos = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), 0);
            GridManager.Instance.QueryNearby(queryPos, queryRadius);
        }
        long queryTime = swQuery.ElapsedMilliseconds / queryCount;
        Debug.Log($"规模 {n}: 平均查询时间: {queryTime} ms");

        Player player = FindObjectOfType<Player>();
        Stopwatch swState = new Stopwatch();
        swState.Start();
        for (int i = 0; i < stateUpdateCount; i++)
        {
            player.UpdateEnemyStates();
        }
        long stateTime = swState.ElapsedMilliseconds / stateUpdateCount;
        Debug.Log($"规模 {n}: 平均状态更新时间: {stateTime} ms");

        int nearbyCount = GridManager.Instance.QueryNearby(Vector3.zero, queryRadius).Count;
        Debug.Log($"规模 {n}: Player 中心附近 Enemy 数: {nearbyCount}");

        Destroy(spawnerGO);
        CleanupEnemies();

        yield return null;
    }

    private void CleanupEnemies()
    {
        var allEnemies = GridManager.Instance.AllEnemies;
        for (int i = allEnemies.Count - 1; i >= 0; i--)
        {
            var ei = allEnemies[i];
            if (ei == null) continue;

            var poolable = ei.GetComponent<EnemyPoolable>();
            if (poolable != null)
            {
                poolable.ReturnToPool();
            }
            else
            {
                Destroy(ei.gameObject);
            }
        }
        GridManager.Instance.AllEnemies.Clear();
        GridManager.Instance.grid.Clear();
    }
}