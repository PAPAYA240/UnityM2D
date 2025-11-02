using UnityEngine;

public class UI_Timer : UI_Base
{
    enum TimerText
    {
        TimerText,
        WaveText,
    }

    float CurrTime = 0;
    const float StartTimer = 1;
    bool bTimerStart = false;

    public bool TimerStart { private get;  set; }
    void Start()
    {
        BindText(typeof(TimerText));
    }

    void Update()
    {
        if (bTimerStart == false)
        {
            CurrTime += Time.deltaTime;
            if (CurrTime > StartTimer)
            {
                bTimerStart = true;
                Managers.TimerManager.StartTimer(); 
            }
        }
        
        UpdateTimer();
    }

    void UpdateTimer()
    {
        int currentSeconds = (int)Managers.TimerManager.CurrentTimer;
        int minutes = Mathf.FloorToInt(currentSeconds / 60); 
         int seconds = Mathf.FloorToInt(currentSeconds % 60); 
        if (10 > minutes)
            GetText(TimerText.TimerText).text = string.Format($"0{minutes}:{seconds}");
        else
            GetText(TimerText.TimerText).text = string.Format($"{minutes}:{seconds}");
        Wave();
    }
    // 다음 웨이브 시작
    public void Wave()
    {
        GetText(TimerText.WaveText).text = $"{Managers.TimerManager.Wave} WAVE";
    }
}
