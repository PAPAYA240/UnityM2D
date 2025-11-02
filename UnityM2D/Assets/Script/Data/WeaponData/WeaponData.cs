using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using static Defines;

#region Weapon
[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "무기 이름";
    public GameObject weaponPrefab;

    public WeaponType weaponType;
    // 어떤 공격 전략을 사용할 것인가?
    [Header("Attack Strategy Settings")]
    public IAttackStrategy attackStrategyType; 

    public int baseDamage = 10;
    public float attackRange = 1.5f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;
    public float attackSpeed = 1.0f;

    public int openWeaponLimit = 0;
    public int addedPrice = 0;
    public int addedAttack = 0;
    public Vector2 reactionTime = new Vector2(0.1f, 0.2f);
}

public class WeaponDataLoader : MonoBehaviour
{
    private Dictionary<string, WeaponData> weaponDataDictionary = new Dictionary<string, WeaponData>();
    private List<WeaponData> allWeaponData = new List<WeaponData>();

    void Start()
    {
        Init();
    }

    public void Init()
    {
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("WeaponData");

        if (jsonFiles.Length == 0)
            return;

        foreach (var jsonFile in jsonFiles)
        {
            try
            {
                // JSON을 WeaponDataJson으로 파싱
                WeaponDataJson jsonData = JsonConvert.DeserializeObject<WeaponDataJson>(jsonFile.text);

                if (jsonData == null)
                {
                    Debug.LogError($"파싱 실패: {jsonFile.name}");
                    continue;
                }

                Debug.Log($"✓ 파싱 성공: {jsonData.weaponName}");

                // WeaponData ScriptableObject로 변환
                WeaponData weaponData = ConvertToScriptableObject(jsonData);

                // 리스트에 추가
                allWeaponData.Add(weaponData);

                // Dictionary에 추가 (weaponName을 키로 사용)
                if (!weaponDataDictionary.ContainsKey(weaponData.weaponName))
                {
                    weaponDataDictionary.Add(weaponData.weaponName, weaponData);
                }
                else
                {
                    Debug.LogWarning($"중복된 무기 이름: {weaponData.weaponName}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"오류 발생 ({jsonFile.name}): {e.Message}");
                Debug.LogError($"스택: {e.StackTrace}");
            }
        }
    }

    // JSON 데이터를 ScriptableObject로 변환
    private WeaponData ConvertToScriptableObject(WeaponDataJson jsonData)
    {
        WeaponData weaponData = ScriptableObject.CreateInstance<WeaponData>();

        weaponData.weaponName = jsonData.weaponName;
        weaponData.baseDamage = jsonData.baseDamage;
        weaponData.attackRange = jsonData.attackRange;
        weaponData.projectileSpeed = jsonData.projectileSpeed;
        weaponData.attackSpeed = jsonData.attackSpeed;
        weaponData.openWeaponLimit = jsonData.openWeaponLimit;
        weaponData.addedPrice = jsonData.addedPrice;
        weaponData.addedAttack = jsonData.addedAttack;
        weaponData.reactionTime = new Vector2(jsonData.reactionTime.x, jsonData.reactionTime.y);

        // WeaponType Enum 파싱
        if (System.Enum.TryParse(jsonData.weaponType, out WeaponType type))
        {
            weaponData.weaponType = type;
        }
        else
        {
            Debug.LogWarning($"알 수 없는 WeaponType: {jsonData.weaponType}, 기본값 사용");
        }

        if (!string.IsNullOrEmpty(jsonData.weaponPrefab))
        {
            weaponData.weaponPrefab = Resources.Load<GameObject>(jsonData.weaponPrefab);
            if (weaponData.weaponPrefab == null)
            {
                Debug.LogWarning($"weaponPrefab 로드 실패: {jsonData.weaponPrefab}");
            }
            else
            {
                Debug.Log($"✓ weaponPrefab 로드 성공: {jsonData.weaponPrefab}");
            }
        }

        if (!string.IsNullOrEmpty(jsonData.projectilePrefab))
        {
            weaponData.projectilePrefab = Resources.Load<GameObject>(jsonData.projectilePrefab);
            if (weaponData.projectilePrefab == null)
            {
                Debug.LogWarning($"projectilePrefab 로드 실패: {jsonData.projectilePrefab}");
            }
        }

        return weaponData;
    }

    // 이름으로 무기 찾기
    public WeaponData GetWeaponDataByName(string weaponName)
    {
        if (weaponDataDictionary.TryGetValue(weaponName, out WeaponData weapon))
        {
            return weapon;
        }

        Debug.LogWarning($"무기를 찾을 수 없습니다: {weaponName}");
        return null;
    }

    // 타입으로 무기 찾기
    public WeaponData GetWeaponData(WeaponType type)
    {
        return allWeaponData.Find(w => w.weaponType == type);
    }
}

// JSON 직렬화 전용 클래스
[System.Serializable]
public class WeaponDataJson
{
    public string weaponName;
    public string weaponPrefab;
    public string weaponType;
    public string attackStrategyType;
    public int baseDamage;
    public float attackRange;
    public string projectilePrefab;
    public float projectileSpeed;
    public float attackSpeed;
    public int openWeaponLimit;
    public int addedPrice;
    public int addedAttack;
    public Vector2Data reactionTime;
}

[System.Serializable]
public class Vector2Data
{
    public float x;
    public float y;
}


#endregion

#region Pet
[Serializable]
public class PetDataJson
{
    public string petName;      
    public string petType;  
    public int damage;
    public float duration;
    public int price;
    public string petPrefab; 
}

[Serializable]
public class PetDataListContainer
{
    public List<PetDataJson> Pets;
}

public class PetDataLoad : MonoBehaviour
{
    private Dictionary<string, PetData> petDataDictionary = new Dictionary<string, PetData>();
    private List<PetData> allPetData = new List<PetData>();

    private const string JSON_FILE_NAME = "PetData/Pet"; 

    void Start()
    {
        Init();
    }

    public void Init()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(JSON_FILE_NAME);

        if (jsonTextAsset == null)
        {
            Debug.LogError($"❌ JSON 파일 로드 실패: Resources 폴더에 '{JSON_FILE_NAME}.json'을 찾을 수 없습니다.");
            return;
        }

        try
        {
            PetDataListContainer container = JsonConvert.DeserializeObject<PetDataListContainer>(jsonTextAsset.text);

            if (container == null || container.Pets == null)
            {
                Debug.LogError($"파싱 실패: {JSON_FILE_NAME}. JSON 구조를 확인하세요.");
                return;
            }

            foreach (var jsonData in container.Pets)
            {
                PetData petData = ConvertToScriptableObject(jsonData);

                allPetData.Add(petData);

                if (!petDataDictionary.ContainsKey(petData.petName))
                {
                    petDataDictionary.Add(petData.petName, petData);
                }
                else
                {
                    Debug.LogWarning($"중복된 펫 이름: {petData.petName}");
                }
            }

            Debug.Log($"=== 초기화 완료: **{allPetData.Count}**개의 펫 로드됨 ===");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ JSON 역직렬화 중 오류 발생 ({JSON_FILE_NAME}): {e.Message}");
            Debug.LogError($"스택: {e.StackTrace}");
        }
    }

    private PetData ConvertToScriptableObject(PetDataJson jsonData)
    {
        // ScriptableObject 인스턴스 생성
        PetData petData = ScriptableObject.CreateInstance<PetData>();

        petData.petName = jsonData.petName;
        petData.damage = jsonData.damage;
        petData.duration = jsonData.duration;
        petData.price = jsonData.price;
        petData.petPrefab = jsonData.petPrefab;
        // PetType Enum 파싱
        if (System.Enum.TryParse(jsonData.petType, out PetType type))
        {
            petData.petType = type;
        }
        else
        {
            Debug.LogWarning($"알 수 없는 PetType: {jsonData.petType}, 기본값 사용");
        }
        return petData;
    }

    // 이름으로 펫 찾기
    public PetData GetPetDataByName(string petName)
    {
        if (petDataDictionary.TryGetValue(petName, out PetData pet))
            return pet;

        Debug.LogWarning($"펫을 찾을 수 없습니다: {petName}");
        return null;
    }

    // 타입으로 펫 찾기 (첫 번째 일치하는 펫 반환)
    public PetData GetPetDataByType(PetType type)
    {
        return allPetData.Find(p => p.petType == type);
    }
}
#endregion

#region Airplane
[Serializable]
public class AirplaneDataJson
{
    public string airplaneName;
    public string airplaneType;
    public int damage;
    public float duration;
    public int price;
    public string airplanePrefab;
}

[Serializable]
public class AirplaneDataListContainer
{
    public List<AirplaneDataJson> Airplanes;
}

public class AirplaneDataLoad : MonoBehaviour
{
    private Dictionary<string, AirplaneData> airplaneDataDictionary = new Dictionary<string, AirplaneData>();
    private List<AirplaneData> allAirplaneData = new List<AirplaneData>();

    private const string JSON_FILE_NAME = "AirplaneData/AirplaneData";

    void Start()
    {
        Init();
    }

    public void Init()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(JSON_FILE_NAME);

        if (jsonTextAsset == null)
        {
            Debug.LogError($"❌ JSON 파일 로드 실패: Resources 폴더에 '{JSON_FILE_NAME}.json'을 찾을 수 없습니다.");
            return;
        }

        try
        {
            AirplaneDataListContainer container = JsonConvert.DeserializeObject<AirplaneDataListContainer>(jsonTextAsset.text);

            if (container == null || container.Airplanes == null)
            {
                Debug.LogError($"파싱 실패: {JSON_FILE_NAME}. JSON 구조를 확인하세요.");
                return;
            }

            foreach (var jsonData in container.Airplanes)
            {
                AirplaneData petData = ConvertToScriptableObject(jsonData);

                allAirplaneData.Add(petData);

                if (!airplaneDataDictionary.ContainsKey(petData.airplaneName))
                {
                    airplaneDataDictionary.Add(petData.airplaneName, petData);
                }
                else
                {
                    Debug.LogWarning($"중복된 펫 이름: {petData.airplaneName}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ JSON 역직렬화 중 오류 발생 ({JSON_FILE_NAME}): {e.Message}");
            Debug.LogError($"스택: {e.StackTrace}");
        }
    }

    private AirplaneData ConvertToScriptableObject(AirplaneDataJson jsonData)
    {
        // ScriptableObject 인스턴스 생성
        AirplaneData airplaneData = ScriptableObject.CreateInstance<AirplaneData>();

        airplaneData.airplaneName = jsonData.airplaneName;
        airplaneData.damage = jsonData.damage;
        airplaneData.duration = jsonData.duration;
        airplaneData.price = jsonData.price;
        airplaneData.airplanePrefab = jsonData.airplanePrefab;
        // PetType Enum 파싱
        if (System.Enum.TryParse(jsonData.airplaneType, out AirplaneType type))
        {
            airplaneData.airplaneType = type;
        }
        else
        {
            Debug.LogWarning($"알 수 없는 PetType: {jsonData.airplaneType}, 기본값 사용");
        }
        return airplaneData;
    }

    // 이름으로 펫 찾기
    public AirplaneData GetAirplaneDataByName(string petName)
    {
        if (airplaneDataDictionary.TryGetValue(petName, out AirplaneData pet))
            return pet;

        Debug.LogWarning($"펫을 찾을 수 없습니다: {petName}");
        return null;
    }

    // 타입으로 펫 찾기 (첫 번째 일치하는 펫 반환)
    public AirplaneData GetAirplaneDataByType(AirplaneType type)
    {
        return allAirplaneData.Find(p => p.airplaneType == type);
    }
}
#endregion