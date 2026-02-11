using UnityEngine;
using UnityEngine.EventSystems;

public class TrapClickController2D : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask clickableMask = ~0; // Everything by default


    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }


    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;


        Vector3 w = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 p = new Vector2(w.x, w.y);


        Collider2D[] hits = Physics2D.OverlapPointAll(p, clickableMask);
        if (hits == null || hits.Length == 0) return;
        
        
        Collider2D best = hits[0];
        int bestOrder = int.MinValue;

        for (int i = 0; i < hits.Length; i++)
        {
            var sr = hits[i].GetComponentInParent<SpriteRenderer>();
            int order = sr != null ? sr.sortingOrder : 0;

            if (order > bestOrder)
            {
                bestOrder = order;
                best = hits[i];
            }
        }

        var clickable = best.GetComponentInParent<IClickable>();
        clickable?.OnClicked();
    }
}