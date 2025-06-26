using System;
using System.Collections;
using UnityEngine;

public class BomberSkill : Skill
{
    GameObject Prefab = null;
    int objectCnt = 10;
    Vector3 startPosition = Vector3.zero;

    public override bool Init()
    {
        if (_init)
            return false;

        Prefab = Managers.Resource.Instantiate("Prefab/Weapon/Bomber");
        Managers.ObjectPoolManager.CreatePool<Bomber>(Prefab, objectCnt);

        return _init = true;
    }
 

    public override void ExecuteSkill(BaseController _attacker, BaseController _targeter)
    {
        GameObject bomber = Managers.ObjectPoolManager.GetObjectKey(Prefab, startPosition, Quaternion.identity);

        Bomber bomberScript = bomber.GetComponent<Bomber>();

        if(bomberScript != null)
            bomberScript.UseBomber(_attacker.gameObject, _targeter.gameObject, _attacker.data.Speed);
    }
}

