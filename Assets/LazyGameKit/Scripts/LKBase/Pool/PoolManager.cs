using UnityEngine;
using System;

using LazyGameKit.Core;

namespace LazyGameKit.Base.Pool
{
    class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get { return _Instance; } }
        private static PoolManager _Instance;

        private ObjectPool<EnemyPoolable> enemyPool;
        public ObjectPool<EnemyPoolable> GetEnemyPool() => enemyPool;

        void Awake()
        {
            _Instance = this;
        }

        void Start()
        {
            enemyPool = Allocate(maxSize: 200000, initialSize: 100, createFunc: EventBus.OnSpawnEnemy);
        }

        public ObjectPool<EnemyPoolable> Allocate(int maxSize, int initialSize, Func<EnemyPoolable> createFunc)
        {
            var pool = new ObjectPool<EnemyPoolable>(
                createFunc: createFunc,
                onGet: poolable => poolable.OnSpawned(Vector3.zero),
                onRelease: poolable => poolable.OnDespawned(),
                onDestroyPoolObject: poolable => Destroy(poolable.gameObject),
                collectionCheck: true,
                defaultCapacity: initialSize,
                maxSize: maxSize
            );

            Debug.Log($"[EnemySpawner] 对象池初始化完成，容量：{initialSize} ~ {maxSize}");

            return pool;
        }
    }
}
