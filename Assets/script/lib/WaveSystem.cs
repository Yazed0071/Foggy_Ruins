using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    private int _pulseCount;
    private WaveSystem enemyCount;

    private List<string> routes = new List<string>
    {
        "rt_North_0",
        "rt_North_1",
        "rt_North_2",
        "rt_South_0",
        "rt_South_1",
        "rt_South_2"
    };


    private void Start()
    {
        StartSpawnTimer();
    }


    private void StartSpawnTimer()
    {
        GkEventTimerManager.Start("Spawn_Enemy", 3f, () =>
        {
            _pulseCount++;
            Debug.Log("Spawn_Enemy #" + _pulseCount);
            StartSpawnTimer();
        });
    }

    private void OnDisable()
    {
        GkEventTimerManager.Stop("Spawn_Enemy");
    }

    private void SpawnEnemy()
    {
        GameObject go = new GameObject(string.Format("Enemy_{0}", _pulseCount));
    }
}
