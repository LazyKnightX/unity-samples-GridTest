using UnityEngine;

using LazyGameKit.Base.Pool;
using LazyGameKit.Base.Factory;

static class EventBus
{
    public static EnemyPoolable OnSpawnEnemy()
    {
        return PooledEnemyFactory.Instance.CreatePooled();
    }
}
