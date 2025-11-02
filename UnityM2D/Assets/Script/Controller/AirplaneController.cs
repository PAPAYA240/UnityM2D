using System.Collections;
using UnityEngine;
using static Defines;

public class Airplane : BaseController
{
    #region Enum

    private enum GameObjects
    {
        BulletPoint,
    }

    #endregion

    #region Constants

    private const float ATTACK_DURATION = 1.5f;
    private const float READY_DURATION = 2f;
    private const float BULLET_FIRE_INTERVAL = 1f;
    private const float MOVEMENT_THRESHOLD = 0.1f;
    private const float ATTACK_FORWARD_DISTANCE = 2f;
    private const float EXIT_FORWARD_DISTANCE = 6f;
    private const int BULLET_POOL_SIZE = 20;

    #endregion

    #region Properties

    public AirplaneData AirplaneData => data as AirplaneData;

    #endregion

    #region Private Fields

    private CharacterManager<AirplaneData> _airplaneDataManager = new CharacterManager<AirplaneData>();
    private AirplaneType _airplaneType = AirplaneType.Basic_Airplane;
    private Vector3 _spawnPosition = Vector3.zero;
    private GameObject _bulletPrefab;
    private Coroutine _autoFireCoroutine;
    private SpriteRenderer _spriteRenderer = null;
    #endregion

    #region Initialization
    private void Awake() => Init();

    public override bool Init()
    {
        if (!base.Init())
            return false;

        if (!InitializeBindings())
            return false;

        if (!InitializeAirplane())
            return false;

        StartCoroutine(AirplaneAttackSequence());

        return true;
    }

    protected override ICharacterManager GetCharacterDataManager()
    {
        return _airplaneDataManager;
    }

    private bool InitializeBindings()
    {
        BindObject(typeof(GameObjects));
        return true;
    }
    public override int GetAttackPower()
    {
        AirplaneData data = Managers.AirplaneLoader.GetAirplaneDataByType(_airplaneType);
        if (data == null)
            return 0;

        return data.damage;
    }
    private bool InitializeAirplane()
    {
        if (!InitializeTarget())
            return false;

        if (!InitializeBulletSystem())
            return false;

        if (!InitializeSpawnPosition())
            return false;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            return false;

        return true;
    }

    private bool InitializeTarget()
    {
        TargetObject = GameObject.Find(strEnemyObject);
        if (TargetObject == null)
            return false;
        return true;
    }

    private bool InitializeBulletSystem()
    {
        _bulletPrefab = Managers.Resource.Instantiate(strBulletPath, transform);
        if (_bulletPrefab == null)
            return false;

        Managers.ObjectPoolManager.CreatePool<Bullet>(_bulletPrefab, BULLET_POOL_SIZE, transform);
        return true;
    }

    private bool InitializeSpawnPosition()
    {
        GameObject spawnArea = Managers.Resource.Instantiate(strAirplaneSpawnAreaPath);
        if (spawnArea == null)
            return false;

        _spawnPosition = spawnArea.transform.position;
        transform.position = _spawnPosition;
        Destroy(spawnArea);

        return true;
    }

    #endregion

    #region Attack Sequence

    private IEnumerator AirplaneAttackSequence()
    {
        while (true)
        {
            AirplaneData data = Managers.AirplaneLoader.GetAirplaneDataByType(_airplaneType);
            if (data == null)
                yield return null;

            yield return new WaitForSeconds(data.duration); // 대기

            // 공격 위치로 이동
            yield return StartCoroutine(MoveToAttackPosition());

            // 공격 시작
            StartAutoFire();
            yield return new WaitForSeconds(ATTACK_DURATION);

            // 공격 종료
            StopAutoFire();

            // 퇴각
            yield return StartCoroutine(ExitFromScreen());
        }
    }

    private IEnumerator MoveToAttackPosition()
    {
        transform.position = _spawnPosition;
        Vector3 targetPosition = _spawnPosition + new Vector3(ATTACK_FORWARD_DISTANCE, 0, 0);

        yield return StartCoroutine(MoveToPosition(targetPosition));
    }

    private IEnumerator ExitFromScreen()
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = currentPosition + new Vector3(EXIT_FORWARD_DISTANCE, 0, 0);

        yield return StartCoroutine(MoveToPosition(targetPosition));

        transform.position = _spawnPosition;
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > MOVEMENT_THRESHOLD)
        {
            transform.position = Managers.TransformManager.MoveToTarget(
                transform.position,
                targetPosition,
                data.Speed
            );
            yield return null;
        }

        transform.position = targetPosition;
    }

    #endregion

    #region Fire System

    private void StartAutoFire()
    {
        if (_autoFireCoroutine != null)
        {
            StopCoroutine(_autoFireCoroutine);
        }

        _autoFireCoroutine = StartCoroutine(AutoFireCoroutine());
    }

    private void StopAutoFire()
    {
        if (_autoFireCoroutine != null)
        {
            StopCoroutine(_autoFireCoroutine);
            _autoFireCoroutine = null;
        }
    }

    private IEnumerator AutoFireCoroutine()
    {
        while (true)
        {
            FireBullet();

            yield return new WaitForSeconds(BULLET_FIRE_INTERVAL);
        }
    }

    private void FireBullet()
    {
        Transform bulletPoint = GetObject(GameObjects.BulletPoint).transform;
        GameObject bullet = Managers.ObjectPoolManager.GetObjectKey(
            _bulletPrefab,
            bulletPoint.position,
            transform.rotation
        );

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        Vector3 targetPosition = TargetObject.transform.position;
        if (bulletScript != null)
        {
            StartCoroutine(bulletScript.Fire(0.5f, targetPosition));
        }
    }

    #endregion

    #region Upgrade System
    public AirplaneType UpgradeAirplane(BaseController owner)
    {
        if (!CanUpgrade(owner))
            return _airplaneType;

        ProcessUpgrade(owner);

        return _airplaneType;
    }

    private bool CanUpgrade(BaseController owner)
    {
        AirplaneData data = Managers.AirplaneLoader.GetAirplaneDataByType(_airplaneType);
        if (data == null)
            return false;
        if (data.price > owner.data.Money)
        {
            Debug.Log("비행기 살 돈 없음");
            return false;
        }

        if (_airplaneType >= AirplaneType.Epic_Airplane)
        {
            Debug.Log("이미 네 비행기가 만렙임");
            return false;
        }
        return true;
    }

    private void ProcessUpgrade(BaseController owner)
    {
        AirplaneData data = Managers.AirplaneLoader.GetAirplaneDataByType(_airplaneType);
        owner.data.Money -= data.price;
        Debug.Log($"비행기 값으로 {data.price} 사라짐");

        LoadAirplaneData(_airplaneType);
        _airplaneType = _airplaneType + 1;
    }

    #endregion

    #region Data Management

    private void LoadAirplaneData(AirplaneType type)
    {
        AirplaneData data = Managers.AirplaneLoader.GetAirplaneDataByType(_airplaneType);
        if (_airplaneDataManager.Data == null)
            _airplaneDataManager.Data = new AirplaneData();

        _airplaneDataManager.ChangeData(data);

        LoadAirplaneSprite(data.airplanePrefab);
        _airplaneType = type;
    }
    private void LoadAirplaneSprite(string spritePath)
    {
        Sprite loadedSprite = Resources.Load<Sprite>(spritePath);

        if (loadedSprite != null && _spriteRenderer != null)
            _spriteRenderer.sprite = loadedSprite;
    }
    #endregion
}