using System;
using System.Collections.Generic;
using UnityEngine;
using static Defines;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "무기 이름";
    public GameObject weaponPrefab;

    public Defines.WeaponType weaponType;
    // 어떤 공격 전략을 사용할 것인가?
    [Header("Attack Strategy Settings")]
    public IAttackStrategy attackStrategyType; 

    public int baseDamage = 10;
    public float attackRange = 1.5f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;
    public float attackSpeed = 1.0f;

    public Vector2 reactionTime = new Vector2(0.1f, 0.2f);
}

public class WeaponLoader 
{
    private Dictionary<WeaponType, WeaponData> _weaponDataDictionary = new Dictionary<WeaponType, WeaponData>();

    public void Init()
    {
        WeaponData[] allWeaponData = Resources.LoadAll<WeaponData>("WeaponData");

        foreach (WeaponData data in allWeaponData)
        {
            if (System.Enum.TryParse(data.name, out WeaponType id))
            {
                if (!_weaponDataDictionary.ContainsKey(id))
                {
                    _weaponDataDictionary.Add(id, data);
                }
                else
                {
                    Debug.LogWarning($"중복된 WeaponData ID: {id} ({data.name})");
                }
            }
            else
            {
                Debug.LogWarning($"WeaponData 이름 '{data.name}'이(가) WeaponTypeID enum과 일치하지 않습니다.");
            }
        }
    }

    public WeaponData GetWeaponData(WeaponType id)
    {
        if (_weaponDataDictionary.TryGetValue(id, out WeaponData data))
        {
            return data;
        }

        Debug.LogWarning($"WeaponData ID '{id}'를 찾을 수 없습니다.");
        return null;
    }
}
