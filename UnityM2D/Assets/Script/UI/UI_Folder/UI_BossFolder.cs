using Mono.Cecil;
using UnityEngine;
using UnityEngine.UI;
using static Defines;

public class UI_BossFolder : UI_Base
{
    enum Buttons
    {
        Boss_Type,
    }

    enum Texts
    {
        Boss_Name,
        Boss_Hp,
    }
    enum Images
    {
        Boss_Icon,
    }


    private EnemyType bossType = EnemyType.Zombi_Boss;
    private UI_CheckBossFolder _checkBossFolderUI;
    public EnemyController TargetEnemyController { get; set; }
    private RuntimeAnimatorController _pendingLoadAnim;
    public RuntimeAnimatorController PendingLoadAnim
    {
        get { return _pendingLoadAnim; }
        set { _pendingLoadAnim = value; }
    }

    private void Start() => Init();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        // Timer Observer
        Managers.TimerManager.OnTimeNext += HandleTimerExpired;

        Button bossButton = GetButton(Buttons.Boss_Type);
        BindEvent(bossButton.gameObject, OnSelectBossClick);

        return true;
    }
    
    void OnSelectBossClick()
    {
        if(_checkBossFolderUI != null)
            _checkBossFolderUI.gameObject.SetActive(true);
    }

    protected void OnDisable()
    {
        Managers.TimerManager.OnTimeNext -= HandleTimerExpired;
    }
    // 타이머 만료 시
    private void HandleTimerExpired()
    {
        if (gameObject.activeInHierarchy)
            return;

        if (TargetEnemyController != null && _pendingLoadAnim != null)
        {
            TargetEnemyController.ApplyAnimator(_pendingLoadAnim); // EnemyController의 메소드 호출
        }

        _pendingLoadAnim = null; // 적용 후 보류 중인 애니메이터 초기화
    }


    public void SetInfo(Defines.EnemyType type, UI_CheckBossFolder checkFolderUI, EnemyController enemyController)
    {
        bossType = type;
        _checkBossFolderUI = checkFolderUI;
        TargetEnemyController = enemyController;
    }
}
