using UnityEngine;

public class EnemyIndexer : MonoBehaviour
{
    [Header("索引状态")]
    public bool isIndexed = false; // 【成功被索引】=true，【未被索引】=false

    private Vector3 lastPosition;

    private void Start()
    {
        GridManager.Instance.Add(this);
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (transform.position != lastPosition)
        {
            GridManager.Instance.UpdatePosition(this, lastPosition);
            lastPosition = transform.position;
        }
    }

    private void OnDestroy()
    {
        GridManager.Instance.Remove(this);
    }

    // 可视化Enemy状态（Scene视图）
    private void OnDrawGizmos()
    {
        Gizmos.color = isIndexed ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.up * 1f, 0.5f); // 球体表示状态
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isIndexed ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f); // 选中放大
        UnityEditor.Handles.Label(transform.position + Vector3.up * 3f, isIndexed ? "已索引" : "未索引");
    }

    // 公共方法供Player调用
    public void SetIndexed(bool state)
    {
        isIndexed = state;
    }
}