using System.Collections;
using UnityEngine;

public class TrapKiller : MonoBehaviour, IClickable
{
    [Header("Refs")]
    [SerializeField] private Collider2D frontKillZone;
    [SerializeField] private Animator animator;


    [Header("Timing")]
    [SerializeField] private float activeDuration = 1.0f;
    [SerializeField] private float cooldown = 1.0f;


    [Header("Filter")]
    [SerializeField] private LayerMask enemyMask;


    [Header("Animation")]
    [SerializeField] private string activateTrigger = "Activate";


    private bool isActive;
    private bool isOnCooldown;


    public void OnClicked()
    {
        if (isOnCooldown)
        {
            Debug.Log("Trap on cooldown");
            return;
        }
        StartCoroutine(ActivateRoutine());
    }


    private IEnumerator ActivateRoutine()
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


    private void KillEnemiesCurrentlyInsideZone()
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


    private void TryKill(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyMask) == 0) return;


        EnemyAI e = other.GetComponent<EnemyAI>();
        if (e == null) e = other.GetComponentInParent<EnemyAI>();


        if (e != null) e.Kill();
    }
}