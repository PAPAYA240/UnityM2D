using System;
using System.Collections.Generic;
using UnityEngine;

using static Defines;

public class PlayerController : BaseController
{
    #region 변수
    // ============ Eume ============ 
    enum PlayerText
    {
        None,
        StateText,
    }


    // ============ Player Information ============ 
    private CharacterManager<PlayerData> playerDataManager = new CharacterManager<PlayerData>();
  
    public PlayerData playerData => data as PlayerData;
    protected override ICharacterManager GetCharacterDataManager()
    {
        return playerDataManager;
    }
    #endregion


    private void Start() => Init();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (!InitBind())
            Debug.Log("Failed Bind : PlayerController()");

        if (!InitAnimation())
            Debug.Log("Failed Animation : PlayerController()");

        if (!InitReigster())
            Debug.Log("Failed Reigster : EnemyController");

        LoadData(JobType.Knight);

        EquipWeapon(WeaponType.Basic_Weapon);

        Managers.TimerManager.OnTimeOver += HandleTimerOver;
        Managers.TimerManager.OnTimeNext += HandleTimerEndWave;

        return true;
    }


    private void Update()
    {
        if(TargetObject != null)
            moveTable[MyAnimState].Invoke(TargetObject);
    }

    #region Change State
    private void HandleTimerEndWave()
    {
        // TODO : 웨이브가 끝났다면
        if (playerDataManager.LevelCount >= playerDataManager.LevelCountMax)
            playerDataManager.LevelCountMax = 300;
        else
        {
            EnemyController enemy = TargetObject.GetComponent<EnemyController>();
            if (enemy != null)
                playerDataManager.Level += enemy.data.LevelCount;
        }
    }

    public override void OnTurnStart()  
    {
        // 한 턴을 시작했을 떄
        if (TargetObject == null)
            return;

        MyAnimState = AnimState.Attack;
        StartCoroutine(ExecuteTurnAttack());
    }

     public override void OnTurnEnd() 
     {
        // (한 번의 공격)한 턴을 마쳤을 때
     }

    protected override void Dead()
    {
        AnimState = AnimState.Dead;
        if (EquippedWeapon != null)
            StartCoroutine(EquippedWeapon.DeadWeapon());

        CancelInvoke(nameof(DeadUI));
        Invoke(nameof(DeadUI), 1f);
    }

    void DeadUI() => Managers.UIManager.ShowUI<UI_Base>("UI_Dead");
    private void HandleTimerOver()
    {
        Debug.Log("PlayerController: 타이머가 만료되었습니다! 플레이어가 사망합니다.");
        TakeDamage(playerData.MaxHp);
    }

    void OnDisable()
    {
        if (Managers.TimerManager != null)
        {   
            Managers.TimerManager.OnTimeOver -= HandleTimerOver;
            Managers.TimerManager.OnTimeNext -= HandleTimerEndWave;
        }
    }
    #endregion

    #region Load Change Player
    void LoadData(JobType _type)
    {
        if(playerDataManager.Data == null)
            playerDataManager.Data = new PlayerData();

        playerData.jobType = _type;
        playerDataManager.ChangeData(playerData);
    }
    #endregion

    #region Animation
    protected override void SettingAnimation()
    {
        animTable = new()
        {
            { AnimState.Idle, a =>{ a.SetBool( "bRun", false ); } },
            { AnimState.Run, a =>{ a.SetBool("bRun", true); } },
            { AnimState.Attack, a =>{ a.SetBool("bAttack", true); } },
            { AnimState.Dead, a =>{ a.SetBool("bDead", true); } }
        };

        moveTable = new Dictionary<AnimState, Action<GameObject>>
        {
            { AnimState.Idle, (target) => { } },
            { AnimState.Run, (target) => ReturnToPosition() }, 
            { AnimState.Attack, (target) => Attack(target) },
            { AnimState.Dead, (target) => { } },
        };
    }
    #endregion

    #region Initialize
    private bool InitAnimation()
    {
        myAnim = GetComponent<Animator>();
        if (myAnim == null)
            return false;

        // # Animation Setting 후 State 교체해야 합니다.
        SettingAnimation(); 

        AnimState = AnimState.Idle;

        return true;
    }

    private bool InitReigster()
    {
        // 몬스터 생성
        TargetObject = GameObject.Find(strEnemyObject);
        if (TargetObject == null)
        {
            TargetObject = Managers.Resource.Instantiate(strEnemyPath);
            TargetObject.name = strEnemyObject;
        }

        if (rangeArea == null)
        {
            rangeArea = Managers.Resource.Instantiate(strPlayerAreaPath);
            rangeArea.name = strPlayerArea;
        }

        if (rangeArea == null && rangeArea == null)
            return false;

        SettingAreaCollider();

        return true;
    }

    private bool InitBind()
    {
        BindText(typeof(PlayerText));

        return true;
    }
    #endregion
}
