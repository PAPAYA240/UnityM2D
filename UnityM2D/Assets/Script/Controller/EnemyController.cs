using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

using static Defines;
using System.Linq;
using Mono.Cecil;


public class EnemyController : BaseController
{
    #region 변수
    // ======== Enemy Information ========
    public EnemyType    convertedEnemyType = EnemyType.Zombi;
    public MonsterData CurrentCharacterData { get; private set; }
    private CharacterManager<MonsterData> monsterDataManager = new CharacterManager<MonsterData>();
    public MonsterData monsterData => data as MonsterData;
    protected override ICharacterManager GetCharacterDataManager()
    {
        return monsterDataManager;
    }

    // ======== Check Point ========
    private Vector3         spawnPosition = new Vector3();
    private Vector3         originScaled;
    private const float     upgradeScaled = 1f;

    // ======== WaitForSeconds ========
    const float waitDeadEnemy = 1f;
    const float waitChangeEnemy = 0.6f;
    #endregion

    private void Start() => Init();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (!InitAnimation())
            Debug.Log("Failed Animation : EnemyController");

        if(!InitReigster())
            Debug.Log("Failed Reigster : EnemyController");

        LoadData(EnemyType.Zombi);

        EquipWeapon(WeaponType.None_Weapon);

        Managers.TimerManager.OnTimeNext += HandleTimerNext;

        return true; 
    }

    private void Update()
    {
        if(Managers.Scene.CurrentSceneType == Defines.Scene.InGame)
             moveTable[MyAnimState].Invoke(TargetObject);
    }

    #region Change State
    public override void OnTurnStart()
    {
        // 타이머 시작 순간의 공간
        if (TargetObject == null)
            return;

        MyAnimState = AnimState.Attack;

        StartCoroutine(ExecuteTurnAttack());
    }

    public override void OnTurnEnd()
    {
        // 타이머 종료 순간의 공간
    }

    protected override void Dead()
    {
        AnimState = AnimState.Dead;

        PlayerController player = TargetObject.GetComponent<PlayerController>();
        if(player != null)
            player.data.LevelCount += monsterDataManager.Level;

        StartCoroutine(NextEnemy());
    }
    #endregion

    #region Load Change Enemy
    private void HandleTimerNext()
    {
        // 다음 적 변경 시 사용할 공간
    }

    private IEnumerator NextEnemy()
    {
        yield return new WaitForSeconds(waitDeadEnemy);
        const float stopDistance = 0.1f;

        // 1. Start Spawn으로 돌아가기
        while (Vector3.Distance(transform.position, spawnPosition) > stopDistance)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = spawnPosition;

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
        transform.position = spawnPosition;

        // 변경 방식
        SelectEnemy();

        StartCoroutine(ChangeEnemy());

        yield return new WaitForSeconds(waitChangeEnemy);

        Managers.TimerManager.StartTimer();
    }

    const int allowanceCnt = 3;
    int overlapCnt = 0;
    int prevCnt = 0;

    void SelectEnemy()
    {
        if(convertedEnemyType == EnemyType.None)
        {
            int selectInt = UnityEngine.Random.Range((int)EnemyType.Zombi, (int)EnemyType.Zombi_Boss);
            convertedEnemyType = (EnemyType)selectInt;

            if (prevCnt == selectInt)
                ++overlapCnt;
            else
                overlapCnt = 0;
            prevCnt = selectInt;

            if(overlapCnt >= allowanceCnt)
            {
                while(selectInt == prevCnt)
                    selectInt = UnityEngine.Random.Range((int)EnemyType.Zombi, (int)EnemyType.Zombi_Boss);

                prevCnt = selectInt;
                overlapCnt = 0;
            }

        }
    }

    private List<T> GetNotDuplicateRandomList_HashSet<T>(IList<T> list, int count)
    {
        HashSet<T> hashSet = new HashSet<T>(list);
        List<T> uniqueList = hashSet.ToList();
        List<T> result = new List<T>();
        int n = uniqueList.Count;

        // count > n => error!

        for (int i = 0; i < count; i++)
        {
            int r = UnityEngine.Random.Range(i, n);
            T temp = uniqueList[i];
            uniqueList[i] = uniqueList[r];
            uniqueList[r] = temp;
            result.Add(uniqueList[i]);
        }

        return result;
    }

    private IEnumerator ChangeEnemy()
    {
        LoadData(convertedEnemyType);

        if (monsterData.enemyType >= EnemyType.Zombi_Boss)
            this.transform.localScale = new Vector3(upgradeScaled, upgradeScaled, 1);
        else
            this.transform.localScale = originScaled;

        AnimState = AnimState.Idle;

        SettingAreaCollider();

        yield break;
    }
    #endregion

    #region Animation
    private void LoadData(EnemyType _type)
    {
        if(monsterDataManager.Data == null)
        { 
            monsterDataManager.Data = new MonsterData();
        }

        bool bChangeAnim = false;
        if (convertedEnemyType != monsterData.enemyType)
            bChangeAnim = true;

        monsterData.enemyType = convertedEnemyType;
        if (monsterData.enemyType == convertedEnemyType)
            convertedEnemyType = EnemyType.None;

        // # ChangeData() 사용 전에 꼭 다음 type으로 monsterData.enemyType 세팅할 것
        monsterDataManager.ChangeData(monsterData);

        if(bChangeAnim == true)
            LoadAnimator();

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
        animTable = new()
        {
            { AnimState.Idle, a =>{ a.SetBool( "bRun", false ); a.SetBool( "bDead", false );} },
             { AnimState.Run, a =>{ a.SetBool("bRun", true); } },
            { AnimState.Dead, a =>{ a.SetBool("bDead", true); } }
        };

        moveTable = new Dictionary<AnimState, Action<GameObject>>
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

    #region Initialize
    private bool InitAnimation()
    {
        myAnim = GetComponent<Animator>();
        if (myAnim == null)
            return false;

        SettingAnimation();

        AnimState = AnimState.Idle;

        return true;
    }

    private bool InitReigster()
    {
        // Enemy Info
        TargetObject = GameObject.Find(strPlayerObject);
        originScaled = this.transform.localScale;

        // Enemy Area
        rangeArea = Managers.Resource.Instantiate(strEnemyAreaPath);
        if(rangeArea == null) 
            return false;
        rangeArea.name = strEnemyArea;

        GameObject spawnArea = Managers.Resource.Instantiate(strEnemySpawnAreaPath);
        if(spawnArea == null)
            return false;
        this.transform.position = spawnPosition = spawnArea.transform.position;
        Destroy(spawnArea);

        SettingAreaCollider();

        return true;
    }
    #endregion
}
