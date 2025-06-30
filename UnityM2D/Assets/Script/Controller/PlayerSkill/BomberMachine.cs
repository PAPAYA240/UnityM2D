using UnityEngine;


public class BomberMachine : Skill
{
    private GameObject bomberPrefab = null;

    private int objectCnt = 10;
    private Vector3 startPosition = Vector3.zero;

    const float intervalTime = 0.5f;
    private float currentTime = 0f;
    const int usedSkillCnt = 5;
    private int usedSkill = 0;

    public override bool Init()
    {
        if (_init)
            return false;

        GameObject spawner = Managers.Resource.Instantiate("Prefab/Weapon/BomberSpawner");
        if(spawner != null ) 
        {
            this.transform.position = spawner.transform.position;
            Destroy( spawner );
        }

        bomberPrefab = Managers.Resource.Instantiate("Prefab/Weapon/Bomber");
        Managers.ObjectPoolManager.CreatePool<Bomber>(bomberPrefab, objectCnt);
        bomberPrefab.SetActive(false);
        
        return _init = true;
    }

    public override bool ExecuteSkill(GameObject _attacker, GameObject _targeter)
    {
        if (usedSkill >= usedSkillCnt)
        {
            usedSkill = 0;
            currentTime = 0;
            return true;
        }

        currentTime += Time.deltaTime;
        if (intervalTime <= currentTime)
        {
            currentTime = 0;

            GameObject bomber = Managers.ObjectPoolManager.GetObjectKey(bomberPrefab, startPosition, Quaternion.identity);
            Bomber bomberScript = bomber.GetComponent<Bomber>();
            if (bomberScript != null)
                bomberScript.UseBomber(this.gameObject, _targeter.gameObject, 3f);
            usedSkill++;
        }
        return false;
    }
}



