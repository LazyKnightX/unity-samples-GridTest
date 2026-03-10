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
                StartCoroutine(SpawnWithPooling());
            }
            else
            {
                StartCoroutine(SpawnWithoutPooling());
            }
        }

        private IEnumerator SpawnWithPooling()
        {
            var pool = PoolManager.Instance.GetEnemyPool();

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