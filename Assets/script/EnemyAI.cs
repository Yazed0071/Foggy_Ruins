using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static route API + per-enemy Kill() component.
/// Also shows current route in Inspector (read-only).
/// </summary>
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private int health;
    
    // ---------- Per-enemy runtime/inspector ----------
    [Header("Runtime (Read Only)")]
    [SerializeField] private string currentRoute = "None";

    [SerializeField] private float moveSpeed = 3f;

    public string CurrentRoute => string.IsNullOrWhiteSpace(currentRoute) ? "None" : currentRoute;

    /// <summary>
    /// Called by EnemyRouteMover whenever route changes.
    /// </summary>
    public void SetCurrentRouteDebug(string routeName)
    {
        currentRoute = string.IsNullOrWhiteSpace(routeName) ? "None" : routeName;
    }

    // ---------- Static registries ----------
    private static readonly Dictionary<string, EnemyRouteMover> EnemyByName = new Dictionary<string, EnemyRouteMover>();
    private static readonly Dictionary<string, EnemyRoute> RouteByName = new Dictionary<string, EnemyRoute>();

    public static void RebuildRegistry()
    {
        EnemyByName.Clear();
        RouteByName.Clear();

        EnemyRouteMover[] enemies = Object.FindObjectsByType<EnemyRouteMover>(FindObjectsSortMode.None);
        foreach (var e in enemies)
        {
            string nameKey = e.EnemyName;

            if (string.IsNullOrWhiteSpace(nameKey))
            {
                Debug.LogWarning($"EnemyRouteMover on '{e.name}' has empty EnemyName. Skipped.");
                continue;
            }

            if (!EnemyByName.TryAdd(nameKey, e))
                Debug.LogWarning($"Duplicate enemyName '{nameKey}'. Last one ignored.");
        }

        EnemyRoute[] routes = Object.FindObjectsByType<EnemyRoute>(FindObjectsSortMode.None);
        foreach (var r in routes)
        {
            r.RefreshNodesFromChildren();

            string routeKey = r.RouteName;
            if (string.IsNullOrWhiteSpace(routeKey))
            {
                Debug.LogWarning($"EnemyRoute on '{r.name}' has empty RouteName. Skipped.");
                continue;
            }

            if (!RouteByName.TryAdd(routeKey, r))
                Debug.LogWarning($"Duplicate routeName '{routeKey}'. Last one ignored.");
        }
    }

    /// <summary>
    /// Usage: EnemyAI.SetRoute("sol_enemy_0000", "rt_guard_0000");
    /// </summary>
    public static bool SetRoute(string enemyName, string routeName)
    {
        if (RouteByName.Count == 0)
            RebuildRegistry();

        if (!EnemyByName.TryGetValue(enemyName, out EnemyRouteMover enemy))
        {
            RebuildRegistry();

            if (!EnemyByName.TryGetValue(enemyName, out enemy))
            {
                Debug.LogError($"EnemyAI.SetRoute failed: enemy '{enemyName}' not found.");
                return false;
            }
        }

        if (!RouteByName.TryGetValue(routeName, out EnemyRoute route))
        {
            RebuildRegistry();

            if (!RouteByName.TryGetValue(routeName, out route))
            {
                Debug.LogError($"EnemyAI.SetRoute failed: route '{routeName}' not found.");
                return false;
            }
        }

        if (route.Nodes == null || route.Nodes.Count == 0)
        {
            route.RefreshNodesFromChildren();
            if (route.Nodes.Count == 0)
            {
                Debug.LogError($"EnemyAI.SetRoute failed: route '{routeName}' has no RouteNode children.");
                return false;
            }
        }

        enemy.AssignRoute(route);
        return true;
    }

    public static bool StopRoute(string enemyName)
    {
        if (!EnemyByName.TryGetValue(enemyName, out EnemyRouteMover enemy))
        {
            RebuildRegistry();
            if (!EnemyByName.TryGetValue(enemyName, out enemy))
            {
                Debug.LogError($"EnemyAI.StopRoute failed: enemy '{enemyName}' not found.");
                return false;
            }
        }
        
        enemy.StopRoute();
        return true;
    }

    public static bool PauseRoute(string enemyName, bool paused)
    {
        if (!EnemyByName.TryGetValue(enemyName, out EnemyRouteMover enemy))
        {
            RebuildRegistry();
            if (!EnemyByName.TryGetValue(enemyName, out enemy))
            {
                Debug.LogError($"EnemyAI.PauseRoute failed: enemy '{enemyName}' not found.");
                return false;
            }
        }

        enemy.SetPaused(paused);
        return true;
    }

    public static bool HasEnemy(string enemyName)
    {
        if (EnemyByName.Count == 0) RebuildRegistry();
        return EnemyByName.ContainsKey(enemyName);
    }

    public static bool HasRoute(string routeName)
    {
        if (RouteByName.Count == 0) RebuildRegistry();
        return RouteByName.ContainsKey(routeName);
    }

    public static void PrintRegistry()
    {
        RebuildRegistry();

        foreach (var e in EnemyByName.Keys)
            Debug.Log($"[EnemyAI] Enemy: {e}");

        foreach (var r in RouteByName.Keys)
            Debug.Log($"[EnemyAI] Route: {r}");
    }

    public void DealDamage(int damageAmount)
    {
        Debug.Log("Enemy Damaged: " + gameObject.name + "Damage ammount: " + damageAmount);
        health -= damageAmount;
        if (health <= 0)Destroy(gameObject);
        
    }

    public static void RegisterEnemy(EnemyRouteMover mover)
    {
        if (mover == null) return;
        string key = mover.EnemyName;
        if (string.IsNullOrWhiteSpace(key)) return;
        EnemyByName[key] = mover;
    }

    public static void UnregisterEnemy(EnemyRouteMover mover)
    {
        if (mover == null) return;
        string key = mover.EnemyName;
        if (string.IsNullOrWhiteSpace(key)) return;

        if (EnemyByName.TryGetValue(key, out var existing) && existing == mover)
            EnemyByName.Remove(key);
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }
}