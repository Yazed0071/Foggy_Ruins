using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    private int _pulseCount;


    private void Start()
    {
        StartSpawnTimer();
        EnemyAI.SetRoute("ENEMY_SAMPLE", "rt_spawn_0000");
    }


    private void StartSpawnTimer()
    {
        GkEventTimerManager.Start("Spawn_Enemy", 3f, () =>
        {
            _pulseCount++;
            Debug.Log("Spawn_Enemy #" + _pulseCount);
            StartSpawnTimer();
            //SPAWN ENEMY
        });
    }


    private void OnDisable()
    {
        GkEventTimerManager.Stop("Spawn_Enemy");
    }
}
