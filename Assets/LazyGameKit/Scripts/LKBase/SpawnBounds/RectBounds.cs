using UnityEngine;

using LazyGameKit.Core;

namespace LazyGameKit.Base.SpawnBounds
{
    class RectBounds : MonoBehaviour, ISpawnPositionProvider
    {
        [SerializeField] private Vector2 spawnArea = new(200f, 200f);
        [SerializeField] private float minDistanceBetweenEnemies = 1.5f;

        private Bounds bounds;

        private void Start()
        {
            bounds = new Bounds(transform.position, new Vector3(spawnArea.x, spawnArea.y, 0.1f));
        }

        public Vector3 GetValidPosition()
        {
            Vector3 pos;
            int attempts = 0;
            const int maxAttempts = 30;

            do
            {
                pos = bounds.RandomPointInBounds() + transform.position;
                pos.z = 0f;

                attempts++;

                if (attempts > maxAttempts)
                {
                    Debug.LogWarning("[RectBounds] 位置生成尝试超过上限，可能敌人密度过高，使用最后一个位置");
                    break;
                }
            }
            while (Physics.CheckSphere(pos, minDistanceBetweenEnemies, LayerMask.GetMask("Default")));

            return pos;
        }

        #region Debug - Gizmos

        private void OnDrawGizmos()
        {
            DrawSpawnAreaGizmo(false);
        }

        private void OnDrawGizmosSelected()
        {
            DrawSpawnAreaGizmo(true);
        }

        private void DrawSpawnAreaGizmo(bool selected)
        {
            if (!enabled) return;

            Vector3 center = transform.position;
            Vector3 size = new(spawnArea.x, spawnArea.y, 0.02f);

            Gizmos.matrix = Matrix4x4.identity;

            // area line
            Gizmos.color = selected
                ? new Color(0.8f, 0.2f, 0.2f, 0.9f)
                : new Color(0.8f, 0.2f, 0.2f, 0.35f);
            Gizmos.DrawWireCube(center, size);

            // center cube
            Gizmos.color = selected
                ? new Color(1f, 1f, 0.2f, 0.7f)
                : new Color(1f, 1f, 0.2f, 0.4f);
            Gizmos.DrawCube(center, Vector3.one * 0.4f);
        }

        #endregion
    }
}