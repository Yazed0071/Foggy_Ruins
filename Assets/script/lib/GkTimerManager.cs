using System;
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
        public Action OnFinished;
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

                Action callback = timer.OnFinished;
                if (callback != null)
                {
                    try
                    {
                        callback.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("GkEventTimerManager callback error on timer '" + name + "': " + ex);
                    }
                }

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
        Start(name, duration, null);
    }

    public static void StartRaw(string name, float duration)
    {
        StartRaw(name, duration, null);
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

    public static void Start(string name, float duration, Action onFinished)
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
        timer.OnFinished = onFinished;

        _timers[name] = timer;
    }

    public static void StartRaw(string name, float duration, Action onFinished)
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
        timer.IsRaw = true;
        timer.OnFinished = onFinished;

        _timers[name] = timer;
    }

    public static void SetOnFinished(string name, Action onFinished)
    {
        EnsureInstance();

        TimerData timer;
        if (_timers.TryGetValue(name, out timer))
        {
            timer.OnFinished = onFinished;
        }
    }

    public static float GetRemaining(string name)
    {
        EnsureInstance();

        TimerData timer;
        if (_timers.TryGetValue(name, out timer))
            return timer.Remaining;

        return 0f;
    }

    public static bool Exists(string name)
    {
        EnsureInstance();
        return _timers.ContainsKey(name);
    }

    public static void ClearAll()
    {
        EnsureInstance();
        _timers.Clear();
    }
}

//Example usage
/*
// Scaled timer with callback

Start a timer for 10 seconds and log a message when it finishes
GkEventTimerManager.Start("CountDown_Target02_Monologue", 10f, () =>
{
    Debug.Log("Do this when CountDown_Target02_Monologue finishes.");
});

// Unscaled timer with callback (Runs normally even if Time.timeScale is 0)
GkEventTimerManager.StartRaw("CountDown_Target02_Monologue", 10f, () =>
{
    Debug.Log("Raw timer finished.");
});

GkEventTimerManager.Start("CountDown_Target02_Monologue", 10f);
GkEventTimerManager.SetPaused("Go_Execute", false);
GkEventTimerManager.Stop("CountDown_Target02_Monologue");
bool active = GkEventTimerManager.IsTimerActive("CountDown_Target02_Monologue");
*/