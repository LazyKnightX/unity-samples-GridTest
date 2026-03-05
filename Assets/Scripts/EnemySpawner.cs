using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("生成参数")]
    public GameObject enemyPrefab;
    public int enemyCount = 1000;
    public bool usePooling = true;
    public float minDistanceBetweenEnemies = 1.5f;
    public Vector2 spawnArea = new Vector2(200f, 200f);

    [Header("Pooling 设置")]
    [SerializeField] private int poolInitialSize = 100;
    [SerializeField] private int poolMaxSize = 200000;

    private ObjectPool<EnemyPoolable> pool;
    private Bounds spawnBounds;

    private void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("[EnemySpawner] enemyPrefab 未赋值！");
            return;
        }

        enemyPrefab.tag = "Enemy";

        spawnBounds = new Bounds(Vector3.zero, new Vector3(spawnArea.x, spawnArea.y, 0.1f));

        if (usePooling)
        {
            InitializePool();
            StartCoroutine(SpawnWithPooling());
        }
        else
        {
            StartCoroutine(SpawnWithoutPooling());
        }
    }

    private void InitializePool()
    {
        pool = new ObjectPool<EnemyPoolable>(
            createFunc: () =>
            {
                GameObject go = Instantiate(enemyPrefab);
                var poolable = go.GetComponent<EnemyPoolable>();
                if (poolable == null) poolable = go.AddComponent<EnemyPoolable>();
                poolable.SetOwningPool(pool);
                if (go.GetComponent<EnemyIndexer>() == null)
                    go.AddComponent<EnemyIndexer>();
                go.SetActive(false);
                return poolable;
            },
            onGet: poolable => poolable.OnSpawned(Vector3.zero),
            onRelease: poolable => poolable.OnDespawned(),
            onDestroyPoolObject: poolable => Destroy(poolable.gameObject),
            collectionCheck: true,
            defaultCapacity: poolInitialSize,
            maxSize: poolMaxSize
        );

        Debug.Log($"[EnemySpawner] 对象池初始化完成，容量：{poolInitialSize} ~ {poolMaxSize}");
    }

    private IEnumerator SpawnWithPooling()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            if (i % 2000 == 0 && i > 0) yield return null;

            Vector3 pos = GetValidSpawnPosition();
            var pooled = pool.Get();
            pooled.transform.position = pos;
            pooled.OnSpawned(pos);

            var indexer = pooled.GetComponent<EnemyIndexer>();
            if (indexer != null)
            {
                GridManager.Instance.Add(indexer);
            }
        }

        Debug.Log($"[EnemySpawner] 使用对象池生成 {enemyCount} 个敌人完成");
    }

    private IEnumerator SpawnWithoutPooling()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            if (i % 1000 == 0 && i > 0) yield return null;

            Vector3 pos = GetValidSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

            if (enemy.GetComponent<EnemyIndexer>() == null)
                enemy.AddComponent<EnemyIndexer>();
        }

        Debug.Log($"[EnemySpawner] 普通 Instantiate 生成 {enemyCount} 个敌人完成");
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 pos;
        int attempts = 0;
        const int maxAttempts = 30;

        do
        {
            pos = spawnBounds.RandomPointInBounds();
            pos.z = 0f;

            attempts++;
            if (attempts > maxAttempts)
            {
                Debug.LogWarning("[EnemySpawner] 位置生成尝试超过上限，可能敌人密度过高，使用最后一个位置");
                break;
            }
        }
        while (Physics.CheckSphere(pos, minDistanceBetweenEnemies, LayerMask.GetMask("Default")));

        return pos;
    }

    private void OnDestroy()
    {
        if (pool != null) pool.Clear();
    }
}