using System.Collections.Generic;
using UnityEngine;

public class AddPoint : MonoBehaviour
{
    public List<GameObject> points = new List<GameObject>();
    [ContextMenu("Add Point")]
    void AddOnePoint()
    {
        var p = new GameObject($"Point_{transform.childCount}");
        p.transform.SetParent(transform);
        p.transform.position = transform.position;
        points.Add(p);
        
    }
}
