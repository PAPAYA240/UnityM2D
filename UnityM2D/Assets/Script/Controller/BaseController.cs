using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static Defines;

public abstract  class BaseController : Base, ITurnParticipant
{
    #region Enum
    enum GameObjects
    {
        None,
        WeaponSocket,
        HP_Position,
    }
    #endregion 

    #region Value
    // ============ Strategy Attack ============
    protected Weapon EquippedWeapon = null;
    public GameObject TargetObject = null;
    protected GameObject bomberObject = null;

    // ============ Character Information ============
    protected virtual CharacterManager<CharacterData> Data() { return null;  }
    public bool isAlive => IsAlive();

    protected Animator _myAnimation = null;
    protected AnimState MyAnimState = AnimState.None;

    protected Dictionary<AnimState, Action<Animator>> animTable = null;
    protected Dictionary<AnimState, Action<GameObject>> moveTable = null;
    public event Action<AnimState> OnStateChanged = null;

    // ============ Load Character Info ============ 
    protected ICharacterManager characterDataManager;
    protected abstract ICharacterManager GetCharacterDataManager();
    public CharacterData data => characterDataManager?.GetCurrentCharacterData();

    // ============ const Area ============ 
    protected GameObject rangeArea = null;
    protected BoxCollider2D rangeCollider = null;
    protected Vector3 RunAreaPosition;

    // ======== Seconds ========
    const float waitStartAttack = 0.3f;
    const float waitOnAttack = 0.2f;
    const float waitEndAttack = 0.6f;
    const float AttackOffset = 0.5f;

    // ======== getter/setter ========
    public AnimState AnimState
    {
        get => MyAnimState;
        set
        {
            MyAnimState = value;
            animTable[MyAnimState].Invoke(_myAnimation);
            OnStateChanged?.Invoke(MyAnimState);
        }
    }
    public BaseController GetOwner()
    {
        return this;
    }
    #endregion
    private SpriteRenderer myRenderer;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        characterDataManager = GetCharacterDataManager();
        myRenderer = GetComponent<SpriteRenderer>();
        if (!InitBind())
            Debug.Log("Failed Bind : BaseController");

        if (!InitUI())
            Debug.Log("Failed UI : BaseController");

        // 전투 참여자 등록
        Managers.TurnManager.RegisterParticipant(this);

        return true;
    }


    #region Attack
    public void TakeDamage(int _amount)
    {
        if (MyAnimState == AnimState.Dead) return;

        StartCoroutine(ObjectDamageColor());

        if (_amount < 0) _amount = 0;
        data.Hp -= _amount;
        if(data.Hp < 0) data.Hp = 0;

        if (data.Hp <= 0)
            Dead();
    }

    IEnumerator ObjectDamageColor()
    {
        myRenderer.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        yield return new WaitForSeconds(0.3f);
        myRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }
    protected virtual IEnumerator Attack(GameObject target)
    {
        // 공격 시작 시 호출할 함수

        if (!IsAlive())
            yield break;

        if (target == null)
            yield break;

        if (EquippedWeapon != null)
            yield return StartCoroutine(EquippedWeapon.PerformAttack(this.gameObject, target));
    }

    protected IEnumerator ExecuteTurnAttack()
    {
        yield return new WaitForSeconds(waitStartAttack);

        // 몬스터에게 다가가는 시간
        yield return StartCoroutine(Attack(TargetObject));

        // 공격 중인 시간
        yield return new WaitForSeconds(waitOnAttack);

        MyAnimState = AnimState.Run;
        // 돌아서는 시간 
        yield return new WaitForSeconds(waitEndAttack);

        Managers.TurnManager.EndCurrentTurn();
    }

    #endregion

    #region State
    protected virtual void Dead() { }

    private bool IsAlive()
    {
        return data.Hp > 0;
    }

    /// <summary>
    /// 근접 공격 시 수행할 함수
    /// </summary>
    public virtual IEnumerator PerformMelleAttack(GameObject _target)
    {
        if (_target == null)
            yield break;

        Vector3 finalTargetPosition = new Vector3();
        if (transform.position.x > _target.transform.position.x)
            finalTargetPosition = _target.transform.position + new Vector3(AttackOffset, 0, 0);
        else
            finalTargetPosition = _target.transform.position - new Vector3(AttackOffset, 0, 0);

        const float stopDistance = 0.1f;
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

            ReactionWeapon();

            // 플레이어 데미지
            BaseController bc = _target.GetComponent<BaseController>();
            if (bc != null)
            {
                int damage = GetAttackPower();
                bc.TakeDamage(damage);
            }

            transform.position = targetPosition;
            yield return null;
        }
        transform.position = finalTargetPosition;
    }

    public virtual int GetAttackPower()
    {
        return 0;
    }
    public virtual int GetDefence()
    {
        return 0;
    }

    protected void ReturnToPosition()
    {
        transform.position = Managers.TransformManager.MoveToTarget(transform.position, RunAreaPosition, data.AttackSpeed);
    }

    #endregion

    #region Weapon
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
                EquippedWeapon.Init(weaponData, this, socketObject, _weaponType);
            }
        }
        else
        {
            Console.WriteLine("Failed Load Weapon Prefab : Basecontroller()");
            EquippedWeapon = null;
        }
    }

    /// <summary>
    /// 호출 시 무기에 반동을 줄 함수
    /// </summary>
    public void ReactionWeapon()
    {
        if (EquippedWeapon != null)
            EquippedWeapon.OperateWeapon();
    }
    #endregion

    #region 세팅
    virtual public void OnTurnStart() { }

    virtual public void OnTurnEnd() { }

    protected virtual void SettingAnimation() { }

    public Vector3 SettingAreaCollider()
    {
        if (rangeArea == null)
            return new Vector3();

        rangeArea.transform.position = new Vector3(0, 0, 0);

        if (rangeCollider == null)
            rangeCollider = rangeArea.GetComponent<BoxCollider2D>();

        Bounds bounds = rangeCollider.bounds;
        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float minY = bounds.min.y;
        float maxY = bounds.max.y;

        Vector3 resultPos = new Vector3();
        resultPos.x = UnityEngine.Random.Range(minX, maxX);
        resultPos.y = UnityEngine.Random.Range(minY, maxY);
        
        return resultPos;
    }
    #endregion

    #region Initialize
    private bool InitUI()
    {
        GameObject hpObj = GetObject(GameObjects.HP_Position);
        if (hpObj == null)
            return true;

        UI_Base HpUI = Managers.UIManager.ShowUI<UI_Slide>(CreateHpBar, GetObject(GameObjects.HP_Position).gameObject.transform);
        if (HpUI == null)
            return false;

        HpUI.SetInfo(GetObject(GameObjects.HP_Position).gameObject, true);
        return true;
    }
    
    private bool InitBind()
    {
        BindObject(typeof(GameObjects));

        return true;
    }
    #endregion
}
