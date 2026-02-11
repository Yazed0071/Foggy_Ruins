using System;
using UnityEngine;

public class RuinLogic : MonoBehaviour
{
    [SerializeField]private int health = 3;
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            health -= 1;
        }
    }


    void Update()
    {
        
    }
}
