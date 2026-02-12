using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSystem : MonoBehaviour
{
    public static WaveSystem Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private VictoryMenu victoryMenu;   // assign in Inspector OR auto-find
    [SerializeField] private TMP_Text enemyRemainingText;

    [Header("Wave")]
    [SerializeField] private int totalEnemies = 50;

    [Header("Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn")]
    [SerializeField] private Vector3 spawnPosition = new(-31.48f, -4.81f, 1f);

    private int spawnedCount = 0;
    private int remainingEnemy;
    private bool hasWon = false;

    private readonly List<string> routes = new()
    {
        "rt_North_0", "rt_North_1", "rt_North_2",
        "rt_South_0", "rt_South_1", "rt_South_2"
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (victoryMenu == null)
            victoryMenu = FindFirstObjectByType<VictoryMenu>();
    }

    private void Start()
    {
        remainingEnemy = totalEnemies;
        UpdateUI();
        StartSpawnTimer();
    }

    private void StartSpawnTimer()
    {
        GkEventTimerManager.Start("Spawn_Enemy", 3f, () =>
        {
            if (spawnedCount < totalEnemies && !hasWon)
            {
                spawnedCount++;
                Debug.Log("Spawn_Enemy #" + spawnedCount);

                SpawnEnemyFromPrefab();
                StartSpawnTimer(); // continue chain
            }
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

        string enemyName = $"Enemy_{spawnedCount}";
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
        enemy.name = enemyName;

        var eAI = enemy.GetComponent<EnemyAI>();
        var eRoute = enemy.GetComponent<EnemyRouteMover>();

        if (eAI == null || eRoute == null)
        {
            Debug.LogError("Prefab must contain EnemyAI and EnemyRouteMover.");
            Destroy(enemy);
            return;
        }

        eRoute.SetEnemyName(enemyName);

        if (routes.Count > 0)
        {
            string pickedRoute = routes[Random.Range(0, routes.Count)];
            EnemyAI.SetRoute(enemyName, pickedRoute);
        }
    }

    public void DecreaseEnemyCount()
    {
        if (hasWon) return;

        remainingEnemy = Mathf.Max(remainingEnemy - 1, 0);
        UpdateUI();

        if (remainingEnemy == 0)
            ShowVictoryScreen();
    }

    public int GetRemainingEnemyCount() => remainingEnemy;

    private void ShowVictoryScreen()
    {
        if (hasWon) return;
        hasWon = true;

        Debug.Log("Victory! All enemies defeated.");
        GkEventTimerManager.Stop("Spawn_Enemy");

        if (victoryMenu != null)
            victoryMenu.Show(true);
        else
            Debug.LogWarning("VictoryMenu reference is missing.");
    }

    private void UpdateUI()
    {
        if (enemyRemainingText != null)
            enemyRemainingText.text = "Remaining enemies: " + remainingEnemy;
    }
}