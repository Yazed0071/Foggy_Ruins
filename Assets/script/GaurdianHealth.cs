using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GaurdianHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [SerializeField] public float Health = 5; 
    [SerializeField] private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("Attacking?"))
        {
            Health--;
        }
        if (Health <= 0)
        {
            Debug.Log("Dead");
        }
    }
}
