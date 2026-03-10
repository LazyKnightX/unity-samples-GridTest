using UnityEngine;
using LazyGameKit.Base.Pool;
using LazyGameKit.Base.Grid;

namespace LazyGameKit.Base.Factory
{
    public class PooledEnemyFactory : IEnemyFactory
    {
        public static PooledEnemyFactory Instance
        {
            get
            {
                _Instance ??= new PooledEnemyFactory(prefab);
                return _Instance;
            }
        }
        private static PooledEnemyFactory _Instance;

        private readonly ObjectPool<EnemyPoolable> _pool;
        private readonly GameObject _prefab;  // 敌人预制体

        public int ActiveCount => _pool.CountActive;

        public PooledEnemyFactory(GameObject prefab)
        {
            _prefab = prefab;
            _pool = PoolManager.Instance.GetEnemyPool();
        }

        public EnemyPoolable CreatePooled()
        {
            // create enemy
            var go = Object.Instantiate(_prefab);

            // pool state
            var poolable = go.GetComponent<EnemyPoolable>();
            if (poolable == null) poolable = go.AddComponent<EnemyPoolable>();
            poolable.SetOwningPool(_pool);

            // attach indexer
            if (go.GetComponent<EnemyIndexer>() == null)
                go.AddComponent<EnemyIndexer>();

            // initial active state
            go.SetActive(false);

            return poolable;
        }

        public EnemyPoolable Create(Vector3 position)
        {
            var pooled = _pool.Get();
            pooled.transform.position = position;
            pooled.gameObject.SetActive(true);

            return pooled;
        }

        public void Release(EnemyPoolable enemy)
        {
            enemy.gameObject.SetActive(false);
            _pool.Release(enemy);
        }
    }
}