using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private float cellSize = 10f;
    [SerializeField] private Vector2 worldSize = new Vector2(200f, 200f);
    [SerializeField] private bool showGridGizmos = true;
    [SerializeField] private float gizmoRange = 50f;

    public Dictionary<Vector2Int, List<EnemyIndexer>> grid = new Dictionary<Vector2Int, List<EnemyIndexer>>();
    private List<EnemyIndexer> allEnemies = new List<EnemyIndexer>();
    private Vector2 halfWorldSize;
    public List<EnemyIndexer> AllEnemies => allEnemies;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        halfWorldSize = worldSize / 2f;
    }

    public Vector2Int GetGridKey(Vector3 position)
    {
        // 世界为 XY 平面，Z=0
        int x = Mathf.FloorToInt((position.x + halfWorldSize.x) / cellSize);
        int y = Mathf.FloorToInt((position.y + halfWorldSize.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public void Add(EnemyIndexer ei)
    {
        if (ei == null || ei.gameObject.tag != "Enemy") return;
        Vector2Int key = GetGridKey(ei.transform.position);
        if (!grid.ContainsKey(key)) grid[key] = new List<EnemyIndexer>();
        if (!grid[key].Contains(ei)) grid[key].Add(ei);
        if (!allEnemies.Contains(ei)) allEnemies.Add(ei);
    }

    public void Remove(EnemyIndexer ei)
    {
        Vector2Int key = GetGridKey(ei.transform.position);
        if (grid.TryGetValue(key, out var list)) list.Remove(ei);
        allEnemies.Remove(ei);
        if (list != null && list.Count == 0) grid.Remove(key);
    }

    public void UpdatePosition(EnemyIndexer ei, Vector3 oldPosition)
    {
        Vector2Int oldKey = GetGridKey(oldPosition);
        Vector2Int newKey = GetGridKey(ei.transform.position);
        if (oldKey != newKey)
        {
            if (grid.TryGetValue(oldKey, out var oldList)) oldList.Remove(ei);
            Add(ei);
        }
    }

    public List<EnemyIndexer> QueryNearby(Vector3 position, float radius)
    {
        List<EnemyIndexer> result = new List<EnemyIndexer>();
        Vector2Int centerKey = GetGridKey(position);
        int range = Mathf.CeilToInt(radius / cellSize) + 1;

        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector2Int key = centerKey + new Vector2Int(x, y);
                if (grid.TryGetValue(key, out var list))
                {
                    foreach (var ei in list)
                    {
                        if (Vector3.Distance(position, ei.transform.position) <= radius)
                            result.Add(ei);
                    }
                }
            }
        }
        return result;
    }

    private void OnDrawGizmos()
    {
        if (!showGridGizmos) return;
        Gizmos.color = Color.gray * 0.5f;

        Player player = FindObjectOfType<Player>();
        Vector3 center = player ? player.transform.position : Vector3.zero;
        float drawRange = gizmoRange;

        float startX = (center.x - drawRange + halfWorldSize.x) / cellSize;
        float endX   = (center.x + drawRange + halfWorldSize.x) / cellSize;
        float startY = (center.y - drawRange + halfWorldSize.y) / cellSize;
        float endY   = (center.y + drawRange + halfWorldSize.y) / cellSize;

        for (int x = Mathf.FloorToInt(startX); x <= Mathf.FloorToInt(endX); x++)
        {
            for (int y = Mathf.FloorToInt(startY); y <= Mathf.FloorToInt(endY); y++)
            {
                Vector2Int key = new Vector2Int(x, y);
                Vector3 cellCenter = new Vector3(
                    (x * cellSize) - halfWorldSize.x,
                    (y * cellSize) - halfWorldSize.y,
                    0
                );
                // 平面很薄，Z 方向只显示一点厚度
                Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, cellSize, 0.1f));

                if (grid.TryGetValue(key, out var list) && list.Count > 0)
                {
                    Gizmos.color = Color.blue;
                    UnityEditor.Handles.Label(cellCenter + Vector3.forward * 2f, list.Count.ToString());
                    Gizmos.color = Color.gray * 0.5f;
                }
            }
        }

        if (player)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.transform.position, player.Radius);
        }
    }
}