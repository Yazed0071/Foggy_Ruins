using System;
using UnityEngine;

public class ControlLogic : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] LayerMask layerMask;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 50, Color.red,10f);
            
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 1000f, layerMask);
            
            if (hit.collider != null)
            {
                //Debug.Log("Clicked: " + hit.collider.gameObject.name);
                //you put what you want to do with the enemy/collided object here as so hit.collider.gameObject----
                //like so
                Destroy(hit.collider.gameObject);
            }
            else
            {
                //Debug.Log("Miss");
            }
        }
        
    }
}
