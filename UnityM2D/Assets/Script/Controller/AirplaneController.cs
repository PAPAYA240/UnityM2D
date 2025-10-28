using System.Collections;
using UnityEngine;
using static Defines;

public class Airplane : BaseController
{
    enum GameObjects
    {
        BulletPoint,
    }

    private CharacterManager<AirplaneData> airplaneDataManager = new CharacterManager<AirplaneData> ();
    public AirplaneData airplaneData => data as AirplaneData;
    private AirplaneType airplaneType = AirplaneType.Basic_Airplane;
    private AirplaneType nextAirplaneType = AirplaneType.Basic_Airplane;

    GameObject bulletprefab = null;

    private Vector3 startPosition = Vector3.zero;
    private float waitAttackTime = 3;
    private float waitReadyTime = 2;

    private float _bulletDuration = 1.0f;

    protected override ICharacterManager GetCharacterDataManager()
    {
        return airplaneDataManager;
    }

    private void Start() => Init();
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        GameObject playerObj = GameObject.Find(strPlayerObject);
        if (playerObj == null)
            return false;

        BindObject(typeof(GameObjects));

        TargetObject = GameObject.Find(strEnemyObject); // 공격할 적

        bulletprefab = Managers.Resource.Instantiate("WeaponPrefab/Bullet", this.transform);
        int bulletCount = 30;
        Managers.ObjectPoolManager.CreatePool<Bullet>(bulletprefab, bulletCount, this.transform);

        GameObject spawnArea = Managers.Resource.Instantiate(strAirplaneSpawnAreaPath);
        if (spawnArea == null)
            return false;
        this.transform.position = startPosition = spawnArea.transform.position;

        LoadData(AirplaneType.Basic_Airplane);

        StartCoroutine(AutoAirplaneAttack());

        return true;
    }
    private void AutoFire()
    {
         Vector3 startPos = GetObject(GameObjects.BulletPoint).gameObject.transform.position;
         GameObject bullet = Managers.ObjectPoolManager.GetObjectKey(bulletprefab, startPos, transform.rotation);

         Bullet bulletScript = bullet.GetComponent<Bullet>();
         StartCoroutine(bulletScript.Fire());
    }

    private IEnumerator AutoFireCoroutine()
    {
        while (true)
        {
            AutoFire();
            yield return new WaitForSeconds(_bulletDuration);
        }
    }
    private IEnumerator AutoAirplaneAttack()
    {
        while(true)
        {
            yield return new WaitForSeconds(waitReadyTime);

            Coroutine autoFireCoroutine = StartCoroutine(AutoFireCoroutine());

            this.transform.position = startPosition;
            Vector3 midPosition = startPosition;
            midPosition.x += 2f;

            const float interval = 0.1f;
            while (Vector3.Distance(this.transform.position, midPosition) > interval)
            {
                transform.position = Managers.TransformManager.MoveToTarget(this.transform.position, midPosition, data.Speed);
                yield return null;
            }
            transform.position = midPosition;

            yield return new WaitForSeconds(waitAttackTime);

            StopCoroutine(autoFireCoroutine);

            Vector3 endPosition = midPosition;
            endPosition.x += 6;
            while (Vector3.Distance(this.transform.position, endPosition) > interval)
            {
                transform.position = Managers.TransformManager.MoveToTarget(this.transform.position, endPosition, data.Speed);
                yield return null;
            }
            transform.position = startPosition;
        }
    }

    public AirplaneType UpgradeAirplane(BaseController _owner)
    {
        ChangeAirplane(_owner);

        nextAirplaneType = airplaneType + 1;
        if (nextAirplaneType >= AirplaneType.Epic_Airplane)
            nextAirplaneType = AirplaneType.Epic_Airplane;

        return nextAirplaneType;
    }
    private void ChangeAirplane(BaseController _owner)
    {
        if (data.Money > _owner.data.Money)
            return;

        if (airplaneType >= AirplaneType.Epic_Airplane)
            return;

        _owner.data.Money -= data.Money;

        LoadData((AirplaneType)((int)airplaneType + 1));
    }

    private void LoadData(AirplaneType _type)
    {
        if (airplaneDataManager.Data == null)
            airplaneDataManager.Data = new AirplaneData();

        airplaneData.airplaneType = _type;
        airplaneDataManager.ChangeData(airplaneData);
    }
}
