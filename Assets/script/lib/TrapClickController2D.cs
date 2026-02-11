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


        Collider2D hit = Physics2D.OverlapPoint(p, clickableMask);
        if (hit == null) return;


        if (hit.TryGetComponent<IClickable>(out var c))
            c.OnClicked();
        else
        {
            c = hit.GetComponentInParent<IClickable>();
            if (c != null) c.OnClicked();
        }
    }
}