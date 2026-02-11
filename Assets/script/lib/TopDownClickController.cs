using UnityEngine;

public class TopDownClickController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask clickableMask;


    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryHandleClick2D(Input.mousePosition);
    }


    private void TryHandleClick2D(Vector3 mouseScreenPos)
    {
        Vector3 world = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        Vector2 point = new Vector2(world.x, world.y);


        Collider2D hit = Physics2D.OverlapPoint(point, clickableMask);
        if (hit == null) return;


        if (hit.TryGetComponent<IClickable>(out var clickable))
            clickable.OnClicked();
        else
        {
            clickable = hit.GetComponentInParent<IClickable>();
            if (clickable != null) clickable.OnClicked();
        }
    }
}