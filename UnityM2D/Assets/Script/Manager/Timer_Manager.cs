using System;
using UnityEngine;

public class Timer_Manager : MonoBehaviour
{
    // 이벤트를 정의하여 옵저버들이 구독하게 한다.
    public event Action OnTimerExpired; // 타이머 만료 시 호출

    public float CurrentTimer { get; private set; }
    public int Wave { get; private set; }
    public bool IsRunning { get; private set; }

    private int Timer = 5;

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
    }

    public void UpdateTimer(float deltaTime)
    {
        if (!IsRunning) return;

        CurrentTimer -= deltaTime;

        if (CurrentTimer <= 0)
        {
            CurrentTimer = 0;
            IsRunning = false;
            OnTimerExpired?.Invoke(); // 이벤트 발생
        }
    }

    // 구독 해지 작업
    public void Dispose()
    {
        OnTimerExpired = null;
    }
}
