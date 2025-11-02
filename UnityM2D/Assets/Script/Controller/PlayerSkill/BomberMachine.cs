using UnityEngine;


public class BomberMachine : Skill
{
    private GameObject _bomber = null;

    private const int MAX_BOMBER_COUNT = 10;
    private const float INTERVAL_TIME = 0.5f;

    private Vector3 _spawnPosition = Vector3.zero;

    private float currentTime = 0f;
    const int usedSkillCnt = 5;
    private int usedSkill = 0;

    public override bool Init()
    {
        if (_init)
            return false;

        if (!CreatePrefab())
            return false;
        
        return (_init = true);
    }

    public override bool ExecuteSkill(GameObject attacker, GameObject target)
    {
        if (usedSkill >= usedSkillCnt)
        {
            Reset();
            return true;
        }

        currentTime += Time.deltaTime;
        if (INTERVAL_TIME <= currentTime)
        {
            currentTime = 0;
            LaunchBomb(target);
        }
        return false;
    }

    private void LaunchBomb(GameObject target)
    {
        GameObject bomber = Managers.ObjectPoolManager.GetObjectKey(_bomber, _spawnPosition, Quaternion.identity);
        Bomber bomberScript = bomber?.GetComponent<Bomber>();

        if (bomberScript != null)
            bomberScript.UseBomber(this.gameObject, target.gameObject, 3f);

        usedSkill++;
    }

    private void Reset()
    {
        usedSkill = 0;
        currentTime = 0;
    }

    #region Init
    private bool CreatePrefab()
    {
        GameObject bomberSpawner = Managers.Resource.Instantiate("Prefab/Weapon/BomberSpawner");
        if (bomberSpawner == null)
            return false;

        this.transform.position = bomberSpawner.transform.position;
        Destroy(bomberSpawner);

        _bomber = Managers.Resource.Instantiate("Prefab/Weapon/Bomber");
        if (_bomber == null)
            return false;

        _bomber.SetActive(false);
        Managers.ObjectPoolManager.CreatePool<Bomber>(_bomber, MAX_BOMBER_COUNT);
        return true;
    }
    #endregion
}



