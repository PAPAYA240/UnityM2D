using System;
using UnityEngine;

public class Timer_Manager : MonoBehaviour
{
    public event Action OnTimeOver;
    public event Action OnTimeNext;

    public float CurrentTimer { get; private set; }
    public int Wave { get; private set; }
    public bool IsRunning { get; private set; }

    private int Timer = 30;

    public void Clear()
    {
        Wave = 0;
    }
    private void Start()
    {
        Wave = 0;
    }

    public void StartTimer()
    {
        CurrentTimer = Timer;
        Wave += 1;
        IsRunning = true;

        Managers.TurnManager.StartCombat();
    }

    public void StopTimer()
    {
        CurrentTimer = Timer;
        IsRunning = false;
        OnTimeNext?.Invoke(); 
    }

    public void UpdateTimer(float deltaTime)
    {
        if (!IsRunning) return;

        CurrentTimer -= deltaTime;

        if (CurrentTimer <= 0)
        {
            CurrentTimer = 0;
            IsRunning = false;
            OnTimeOver?.Invoke(); 
        }
    }

    public void Dispose()
    {
        OnTimeOver = null;
    }
}
