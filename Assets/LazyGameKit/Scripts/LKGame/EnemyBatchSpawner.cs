using UnityEngine;
using System.Collections;

using LazyGameKit.Base.Pool;
using LazyGameKit.Base.SpawnBounds;

namespace LazyGameKit.Game
{
    public class EnemyBatchSpawner : MonoBehaviour
    {
        [Header("Generation Setting")]
        public int enemyCount = 1000;

        [Header("RectBounds Setting")]
        [SerializeField] private RectBounds rectBounds;

        private ObjectPool<EnemyPoolable> enemyPool;

        private void Start()
        {
            enemyPool = PoolManager.Instance.GetEnemyPool();
            StartCoroutine(RequestBatchSpawn(enemyCount, 1000));
        }

        private void SpawnSingle()
        {
            Vector3 pos = rectBounds.GetValidPosition();
            var pooled = enemyPool.Get();

            pooled.transform.position = pos;
            pooled.OnSpawned(pos);
        }

        private IEnumerator RequestBatchSpawn(int enemyCount, int restGap)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                if (i % restGap == 0 && i > 0) yield return null;
                SpawnSingle();
            }

            Debug.Log($"[EnemyBatchSpawner] 生成 {enemyCount} 个敌人完成");
        }
    }

}