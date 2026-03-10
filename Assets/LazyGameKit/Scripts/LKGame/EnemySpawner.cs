using UnityEngine;
using System.Collections;

using LazyGameKit.Core;
using LazyGameKit.Base.Pool;
using LazyGameKit.Base.Grid;
using LazyGameKit.Base.SpawnBounds;

namespace LazyGameKit.Game
{

    public class EnemySpawner : MonoBehaviour
    {
        [Header("Generation Setting")]
        public GameObject enemyPrefab;
        public int enemyCount = 1000;
        public bool usePooling = true;

        [Header("Pooling Setting")]
        [SerializeField] private int poolInitialSize = 100;
        [SerializeField] private int poolMaxSize = 200000;

        private ObjectPool<EnemyPoolable> pool;

        [Header("RectBounds Setting")]
        [SerializeField] private RectBounds rectBounds;

        private void Start()
        {
            if (enemyPrefab == null)
            {
                Debug.LogError("[EnemySpawner] enemyPrefab 未赋值！");
                return;
            }

            enemyPrefab.tag = "Enemy";

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

        private void OnDestroy()
        {
            if (pool != null) pool.Clear();
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

                Vector3 pos = rectBounds.GetValidPosition();
                var pooled = pool.Get();
                // pooled.gameObject.hideFlags = HideFlags.HideInHierarchy;

                pooled.transform.position = pos;
                pooled.OnSpawned(pos);

                if (TryGetComponent<EnemyIndexer>(out var indexer)) {
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

                Vector3 pos = rectBounds.GetValidPosition();
                GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
                // enemy.hideFlags = HideFlags.HideInHierarchy;

                if (enemy.GetComponent<EnemyIndexer>() == null)
                    enemy.AddComponent<EnemyIndexer>();
            }

            Debug.Log($"[EnemySpawner] 普通 Instantiate 生成 {enemyCount} 个敌人完成");
        }
    }

}