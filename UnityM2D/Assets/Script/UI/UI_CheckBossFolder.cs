using UnityEngine;
using static Defines;

// 변수명 변경 완료 

public class UI_CheckBossFolder : UI_Base
{
    public EnemyController targetEnemyController { get; set; }
    public EnemyType enemyType { get; private set; }
    private RuntimeAnimatorController pendingLoadAnim;

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

        if (targetEnemyController != null && pendingLoadAnim != null)
        {
            targetEnemyController.ApplyAnimator(pendingLoadAnim); 
        }
        pendingLoadAnim = null;
    }

    // Boss 창 열 때 정보 넘기기
    public void ActiveCheckBossFolder(EnemyType _enemy, bool _active = true)
    {
        this.gameObject.SetActive(_active);
        enemyType = _enemy;
    }

}
