using UnityEngine;


/// <summary>
/// For each child node under a route.
/// speed = move speed toward this node
/// delay = wait before moving toward this node
/// </summary>
public class RouteNode : MonoBehaviour
{
    [Header("Per-node settings")]
    [Min(0f)] public float speed = 2.5f;
    [Min(0f)] public float delay = 0f;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 p = transform.position;
        p.z = 0f; // 2D
        Gizmos.DrawSphere(p, 0.08f);
    }
}