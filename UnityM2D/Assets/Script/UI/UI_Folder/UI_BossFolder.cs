using UnityEngine;
using static Defines;

public class UI_BossFolder : UI_Base
{
    enum Buttons { Boss_Type }
    enum Texts { Boss_Name, Boss_Hp }
    enum Images { Boss_Icon, ProjectCoolTime }

    private EnemyType _bossType = EnemyType.Zombi_Boss;
    private UI_CheckBossFolder _checkBossPopup;
    private EnemyController _targetEnemyController;

    // 프로퍼티
    public RuntimeAnimatorController PendingLoadAnim { get; set; }

    private void Awake() => Init();

    public override bool Init()
    {
        if (!base.Init())
            return false;

        if (!InitBind())
            return false;

        return true;
    }

    private void Update()
    {
        float ratio = GetProjectWaitRatio();
        GetImage(Images.ProjectCoolTime).fillAmount = 1.0f - ratio;
    }

    public void SetInfo(EnemyType type, UI_CheckBossFolder checkPopup, EnemyController enemyController)
    {
        _bossType = type;
        _checkBossPopup = checkPopup;
        _targetEnemyController = enemyController;
    }

    void OnSelectBossClick()
    {
        // 쿨타임 중이면 리턴
        if (GetImage(Images.ProjectCoolTime).fillAmount > 0)
            return;

        if (_checkBossPopup != null)
        {
            _checkBossPopup.ActiveCheckBossFolder(() =>
            {
                if (_targetEnemyController != null)
                {
                    _targetEnemyController.convertedEnemyType = _bossType;
                    LastProjectTime = Managers.PlayTime; 
                }
            });
        }
    }

    private bool InitBind()
    {
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        GameObject bossBtn = GetButton(Buttons.Boss_Type).gameObject;
        if (bossBtn != null)
            BindEvent(bossBtn, OnSelectBossClick);
        else
            return false;

        return true;
    }
}