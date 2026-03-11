using UnityEngine;
using LazyGameKit.Base.Pool;
using LazyGameKit.Base.Grid;
using LazyGameKit.Base.PrefabProvider;

namespace LazyGameKit.Base.Factory
{
    public class PooledEnemyFactory : IEnemyFactory
    {
        public static PooledEnemyFactory Instance
        {
            get
            {
                _Instance ??= new PooledEnemyFactory();
                return _Instance;
            }
        }
        private static PooledEnemyFactory _Instance;

        private readonly ObjectPool<EnemyPoolable> _pool;

        public int ActiveCount => _pool.CountActive;

        private IPrefabProvider _enemyPrefabProvider;

        private PooledEnemyFactory()
        {
            _pool = PoolManager.Instance.GetEnemyPool();
            _enemyPrefabProvider = EnemyPrefabProvider.Instance;
        }

        public EnemyPoolable CreatePooled()
        {
            // create enemy
            var go = Object.Instantiate(_enemyPrefabProvider.Get());

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
            pooled.gameObject.SetActive(false);

            return pooled;
        }

        public void Release(EnemyPoolable enemy)
        {
            enemy.gameObject.SetActive(false);
            _pool.Release(enemy);
        }
    }
}