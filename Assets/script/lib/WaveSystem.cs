using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    private int _pulseCount = 0;

    [Header("Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn")]
    [SerializeField] private Vector3 spawnPosition = new(-31.48f, -4.81f, 1f);

    private readonly List<string> routes = new()
    {
        "rt_North_0", "rt_North_1", "rt_North_2",
        "rt_South_0", "rt_South_1", "rt_South_2"
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
            SpawnEnemyFromPrefab();
            StartSpawnTimer();
        });
    }

    private void OnDisable()
    {
        GkEventTimerManager.Stop("Spawn_Enemy");
    }

    private void SpawnEnemyFromPrefab()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("enemyPrefab is not assigned in Inspector.");
            return;
        }


        string enemyName = $"Enemy_{_pulseCount}";


        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
        enemy.name = enemyName;


        var eAI = enemy.GetComponent<EnemyAI>();
        var eRoute = enemy.GetComponent<EnemyRouteMover>();


        if (eAI == null || eRoute == null)
        {
            Debug.LogError("Prefab must contain EnemyAI and EnemyRouteMover.");
            return;
        }


        eAI.SetHealth(5);
        eRoute.SetEnemyName(enemyName);



        if (routes.Count > 0)
        {
            string pickedRoute = routes[Random.Range(0, routes.Count)];
            EnemyAI.SetRoute(enemyName, pickedRoute);
        }
    }
}