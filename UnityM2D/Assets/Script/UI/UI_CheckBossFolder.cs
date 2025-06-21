using UnityEngine;

public class UI_CheckBossFolder : UI_Base
{
    public EnemyController TargetEnemyController { get; set; }
    private RuntimeAnimatorController _pendingLoadAnim;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.TimerManager.OnTimeNext += HandleTimerExpired;

        return true;
    }
    protected void OnDisable() 
    {
        Managers.TimerManager.OnTimeNext -= HandleTimerExpired;
    }
    private void HandleTimerExpired()
    {
        if (gameObject.activeInHierarchy)
            return;

        if (TargetEnemyController != null && _pendingLoadAnim != null)
        {
            TargetEnemyController.ApplyAnimator(_pendingLoadAnim); 
        }

        _pendingLoadAnim = null;
    }
}
