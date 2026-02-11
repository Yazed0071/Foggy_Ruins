using System.Collections;
using UnityEngine;

public class DamTrapKiller : TrapKiller
{
    [Header("Water movement")]
    [SerializeField] private Transform killZoneTransform;   // drag the KillZone child here
    [SerializeField] private float waterDistance = 0.5f;     // how far down
    [SerializeField] private float waterTime = 0.15f;        // how fast it drops
    [SerializeField] private float resetDelay = 0.05f;      // tiny pause before reset

    private Vector3 _killZoneStartLocalPos;

    private void Awake()
    {
        if (killZoneTransform != null)
            _killZoneStartLocalPos = killZoneTransform.localPosition;
    }
    

    protected override IEnumerator ActivateRoutine()
    {
        isOnCooldown = true;
        isActive = true;

        if (animator != null)
            animator.SetTrigger(activateTrigger);

        Debug.Log("Trap ACTIVE");

        yield return StartCoroutine(WaterKillZoneRoutine());

        yield return new WaitForSeconds(activeDuration);
        isActive = false;
        Debug.Log("Trap INACTIVE");

        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }

    

    private IEnumerator WaterKillZoneRoutine()
    {
        if (killZoneTransform == null) yield break;

        Vector3 start = _killZoneStartLocalPos;
        Vector3 end = start + Vector3.down * waterDistance;

        float t = 0f;
        while (t < waterTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / waterTime);
            killZoneTransform.localPosition = Vector3.Lerp(start, end, a);
            yield return null;
        }

        
        KillEnemiesCurrentlyInsideZone();
        
        if (frontKillZone != null) frontKillZone.enabled = false;

        yield return new WaitForSeconds(resetDelay);

        
        killZoneTransform.localPosition = _killZoneStartLocalPos;
        if (frontKillZone != null) frontKillZone.enabled = true;
    }

    


    
}