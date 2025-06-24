using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Spine;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using static Defines;

public class PlayerController : BaseController
{
    #region 변수
    enum PlayerText
    {
        None,
        StateText,
    }

    // ============ 정보 ============ 
    private CharacterManager<PlayerData> playerDataManager = new CharacterManager<PlayerData>();
    protected override ICharacterManager GetCharacterDataManager()
    {
        return playerDataManager;
    }
    public PlayerData playerData => data as PlayerData;
    #endregion

    private void Start() => Init();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        LoadData(JobType.Knight);

        // 애니메이션
        myAnim = GetComponent<Animator>();
        SettingAnimation();
        AnimState = AnimState.Idle;

        // Text
        BindText(typeof(PlayerText));

        // Weapon
        EquipWeapon(WeaponType.Basic_Weapon);

        // 옵저버 Timer 등록
        Managers.TimerManager.OnTimeOver += HandleTimerExpired;

        // 몬스터 생성
        TargetObject = GameObject.Find(strEnemyObject);
        if (TargetObject == null)
        { 
            TargetObject = Managers.Resource.Instantiate("Prefab/Character/Enemy");
            TargetObject.name = strEnemyObject;
        }

        if (rangeArea == null)
        {
            rangeArea = Managers.Resource.Instantiate("Prefab/Character/PlayerArea");
            rangeArea.name = strPlayerArea;
        }

        // 콜라이더
        SettingAreaCollider();

        return true;
    }

    private void Update()
    {
        _moveTable[MyAnimState].Invoke(TargetObject);
    }
    public override void OnTurnStart()  
    {
        if (TargetObject == null)
            return;

        MyAnimState = AnimState.Attack;
        StartCoroutine(ExecuteTurnAttack());
    }

     public override void OnTurnEnd() 
     {
        Managers.TurnManager.EndCurrentTurn();
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

    void LoadData(JobType _type)
    {
        Managers.DataManager.Players.TryGetValue("Player", out PlayerData _data);
        playerDataManager.Data = _data;
    }

    #region Observer Timer
    private void HandleTimerExpired()
    {
        Debug.Log("PlayerController: 타이머가 만료되었습니다! 플레이어가 사망합니다.");
        TakeDamage(playerData.MaxHp);
    }

    void OnDisable()
    {
        if (Managers.TimerManager != null)
            Managers.TimerManager.OnTimeOver -= HandleTimerExpired;
    }
    #endregion

    protected override void SettingAnimation()
    {
        _animTable = new()
        {
            { AnimState.Idle, a =>{ a.SetBool( "bRun", false ); } },
            { AnimState.Run, a =>{ a.SetBool("bRun", true); } },
            { AnimState.Attack, a =>{ a.SetBool("bAttack", true); } },
            { AnimState.Dead, a =>{ a.SetBool("bDead", true); } }
        };

        _moveTable = new Dictionary<AnimState, Action<GameObject>>
        {
            { AnimState.Idle, (target) => { } },
            { AnimState.Run, (target) => ReturnToPosition() }, 
            { AnimState.Attack, (target) => Attack(target) },
            { AnimState.Dead, (target) => { } },
        };
    }
}
