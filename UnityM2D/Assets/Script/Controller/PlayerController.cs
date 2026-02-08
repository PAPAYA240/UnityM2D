using System;
using System.Collections;
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
    public GameObject[] PlayerSkills;
    Vector3 initPosition = new Vector3(-396.6545f, -995.24f, 0);


    protected override ICharacterManager GetCharacterDataManager()
    {
        return playerDataManager;
    }
    #endregion
    public float bomberDuration = 15.0f;

    private void Start() => Init();

    public override bool Init()
    {
        if (!base.Init())
            return false;

        if (!InitBind())
            Debug.Log("Failed Bind : PlayerController()");

        if (!InitAnimation())
            Debug.Log("Failed Animation : PlayerController()");

        if (!InitReigster())
            Debug.Log("Failed Reigster : EnemyController");

        LoadData(JobType.Knight);

        EquipWeapon(WeaponType.Basic_Weapon);

        PlayerSkills = new GameObject[(int)FixType.End_Fix];
        
        Managers.TimerManager.OnTimeOver += HandleTimerOver;
        Managers.TimerManager.OnTimeNext += HandleTimerEndWave;

        data.Hp = data.MaxHp;
        data.Money = 500;
        return true;
    }

    private float _bomberTimer = 0f;
    private void Update()
    {
        if(TargetObject == null)
            TargetObject = GameObject.Find(strEnemyObject);

        if (TargetObject != null)
            moveTable[MyAnimState].Invoke(TargetObject);

        _bomberTimer += Time.deltaTime;

        // 2. 누적된 시간이 설정한 주기(bomberDuration)보다 커지면 실행합니다.
        if (_bomberTimer >= bomberDuration)
        {
            StartCoroutine(UseSkill(FixType.Bomber_Fix));

            // 3. 실행 후 타이머를 0으로 초기화합니다.
            _bomberTimer = 0f;
        }
    }


    public IEnumerator UseSkill(FixType _skillType, bool bUpgrade = false)
    {
        if (bUpgrade)
        {
            if (PlayerSkills[(int)_skillType] == null)
                Install(_skillType);
            else
                bomberDuration -= 1.0f;

            if (bomberDuration <= 3)
                bomberDuration = 3;
        }
        else
        {
            if (PlayerSkills[(int)_skillType] == null)
                yield break;

            bool usedSkill = false;
            while (!usedSkill)
            {
                usedSkill = PlayerSkills[(int)_skillType].GetComponent<Skill>().ExecuteSkill(this.gameObject, TargetObject);

                yield return null;
            }

            yield break;
        }
    }

    private void Install(FixType _skillType)
    {
        switch (_skillType)
        {
            case FixType.Pet_Fix:
                break;

            case FixType.Bomber_Fix:
                GameObject bomberMachine = Managers.Resource.Instantiate("Prefab/Weapon/BomberMachine");
                bomberMachine.AddComponent<BomberMachine>();
                PlayerSkills[(int)FixType.Bomber_Fix] = bomberMachine;
                break;

            default:
                break;
        }
    }

    void OnDestroy()
    {
        for(int i = 0; i < PlayerSkills.Length; i++)
        {
            if (PlayerSkills[i] != null)
                Managers.Resource.Destroy(PlayerSkills[i]);
        }
    }
    #region Change State
    private void HandleTimerEndWave()
    {
        // TODO : 웨이브가 끝났다면
        if (playerDataManager.LevelCount >= playerDataManager.LevelCountMax)
        {
            _UILevelUp?.Invoke(1.5f);
            playerDataManager.LevelCountMax = 300; 
        }
        else
        {
            if (!TargetObject)
                return;

            EnemyController enemy = TargetObject.GetComponent<EnemyController>();
            if (enemy != null)
                playerDataManager.LevelCount += enemy.data.LevelCount;
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

    void DeadUI() => Managers.UIManager.ShowUI<UI_Dead>("UI_Dead");
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

    // NOTE. 상태별 행동 로직을 if문으로 검사하지 않고, Dictionary/Delegate를 활용한 상태 패턴을 적용했습니다.
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

    public void UpgradeHeal(int add)
    {
        data.Heal += add;
    }
    public override int GetAttackPower()
    {
        return data.AttackPower;
    }

    public void SetHeal(int heal)
    {
        data.Hp += heal;
        if(data.Hp > data.MaxHp)
            data.Hp = data.MaxHp;
    }

    #region Initialize
    private bool InitAnimation()
    {
        _myAnimation = GetComponent<Animator>();
        if (_myAnimation == null)
            return false;

        // # Animation Setting 후 State 교체해야 합니다.
        SettingAnimation(); 

        AnimState = AnimState.Idle;

        return true;
    }
    UI_LevelUp _UILevelUp = null;
    private bool InitReigster()
    {
        // 몬스터 생성
        TargetObject = GameObject.Find(strEnemyObject);
        if (TargetObject == null)
        {
            GameObject enemy = Managers.Resource.Instantiate(strEnemyPath);
            enemy.name = strEnemyObject;
            TargetObject = enemy;
        }

        if (rangeArea == null)
        {
            rangeArea = Managers.Resource.Instantiate(strPlayerAreaPath);
            rangeArea.name = strPlayerArea;
        }

        if (rangeArea == null)
            return false;

         _UILevelUp = gameObject.GetComponentInChildren<UI_LevelUp>();
         _UILevelUp.gameObject.SetActive(false);
         _UILevelUp.SetInfo(gameObject, true);

        RunAreaPosition = SettingAreaCollider();

        return true;
    }

    private bool InitBind()
    {
        BindText(typeof(PlayerText));

        return true;
    }
    #endregion
}
