using System.Collections;
using UnityEngine;

public class TrapKiller : MonoBehaviour, IClickable
{
    [SerializeField] protected int damageAmount = 1;
    [Header("Refs")]
    [SerializeField] protected Collider2D frontKillZone;
    [SerializeField] protected Animator animator;


    [Header("Timing")]
    [SerializeField] protected float activeDuration = 1.0f;
    [SerializeField] protected float cooldown = 1.0f;


    [Header("Filter")]
    [SerializeField] protected LayerMask enemyMask;


    [Header("Animation")]
    [SerializeField] protected string activateTrigger = "Activate";


    protected bool isActive;
    protected bool isOnCooldown;


    [SerializeField] private FireSound fire;


    public void OnClicked()
    {
        Debug.Log("CLICKED: " + name + " (" + GetType().Name + ")");

        if (isOnCooldown)
        {
            Debug.Log("Trap on cooldown");
            return;
        }
        StartCoroutine(ActivateRoutine());
        fire.fireStart();
    }


    protected virtual IEnumerator ActivateRoutine()
    {
        isOnCooldown = true;
        isActive = true;


        if (animator != null)
            animator.SetTrigger(activateTrigger);


        Debug.Log("Trap ACTIVE");


        KillEnemiesCurrentlyInsideZone();


        yield return new WaitForSeconds(activeDuration);
        isActive = false;
        Debug.Log("Trap INACTIVE");


        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }


    public void OnEnemyEnteredZone(Collider2D other)
    {
        if (!isActive) return;
        TryKill(other);
    }


    protected void KillEnemiesCurrentlyInsideZone()
    {
        if (frontKillZone == null)
        {
            Debug.LogError("frontKillZone not assigned");
            return;
        }


        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.SetLayerMask(enemyMask);
        filter.useTriggers = true;


        Collider2D[] results = new Collider2D[32];
        int count = frontKillZone.Overlap(filter, results);


        Debug.Log("Enemies in zone at activate: " + count);


        for (int i = 0; i < count; i++)
            TryKill(results[i]);
    }


    protected void TryKill(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyMask) == 0) return;
        Debug.Log("TryKill: " + name + " (" + GetType().Name + ")");


        EnemyAI e = other.GetComponent<EnemyAI>();
        if (e == null) e = other.GetComponentInParent<EnemyAI>();


        if (e != null) e.DealDamage(damageAmount);
    }
}