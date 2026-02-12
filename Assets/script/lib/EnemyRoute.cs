using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Put on route parent object.
/// Reads child RouteNode components in hierarchy order.
/// RouteName auto-syncs from GameObject name.
/// </summary>
[ExecuteAlways]
public class EnemyRoute : MonoBehaviour
{
    [Header("Auto from object name")]
    [SerializeField] private string routeName;

    [Tooltip("If true, last node links back to first node")]
    public bool loop = false;

    [Header("2D Gizmo")]
    public bool drawGizmos = true;
    [Min(0.01f)] public float nodeRadius = 0.12f;

    private readonly List<RouteNode> _nodes = new List<RouteNode>();
    public IReadOnlyList<RouteNode> Nodes => _nodes;
    public string RouteName => routeName;

    private void Awake()
    {
        routeName = gameObject.name;
        RefreshNodesFromChildren();
    }

    private void OnEnable()
    {
        routeName = gameObject.name;
        RefreshNodesFromChildren();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
            routeName = gameObject.name;

        RefreshNodesFromChildren();
    }
#endif

    public void RefreshNodesFromChildren()
    {
        _nodes.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            RouteNode node = child.GetComponent<RouteNode>();
            if (node != null)
                _nodes.Add(node);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        DrawRouteGizmos(selectedOnly: false);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
        DrawRouteGizmos(selectedOnly: true);
    }

    private void DrawRouteGizmos(bool selectedOnly)
    {
        RefreshNodesFromChildren();

        // Always draw route center marker (debug)
        Gizmos.color = selectedOnly ? Color.magenta : new Color(1f, 0f, 1f, 0.45f);
        {
            Vector3 c = transform.position;
            c.z = 0f;
            Gizmos.DrawWireSphere(c, nodeRadius * 0.75f);
        }

        if (_nodes.Count == 0) return;

        // Draw node spheres
        Gizmos.color = selectedOnly ? Color.yellow : new Color(1f, 1f, 0f, 0.7f);
        for (int i = 0; i < _nodes.Count; i++)
        {
            if (_nodes[i] == null) continue;

            Vector3 p = _nodes[i].transform.position;
            p.z = 0f;
            Gizmos.DrawSphere(p, nodeRadius);

#if UNITY_EDITOR
            Handles.Label(p + Vector3.up * (nodeRadius * 1.8f), $"{i}:{_nodes[i].name}");
#endif
        }

        // Draw links i -> i+1
        Gizmos.color = selectedOnly ? Color.cyan : new Color(0f, 1f, 1f, 0.7f);
        for (int i = 0; i < _nodes.Count - 1; i++)
        {
            if (_nodes[i] == null || _nodes[i + 1] == null) continue;

            Vector3 a = _nodes[i].transform.position; a.z = 0f;
            Vector3 b = _nodes[i + 1].transform.position; b.z = 0f;
            Gizmos.DrawLine(a, b);
        }

        // Loop link last -> first
        if (loop && _nodes.Count > 1 && _nodes[0] != null && _nodes[_nodes.Count - 1] != null)
        {
            Vector3 a = _nodes[_nodes.Count - 1].transform.position; a.z = 0f;
            Vector3 b = _nodes[0].transform.position; b.z = 0f;
            Gizmos.DrawLine(a, b);
        }
    }

    [ContextMenu("Snap Nodes Z To 0 (2D)")]
    private void SnapNodesZToZero()
    {
        RefreshNodesFromChildren();
        foreach (var n in _nodes)
        {
            if (n == null) continue;
            Vector3 p = n.transform.position;
            p.z = 0f;
            n.transform.position = p;
        }
    }

    [ContextMenu("Log Node Positions")]
    private void LogNodePositions()
    {
        RefreshNodesFromChildren();
        for (int i = 0; i < _nodes.Count; i++)
        {
            if (_nodes[i] == null) continue;
            Debug.Log($"Route '{routeName}' node[{i}] '{_nodes[i].name}' pos={_nodes[i].transform.position}");
        }
    }
}