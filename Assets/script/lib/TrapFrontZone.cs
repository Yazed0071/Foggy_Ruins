using UnityEngine;

public class TrapFrontZone : MonoBehaviour
{
    [SerializeField] private TrapKiller trap;

    private void Awake()
    {
        if (trap == null) trap = GetComponentInParent<TrapKiller>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        trap.OnEnemyEnteredZone(other);
    }
}