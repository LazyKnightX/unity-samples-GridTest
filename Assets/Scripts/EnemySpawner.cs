using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("生成参数")]
    [SerializeField] public GameObject enemyPrefab; // Enemy Prefab（需挂 EnemyIndexer）
    [SerializeField] public int enemyCount = 1000; // 生成数量
    [SerializeField] public bool usePooling = false; // 启用对象池（高级）
    [SerializeField] public float minDistance = 1f; // 最小间距（防重叠）
    [SerializeField] public Vector2 spawnArea = new Vector2(200f, 200f); // 生成区域（覆盖 Grid worldSize）

    private ObjectPool<EnemyPoolable> pool; // Pooling 支持
    private Bounds spawnBounds;

    private void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyPrefab 未赋值！");
            return;
        }

        // 强制 Prefab Tag
        enemyPrefab.tag = "Enemy";

        spawnBounds = new Bounds(Vector3.zero, new Vector3(spawnArea.x, 100f, spawnArea.y));
        if (usePooling)
        {
            SetupPooling();
            SpawnAllWithPooling();
        }
        else
        {
            StartCoroutine(SpawnAllCoroutine()); // 分批防卡
        }

        Debug.Log($"EnemySpawner: 生成 {enemyCount} 个 Enemy 完成！");
    }

    // 分批生成协程（每帧 1000 个）
    private IEnumerator SpawnAllCoroutine()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            if (i % 1000 == 0) yield return null; // 每 1000 让一帧

            Vector3 pos = GetRandomPosition();
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            // 动态加 EnemyIndexer（若 Prefab 未挂）
            if (enemy.GetComponent<EnemyIndexer>() == null)
                enemy.AddComponent<EnemyIndexer>();
        }
    }

    // 随机位置（bounds 内，Y=0）
    private Vector3 GetRandomPosition()
    {
        Vector3 pos;
        do
        {
            pos = spawnBounds.RandomPointInBounds(); // Unity 扩展方法，或手动
            pos.y = 0f;
        } while (minDistance > 0 && IsPositionOccupied(pos));
        return pos;
    }

    // 检查重叠（简单版，性能代价）
    private bool IsPositionOccupied(Vector3 pos)
    {
        Collider[] overlaps = Physics.OverlapSphere(pos, minDistance / 2f, LayerMask.GetMask("Enemy")); // 设 Enemy Layer
        return overlaps.Length > 0;
    }

    // Object Pooling 高级版
    private void SetupPooling()
    {
        pool = new ObjectPool<EnemyPoolable>(CreatePooledEnemy, OnTakeFromPool, OnReturnToPool, OnDestroyPooledEnemy, true, 10, 100);
    }

    private void SpawnAllWithPooling()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            EnemyPoolable pooled = pool.Get();
            pooled.transform.position = GetRandomPosition();
            pooled.gameObject.SetActive(true);
        }
    }

    // Pooling 辅助类 & 方法（需单独脚本 EnemyPoolable.cs）
    private EnemyPoolable CreatePooledEnemy()
    {
        GameObject go = Instantiate(enemyPrefab);
        go.SetActive(false);
        EnemyPoolable ep = go.AddComponent<EnemyPoolable>();
        return ep;
    }
    // ... (详见下方 EnemyPoolable)
}