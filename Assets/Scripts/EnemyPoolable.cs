using UnityEngine;

public class EnemyPoolable : MonoBehaviour
{
    // 这个组件主要用于标记对象属于池管理
    // 可以在这里添加额外的池相关逻辑（如重置状态）

    private ObjectPool<EnemyPoolable> owningPool;

    public void SetOwningPool(ObjectPool<EnemyPoolable> pool)
    {
        owningPool = pool;
    }

    // 当对象被“使用”时调用（可选：重置位置、血量、动画等）
    public void OnSpawned(Vector3 position)
    {
        transform.position = position;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);

        // 示例：重置组件状态
        var indexer = GetComponent<EnemyIndexer>();
        if (indexer != null)
        {
            indexer.isIndexed = false;
            // 如果有其他状态，也在这里重置
        }
    }

    // 当对象被回收时调用（可选）
    public void OnDespawned()
    {
        gameObject.SetActive(false);
        // 可在这里清理引用、粒子等
    }

    // 供外部调用回收（通常由 EnemySpawner 或其他管理器调用）
    public void ReturnToPool()
    {
        if (owningPool != null)
        {
            owningPool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}