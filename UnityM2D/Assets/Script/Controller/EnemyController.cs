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
    private EnemyType EnemyType = EnemyType.Zombi;
    public EnemyType NextEnemyType = EnemyType.Zombi;

    private Dictionary<AnimState, Action<Animator>> _animTable;
    private Dictionary<AnimState, Action<GameObject>> _moveTable;

    public event Action<AnimState> OnStateChanged;

    private Vector3 SpawnPosition = new Vector3();

    private Vector3 originScaled;
    private const float upgradeScaled = 1f;

    const float waitDeadEnemy = 1f;
    const float waitChangeEnemy = 0.6f;

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
        originScaled = this.transform.localScale;

        if (rangeArea == null)
        {
            rangeArea = Managers.Resource.Instantiate("Prefab/Character/EnemyArea");
            rangeArea.name = strEnemyArea;
        }

        // Load Enemy Spawn Position 
        GameObject spawnArea = Managers.Resource.Instantiate("Prefab/Character/EnemySpawnArea");
        this.transform.position = SpawnPosition = spawnArea.transform.position;
        Destroy(spawnArea);

        SettingAreaCollider();

        Managers.TimerManager.OnTimeNext += HandleTimerNext;

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

    protected override void Dead()
    {
        EnemyAnim = AnimState.Dead;

        StartCoroutine(NextEnemy());
    }

    // 다음 적 변경 시
    private void HandleTimerNext()
    {
    }

    #region Enemy 변경
    private IEnumerator NextEnemy()
    {
        yield return new WaitForSeconds(waitDeadEnemy);
        const float stopDistance = 0.1f;

        // 1. Start Spawn으로 돌아가기
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

        StartCoroutine(ChangeEnemy());

        yield return new WaitForSeconds(waitChangeEnemy);

        Managers.TimerManager.StartTimer();
    }

    private IEnumerator ChangeEnemy()
    {
        // 1. 애니메이션 리로드
        LoadAnimator();

        // 2. 스케일 변경
        if (EnemyType >= EnemyType.Zombi_Boss)
            this.transform.localScale = new Vector3(upgradeScaled, upgradeScaled, 1);
        else
            this.transform.localScale = originScaled;

        // 3. 데이터 로드 및 리소스 가져오기
        State.Hp = State.MaxHp;

        EnemyAnim = AnimState.Idle;

        // 4. EnemyArea 위치 가져오기
        SettingAreaCollider();

        yield break;
    }
    #endregion

    #region 애니메이터 적용
    public RuntimeAnimatorController LoadAnimator()
    {
        if (NextEnemyType == EnemyType)
            return null;
        EnemyType = NextEnemyType;

        string path = "";
        switch (EnemyType)
        {
            case Defines.EnemyType.Zombi_Boss:
                path = "Prefab/Animation/Zombi_Boss/ZombiBossAnim";
                break;
            case Defines.EnemyType.Skeleton_Boss:
                path = "Prefab/Animation/Skeleton_Boss/SkeletonBossAnim";
                break;
            default:
                Debug.LogWarning($"EnemyType: {EnemyType}에 대한 애니메이터 경로가 정의되지 않았습니다.");
                return null;
        }
        RuntimeAnimatorController loadedController = Resources.Load<RuntimeAnimatorController>(path);
        myAnim.runtimeAnimatorController = loadedController;

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
    protected override void SettingAnimation()
    {
        _animTable = new()
        {
            { AnimState.Idle, a =>{ a.SetBool( "bRun", false ); a.SetBool( "bDead", false );} },
             { AnimState.Run, a =>{ a.SetBool("bRun", true); } },
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
    #endregion

    #region 움직임
    private void Move()
    {
        transform.position = Managers.TransformManager.MoveToTarget(transform.position, RunAreaPosition, MyState.Speed);
    }

    private void Attack()
    {
        transform.position = Managers.TransformManager.MoveToTarget(transform.position, TargetObject.transform.position, MyState.Speed);
    }
    #endregion
}
