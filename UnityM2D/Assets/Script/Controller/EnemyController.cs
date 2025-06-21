using System.Collections.Generic;
using System;
using UnityEngine;
using static Defines;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;
using System.Collections;
using UnityEngine.Rendering;

public class EnemyController : BaseController
{
    public EnemyType EnemyType = EnemyType.Zombi;

    private Dictionary<AnimState, Action<Animator>> _animTable;
    private Dictionary<AnimState, Action<GameObject>> _moveTable;

    public event Action<AnimState> OnStateChanged;

    private Vector3 SpawnPosition = new Vector3();

    private Vector3 originScaled;
    private const float upgradeScaled = 1f;

    public AnimState EnemyAnim
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
        EnemyAnim = AnimState.Idle;

        EquipWeapon(WeaponType.None_Weapon);

        TargetObject = GameObject.Find(strPlayerObject);

        if (rangeArea == null)
        {
            rangeArea = Managers.Resource.Instantiate("Prefab/Character/EnemyArea");
            rangeArea.name = strEnemyArea;
        }


        Managers.TimerManager.OnTimeNext += HandleTimerExpired;

        // Load Enemy Spawn Position 
        GameObject spawnArea = Managers.Resource.Instantiate("Prefab/Character/EnemySpawnArea");
        this.transform.position = SpawnPosition = spawnArea.transform.position;
        originScaled = spawnArea.transform.localScale;
        // 스케일이 자꾸 1로 불러와짐

        Destroy(spawnArea);

        SettingAreaCollider();

        return true; 
    }

    private void Update()
    {
        if(Managers.Scene.CurrentSceneType == Defines.Scene.InGame)
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

    private void Move()
    {
        transform.position = Managers.TransformManager.MoveToTarget(transform.position, RunAreaPosition, MyState.Speed);
    }

    private void Attack()
    {
         transform.position = Managers.TransformManager.MoveToTarget(transform.position, TargetObject.transform.position, MyState.Speed);
    }
    protected override void Dead()
    {
        EnemyAnim = AnimState.Dead;

        StartCoroutine(NextEnemy());
    }

    // 다음 적
    private void HandleTimerExpired()
    {
        // Scaled
        if (EnemyType >= EnemyType.Zombi_Boss)
            this.transform.localScale = new Vector3(upgradeScaled, upgradeScaled, 1);
        else
            this.transform.localScale = originScaled;
    }

    #region 애니메이터 적용
    public RuntimeAnimatorController LoadAnimator(EnemyType _type)
    {
        string path = "";
        switch (_type)
        {
            case Defines.EnemyType.Zombi_Boss:
                path = "Prefab/Animation/Zombi_Boss/ZombiBossAnim"; 
                break;
            case Defines.EnemyType.Skeleton_Boss:
                path = "Prefab/Animation/Skeleton_Boss/SkeletonBossAnim";
                break;
            default:
                Debug.LogWarning($"EnemyType: {_type}에 대한 애니메이터 경로가 정의되지 않았습니다.");
                return null;
        }
        return Resources.Load<RuntimeAnimatorController>(path);
    }
    public void ApplyAnimator(RuntimeAnimatorController controller)
    {
        if (myAnim != null && controller != null)
        {
            myAnim.runtimeAnimatorController = controller;
            Debug.Log($"애니메이터 적용됨: {controller.name}");
        }
        else
        {
            if (myAnim == null) Debug.LogError("애니메이터를 적용할 수 없습니다: Animator 컴포넌트가 null입니다.");
            if (controller == null) Debug.LogWarning("애니메이터를 적용할 수 없습니다: 제공된 컨트롤러가 null입니다.");
        }
    }

    #endregion
    private IEnumerator NextEnemy()
    {
        yield return new WaitForSeconds(0.8f);
        const float stopDistance = 0.1f;

        while (Vector3.Distance(transform.position, SpawnPosition) > stopDistance)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = SpawnPosition;

            float duration = MyState.Speed * 0.07f;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime/duration;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
            yield return null;
        }
        transform.position = SpawnPosition;
        yield return new WaitForSeconds(0.6f);

        StartCoroutine(ChangeEnemy());
    }

    private IEnumerator ChangeEnemy()
    {
        // 데이터 로드 및 리소스 가져오기
        State.Hp = State.MaxHp;

        EnemyAnim = AnimState.Idle;

        // EnemyArea 도 다시 가져오기
        SettingAreaCollider();

        // 초 시작 전 1초 대기
        yield return new WaitForSeconds(0.5f);
        Managers.TimerManager.StartTimer();
    }
    protected override void SettingAnimation()
    {
        _animTable = new()
        {
            { AnimState.Idle, a =>{ a.SetBool( "bRun", false ); a.SetBool( "bDead", false );} },
            { AnimState.Dead, a =>{ a.SetBool("bDead", true); } }
        };

        _moveTable = new Dictionary<AnimState, Action<GameObject>>
        {
             { AnimState.Idle, (target) => { Move(); } },
            { AnimState.Run, (target) => ReturnToPosition() },
            { AnimState.Attack, (target) => Attack(target) },
            { AnimState.Dead, (target) => { } },
        };
    }
}
