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
    enum PlayerText
    {
        None,
        StateText,
    }

    public JobType JobType;
    
    private Dictionary<AnimState, Action<Animator>> _animTable;
    private Dictionary<AnimState, Action<GameObject>> _moveTable;

    // observer : Action만 이용
    public event Action<AnimState> OnStateChanged;


    public AnimState PlayerAnim
    {
        get => MyAnimState;
        set
        {
            MyAnimState = value;
            _animTable[MyAnimState].Invoke(myAnim);
            OnStateChanged?.Invoke(MyAnimState);
        }
    }

    private void Start() => Init();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // 애니메이션
        myAnim = GetComponent<Animator>();
        SettingAnimation();
        PlayerAnim = AnimState.Idle;
        JobType = JobType.Knight;

        // Text
        BindText(typeof(PlayerText));

        // Weapon
        EquipWeapon(WeaponType.Basic_Weapon);

        // 옵저버 Timer 등록
        Managers.TimerManager.OnTimerExpired += HandleTimerExpired;

        // 몬스터 생성
        TargetObject = GameObject.Find("@Enemy");
        if (TargetObject == null)
        { 
            TargetObject = Managers.Resource.Instantiate("Prefab/Character/Enemy");
            TargetObject.name = "@Enemy";
        }

        if (rangeArea == null)
        {
            rangeArea = Managers.Resource.Instantiate("Prefab/Character/PlayerArea");
            rangeArea.name = "@PlayerArea";
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
        PlayerAnim = AnimState.Dead;
        Managers.UIManager.ShowUI<UI_Base>("UI_Dead");
    }

    #region Observer Timer
    private void HandleTimerExpired()
    {
        Debug.Log("PlayerController: 타이머가 만료되었습니다! 플레이어가 사망합니다.");
        TakeDamage(State.MaxHp);
    }

    void OnDisable()
    {
        if (Managers.TimerManager != null)
            Managers.TimerManager.OnTimerExpired -= HandleTimerExpired;
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
