using System.Collections.Generic;
using System;
using UnityEngine;
using static Defines;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.InputSystem.XR;



public class EnemyController : BaseController
{
    #region 변수
    public EnemyType NextEnemyType = EnemyType.Zombi; // 다음 Enemy 미리 예약

    private Vector3 SpawnPosition = new Vector3();
    private Vector3 originScaled;
    private const float upgradeScaled = 1f;

    // WaitForSeconds
    const float waitDeadEnemy = 1f;
    const float waitChangeEnemy = 0.6f;

    // ============ 정보 ============ 
    public MonsterData CurrentCharacterData { get; private set; }

    private CharacterManager<MonsterData> monsterDataManager = new CharacterManager<MonsterData>();

    protected override ICharacterManager GetCharacterDataManager()
    {
        return monsterDataManager;
    }
    public MonsterData monsterData => data as MonsterData;
    #endregion

    private void Start() => Init();


    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        LoadData(EnemyType.Skeleton);

        // 애니메이션
        myAnim = GetComponent<Animator>();
        SettingAnimation();
        AnimState = AnimState.Idle;

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
        AnimState = AnimState.Dead;

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

            float duration = monsterData.Speed * 0.07f;

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
        if (NextEnemyType != monsterData.enemyType)
             LoadAnimator();

        // 3. 데이터 로드 및 리소스 가져오기
        LoadData(NextEnemyType);

        // 2. 스케일 변경
        if (monsterData.enemyType >= EnemyType.Zombi_Boss)
            this.transform.localScale = new Vector3(upgradeScaled, upgradeScaled, 1);
        else
            this.transform.localScale = originScaled;

        AnimState = AnimState.Idle;

        SettingAreaCollider();

        yield break;
    }
    #endregion

    #region 애니메이터 적용
    private void LoadData(EnemyType _type)
    {
        Managers.DataManager.Enemys.TryGetValue(String.Format($"{_type}"), out MonsterData myData);
        monsterDataManager.Data = myData;
    }

    private RuntimeAnimatorController LoadAnimator()
    {
        RuntimeAnimatorController loadedController;
        if (monsterData.myAnimControllerPath != null)
        {
            loadedController = Resources.Load<RuntimeAnimatorController>(monsterData.myAnimControllerPath);
            ApplyAnimator(loadedController);
        }
        else
        {
            loadedController = null;
            Debug.LogWarning("Failed Load Animation Controller : EnemyController()");
        }
    
        return loadedController;
    }
    private void ApplyAnimator(RuntimeAnimatorController controller)
    {
        if (myAnim != null && controller != null)
        {
            myAnim.runtimeAnimatorController = controller;
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
        transform.position = Managers.TransformManager.MoveToTarget(transform.position, RunAreaPosition, data.Speed);
    }

    private void Attack()
    {
        transform.position = Managers.TransformManager.MoveToTarget(transform.position, TargetObject.transform.position, data.Speed);
    }
    #endregion
}
