using UnityEngine;

namespace LazyGameKit.Base.SpawnBounds
{
    public interface ISpawnPositionProvider
    {
        Vector3 GetValidPosition();
    }
}