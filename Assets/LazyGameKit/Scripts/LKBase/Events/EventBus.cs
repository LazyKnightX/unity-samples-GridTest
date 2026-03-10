using UnityEngine;

using LazyGameKit.Base.Pool;
using LazyGameKit.Base.Grid;

static class EventBus
{
    public static EnemyPoolable OnSpawnEnemy()
    {
        // vars
        var pool = PoolManager.Instance.GetEnemyPool();

        // create enemy
        GameObject go = Object.Instantiate(enemyPrefab);

        // pool state
        var poolable = go.GetComponent<EnemyPoolable>();
        if (poolable == null) poolable = go.AddComponent<EnemyPoolable>();
        poolable.SetOwningPool(pool);

        // attach indexer
        if (go.GetComponent<EnemyIndexer>() == null)
            go.AddComponent<EnemyIndexer>();

        // initial active state
        go.SetActive(false);

        return poolable;
    }
}
