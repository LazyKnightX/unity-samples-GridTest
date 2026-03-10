using UnityEngine;

using LazyGameKit.Base.Grid;

namespace LazyGameKit.Base.Pool
{
    /// <summary>
    /// use to mark Enemy Object is owning by ObjectPool.
    /// could provide extra actions for OnSpawned/OnDespawned.
    /// </summary>
    public class EnemyPoolable : MonoBehaviour
    {
        private ObjectPool<EnemyPoolable> owningPool;

        public void SetOwningPool(ObjectPool<EnemyPoolable> pool)
        {
            owningPool = pool;
        }

        public void OnSpawned(Vector3 position)
        {
            transform.position = position;
            transform.rotation = Quaternion.identity;
            gameObject.SetActive(true);

            ExtraAction_OnSpawned();
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);

            ExtraAction_OnDespawned();
        }

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

        void ExtraAction_OnSpawned()
        {
            // reset index state
            if (TryGetComponent<EnemyIndexer>(out var indexer))
            {
                indexer.isIndexed = false;
            }
        }

        void ExtraAction_OnDespawned()
        {
        }
    }
}
