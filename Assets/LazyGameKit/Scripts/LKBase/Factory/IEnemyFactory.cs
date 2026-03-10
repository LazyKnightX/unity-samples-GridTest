using UnityEngine;
using LazyGameKit.Base.Pool;

namespace LazyGameKit.Base.Factory
{
    public interface IEnemyFactory
    {
        EnemyPoolable Create(Vector3 position);
        void Release(EnemyPoolable enemy);
        int ActiveCount { get; }
    }
}
