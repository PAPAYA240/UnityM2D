using Mono.Cecil;
using UnityEngine;
using UnityEngine.UI;
using static Defines;

public class UI_BossFolder : UI_Base
{
    #region 변수
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
        ProjectCoolTime,
    }

    private EnemyType bossType = EnemyType.Zombi_Boss;
    private UI_CheckBossFolder checkBossFolderUI;
    private RuntimeAnimatorController pendingLoadAnim;

    float LastProjectCoolTime = 30f;
    float LastProjectTime = 0.0f;

    // ======= getter/setter =======
    public RuntimeAnimatorController PendingLoadAnim
    {
        get { return pendingLoadAnim; }
        set { pendingLoadAnim = value; }
    }
    public EnemyController targetEnemyController { get; set; }

    #endregion

    private void Start() => Init();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if(!InitBind())
            Debug.Log("Failed Bind : UI_BossFolder");

        return true;
    }

    private void Update()
    {
        float ratio = GetProjectWaitRatio();
        GetImage(Images.ProjectCoolTime).fillAmount = 1.0f - ratio;
    }

    void OnSelectBossClick()
    {
        if (GetImage(Images.ProjectCoolTime).fillAmount > 0)
            return;

        if(checkBossFolderUI != null)
            checkBossFolderUI.ActiveCheckBossFolder(
                () => {
                    targetEnemyController.convertedEnemyType = bossType;
                    LastProjectTime = Managers.PlayTime;
                });
    }

    float GetProjectWaitRatio()
    {
        float playTime = Managers.PlayTime;
        float projectTime = LastProjectTime;

        float ratio = 1.0f;
        if (projectTime > 0 && projectTime < playTime)
            ratio = (playTime - projectTime) / LastProjectCoolTime;

        return ratio;
    }

    public void SetInfo(Defines.EnemyType type, UI_CheckBossFolder checkFolderUI, EnemyController enemyController)
    {
        bossType = type;
        checkBossFolderUI = checkFolderUI;
        targetEnemyController = enemyController;
    }


    #region Initialize
    private bool InitBind()
    {
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        Button bossButton = GetButton(Buttons.Boss_Type);
        if (bossButton == null)
            return false;
        BindEvent(bossButton.gameObject, OnSelectBossClick);

        return true;
    }
    #endregion
}
