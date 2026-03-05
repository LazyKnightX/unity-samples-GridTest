using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private float cellSize = 10f;
    [SerializeField] private Vector2 worldSize = new Vector2(200f, 200f);
    [SerializeField] private bool showGridGizmos = true; // 可视化开关
    [SerializeField] private float gizmoRange = 50f; // 只画Player附近范围

    public Dictionary<Vector2Int, List<EnemyIndexer>> grid = new Dictionary<Vector2Int, List<EnemyIndexer>>();
    private List<EnemyIndexer> allEnemies = new List<EnemyIndexer>(); // 所有Enemy引用，便于批量重置
    private Vector2 halfWorldSize;
    public List<EnemyIndexer> AllEnemies => allEnemies; // Player访问

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        halfWorldSize = worldSize / 2f;
    }

    public Vector2Int GetGridKey(Vector3 position)
    {
        int x = Mathf.FloorToInt((position.x + halfWorldSize.x) / cellSize);
        int z = Mathf.FloorToInt((position.z + halfWorldSize.y) / cellSize);
        return new Vector2Int(x, z);
    }

    public void Add(EnemyIndexer ei)
    {
        if (ei == null || ei.gameObject.tag != "Enemy") return;
        Vector2Int key = GetGridKey(ei.transform.position);
        if (!grid.ContainsKey(key)) grid[key] = new List<EnemyIndexer>();
        if (!grid[key].Contains(ei)) grid[key].Add(ei); // 防重复
        if (!allEnemies.Contains(ei)) allEnemies.Add(ei);
    }

    public void Remove(EnemyIndexer ei)
    {
        Vector2Int key = GetGridKey(ei.transform.position);
        if (grid.TryGetValue(key, out var list)) list.Remove(ei);
        allEnemies.Remove(ei);
        // 清理空List
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
        int range = Mathf.CeilToInt(radius / cellSize) + 1; // +1防边界

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                Vector2Int key = centerKey + new Vector2Int(x, z);
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

    // 可视化Grid
    private void OnDrawGizmos()
    {
        if (!showGridGizmos) return;
        Gizmos.color = Color.gray * 0.5f;

        // 只画Player附近（若有Player）
        Player player = FindObjectOfType<Player>();
        Vector3 center = player ? player.transform.position : Vector3.zero;
        float drawRange = gizmoRange;

        float startX = (center.x - drawRange + halfWorldSize.x) / cellSize;
        float endX = (center.x + drawRange + halfWorldSize.x) / cellSize;
        float startY = (center.y - drawRange + halfWorldSize.y) / cellSize;
        float endY = (center.y + drawRange + halfWorldSize.y) / cellSize;

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
                Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, cellSize, 10f));

                // Cell内Enemy数标签
                if (grid.TryGetValue(key, out var list) && list.Count > 0)
                {
                    Gizmos.color = Color.blue;
                    UnityEditor.Handles.Label(cellCenter + Vector3.up * 6f, list.Count.ToString());
                    Gizmos.color = Color.gray * 0.5f;
                }
            }
        }

        // Player radius球体
        if (player)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.transform.position, player.Radius);
        }
    }
}