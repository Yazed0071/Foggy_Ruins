using System.Collections.Generic;
using UnityEngine;

public sealed class GkEventTimerManager : MonoBehaviour
{
    private sealed class TimerData
    {
        public float Remaining;
        public bool IsPaused;
        public bool IsActive;
        public bool IsRaw;
    }

    private static GkEventTimerManager _instance;
    private static readonly Dictionary<string, TimerData> _timers = new Dictionary<string, TimerData>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoBootstrap()
    {
        EnsureInstance();
    }

    private static void EnsureInstance()
    {
        if (_instance != null)
            return;

        GameObject go = new GameObject("[GkEventTimerManager]");
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<GkEventTimerManager>();
    }

    private void Update()
    {
        Tick(Time.deltaTime, Time.unscaledDeltaTime);
    }

    private static void Tick(float scaledDeltaTime, float unscaledDeltaTime)
    {
        if (_timers.Count == 0)
            return;

        if (scaledDeltaTime < 0f)
            scaledDeltaTime = 0f;

        if (unscaledDeltaTime < 0f)
            unscaledDeltaTime = 0f;

        var keys = new List<string>(_timers.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            string name = keys[i];

            TimerData timer;
            if (!_timers.TryGetValue(name, out timer))
                continue;

            if (!timer.IsActive || timer.IsPaused)
                continue;

            float dt;
            if (timer.IsRaw)
                dt = unscaledDeltaTime;
            else
                dt = scaledDeltaTime;

            timer.Remaining -= dt;

            if (timer.Remaining <= 0f)
            {
                timer.Remaining = 0f;
                timer.IsActive = false;
                Debug.Log("Timer finished: " + name);
            }
        }
    }

    public static void SetPaused(string name, bool paused)
    {
        EnsureInstance();

        TimerData timer;
        if (_timers.TryGetValue(name, out timer))
        {
            timer.IsPaused = paused;
        }
    }

    public static void Stop(string name)
    {
        EnsureInstance();

        if (_timers.ContainsKey(name))
        {
            _timers.Remove(name);
        }
    }

    public static void Start(string name, float duration)
    {
        EnsureInstance();

        if (string.IsNullOrEmpty(name))
            return;

        if (duration <= 0f)
            return;

        TimerData timer = new TimerData();
        timer.Remaining = duration;
        timer.IsPaused = false;
        timer.IsActive = true;
        timer.IsRaw = false;

        _timers[name] = timer;
    }

    public static void StartRaw(string name, float duration)
    {
        EnsureInstance();

        if (string.IsNullOrEmpty(name))
            return;

        if (duration <= 0f)
            return;

        TimerData timer = new TimerData();
        timer.Remaining = duration;
        timer.IsPaused = false;
        timer.IsActive = true;
        timer.IsRaw = true; // unscaled time

        _timers[name] = timer;
    }

    public static bool IsTimerActive(string name)
    {
        EnsureInstance();

        TimerData timer;
        if (_timers.TryGetValue(name, out timer))
        {
            return timer.IsActive && !timer.IsPaused;
        }

        return false;
    }

    public static float GetRemaining(string name)
    {
        EnsureInstance();

        TimerData timer;
        if (_timers.TryGetValue(name, out timer))
            return timer.Remaining;

        return 0f;
    }

    public static bool IsFinishedAndClear(string name)
    {
        EnsureInstance();

        TimerData timer;
        if (_timers.TryGetValue(name, out timer))
        {
            if (!timer.IsActive && timer.Remaining <= 0f)
            {
                _timers.Remove(name);
                return true;
            }
        }

        return false;
    }
}



//Example usage
/*
using UnityEngine;

public class EnemyHeliFlow : MonoBehaviour
{
    private void Start()
    {
        GkEventTimerManager.Start("GetInEnemyHeliLimit", 10f);
    }

    private void Update()
    {
        // one-time trigger when finished
        if (GkEventTimerManager.IsFinishedAndClear("GetInEnemyHeliLimit"))
        {
            Debug.Log("GetInEnemyHeliLimit finished -> do this now");
            // DoThis();
        }
    }
}
*/
