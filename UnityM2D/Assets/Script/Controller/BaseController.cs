using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using static Defines;
using System;
using Unity.IO.LowLevel.Unsafe;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.CullingGroup;
using UnityEditor.Experimental.GraphView;
using System.Resources;
using UnityEngine.EventSystems;



public abstract  class BaseController : Base, ITurnParticipant
{
    // 공격 전략
    protected Weapon EquippedWeapon = null;
    protected GameObject TargetObject = null;

    // 캐릭터 정보
    protected virtual CharacterManager<CharacterData> Data() { return null;  }
    protected Animator myAnim;
    protected AnimState MyAnimState = AnimState.None;

    public bool isAlive => IsAlive();

    protected Dictionary<AnimState, Action<Animator>> _animTable;
    protected Dictionary<AnimState, Action<GameObject>> _moveTable;

    public event Action<AnimState> OnStateChanged;

    // ============ 정보 ============ 
    protected ICharacterManager characterDataManager;
    protected abstract ICharacterManager GetCharacterDataManager();

    // 데이터를 가져올 때는 ICharacterManager를 통해 CharacterData로 받습니다.
    public CharacterData data => characterDataManager?.GetCurrentCharacterData();



    public AnimState AnimState
    {
        get => MyAnimState;
        set
        {
            MyAnimState = value;
            _animTable[MyAnimState].Invoke(myAnim);
            OnStateChanged?.Invoke(MyAnimState);
        }
    }

    // Run Area
    protected GameObject rangeArea;
    protected BoxCollider2D rangeCollider;
    protected Vector3 RunAreaPosition;

    // 공격 초
    const float StartWaitSeconds = 0.3f;
    const float AttackWaitSeconds = 0.2f;
    const float ReturnWaitSeconds = 0.6f;

    const float AttackOffset = 0.5f;
    
    enum GameObjects
    {
        None,
        WeaponSocket,
        HP_Position,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        characterDataManager = GetCharacterDataManager();

        myAnim = GetComponent<Animator>();
        BindObject(typeof(GameObjects));

        UI_Base HpUI = Managers.UIManager.ShowUI<UI_Slide>("UI_HP", GetObject(GameObjects.HP_Position).gameObject.transform);
        HpUI.SetInfo(GetObject(GameObjects.HP_Position).gameObject, true);

        Managers.TurnManager.RegisterParticipant(this); // 공격 등록


        return true;
    }


#region 데미지
    public void TakeDamage(int _amount)
    {
        if (MyAnimState == AnimState.Dead) return;

        if (_amount < 0) _amount = 0;
        data.Hp -= _amount;
        if(data.Hp < 0) data.Hp = 0;

        if (data.Hp <= 0)
            Dead();
    }

    #endregion

    #region 움직임 패턴
    // 죽음
    protected virtual void Dead() { }

    // 생존 여부
    private bool IsAlive()
    {
        return data.Hp > 0;
    }

    // 공격 시작
    protected virtual IEnumerator Attack(GameObject _target)
    {
        if (!IsAlive())
            yield break;

        if (_target == null)
            yield break;

        if (EquippedWeapon != null)
            yield return StartCoroutine(EquippedWeapon.PerformAttack(this.gameObject, _target));

        else
            yield break;
    }

    protected IEnumerator ExecuteTurnAttack()
    {
        yield return new WaitForSeconds(StartWaitSeconds);

        // 몬스터에게 다가가는 시간
        yield return StartCoroutine(Attack(TargetObject));

        // 공격 중인 시간
        yield return new WaitForSeconds(AttackWaitSeconds);

        MyAnimState = AnimState.Run;
        // 돌아서는 시간 
        yield return new WaitForSeconds(ReturnWaitSeconds);

        Managers.TurnManager.EndCurrentTurn();
    }

    // 근접 공격
    public virtual IEnumerator AttacktoMove(GameObject _target)
    {
        if (_target == null)
            yield break; 

        const float stopDistance = 0.1f; 

        Debug.Log($"{gameObject.name}가 {_target.name}에게 이동을 시작합니다.");

        // 위치 조절
        Vector3 finalTargetPosition = new Vector3();
        if (transform.position.x > _target.transform.position.x) 
            finalTargetPosition = _target.transform.position + new Vector3(AttackOffset, 0, 0);
        else
            finalTargetPosition = _target.transform.position - new Vector3(AttackOffset, 0, 0);

        while (Vector3.Distance(transform.position, finalTargetPosition) > stopDistance)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = finalTargetPosition;

            float duration = data.AttackSpeed * 0.03f; 
                                        
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 1. 무기 반동
            if(EquippedWeapon != null)
                ReactionWeapon();

             // 2. Target 데미지
             BaseController targetCon = _target.GetComponent<BaseController>();
             targetCon.TakeDamage(data.AttackPower);

            // 3. Position 재정립
            transform.position = targetPosition;
            yield return null; 
        }

        transform.position = finalTargetPosition;
        Debug.Log($"{gameObject.name}가 {_target.name}에 도착했습니다.");
    }



    public void ReactionWeapon()
    {
         if (EquippedWeapon != null)
             StartCoroutine(EquippedWeapon.ReactionWeapon());
    }
    // 제자리로 돌아가기
    protected void ReturnToPosition()
    {
        transform.position = Managers.TransformManager.MoveToTarget(transform.position, RunAreaPosition, data.AttackSpeed);
    }

    #endregion

    #region 세팅
    virtual public void OnTurnStart() { }

    virtual public void OnTurnEnd() { }


    protected virtual void SettingAnimation() { }

    // 캐릭터 자체를 변경 시
    protected void ChangeCharacterInfo(string _characterType)
    {
        //CharacterData CD = Managers.CharacterLoader.GetCharacterData(_characterType);
        //if(CD == null)
        //{
        //    Debug.LogWarning("Faield Load CharacterData : BaseController()");
        //    return;
        //}

        //data = CD;
    }

    // 무기 변경
    public void EquipWeapon(WeaponType _weaponType)
    {
        WeaponData weaponData = Managers.WeaponLoader.GetWeaponData(_weaponType);
        if (weaponData == null)
        {
            Debug.LogWarning("Failed Load WeaponData : BaseController()");
            return;
        }

        if (EquippedWeapon != null)
        {
            Destroy(EquippedWeapon.gameObject);
            EquippedWeapon = null;
        }

        if (weaponData.weaponPrefab != null)
        {
            GameObject socketObject = GetObject(GameObjects.WeaponSocket).gameObject;
            GameObject WeaponModel = Instantiate(weaponData.weaponPrefab, socketObject.transform);
            if (EquippedWeapon == null)
            {
                EquippedWeapon = WeaponModel.AddComponent<Weapon>();
                EquippedWeapon.Init(weaponData, this, socketObject);
            }
        }
        else
        {
            Console.WriteLine("Failed Load Weapon Prefab : Basecontroller()");
            EquippedWeapon = null;
        }

    } 

    protected void SettingAreaCollider()
    {
        if (rangeArea == null)
            return;

        rangeArea.transform.position = new Vector3(0, 0, 0);

        if (rangeCollider == null)
            rangeCollider = rangeArea.GetComponent<BoxCollider2D>();

        Bounds bounds = rangeCollider.bounds;
        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float minY = bounds.min.y;
        float maxY = bounds.max.y;
        RunAreaPosition.x = UnityEngine.Random.Range(minX, maxX);
        RunAreaPosition.y = UnityEngine.Random.Range(minY, maxY);
    }
    #endregion

}
