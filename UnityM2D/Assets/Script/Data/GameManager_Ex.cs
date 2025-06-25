using System;
using UnityEngine;
using static Defines;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;

[System.Serializable]
public class CharacterData : ScriptableObject
{
	[XmlAttribute]
    public string Name = "None";
	[XmlAttribute]
    public string myAnimControllerPath;
	[XmlAttribute]
    public int Level;
	[XmlAttribute]
    public int LevelCount;

	[XmlAttribute]
    public int Hp = 100;
	[XmlAttribute]
    public int MaxHp = 100;
	[XmlAttribute]
    public int Hill;
	[XmlAttribute]
    public int Exp;
	[XmlAttribute]
    public int AttackPower = 10;
	[XmlAttribute]
    public int Money = 1000000;

	[XmlAttribute]
    public int BulletSpeed = 10;
	[XmlAttribute]
    public int AttackSpeed = 7;
	[XmlAttribute]
    public int Speed = 5;
}
[System.Serializable]

public class PlayerData : CharacterData
{
	[XmlAttribute]
    public JobType jobType;

	[XmlAttribute]
    public int LevelCountMax = 100;
}

[System.Serializable]
public class MonsterData : CharacterData
{
	[XmlAttribute]
    public EnemyType enemyType; 

}

public interface ICharacterManager
{
    CharacterData GetCurrentCharacterData();
    void SetCurrentCharacterData(CharacterData data); // 필요하다면 설정도 가능
}

public class CharacterManager<T> : ICharacterManager where T : CharacterData
{
    T _gameData;

    public T Data { get { return _gameData; } set { _gameData = value; } }
  
    public CharacterData GetCurrentCharacterData()
    {
        return _gameData; // T는 CharacterData의 자식이므로 안전하게 업캐스팅
    }

    public void SetCurrentCharacterData(CharacterData data)
    {
        // 중요: 다운캐스팅이 필요한 경우 안전하게 처리해야 합니다.
        // 현재 매니저가 특정 T를 다루는데, 다른 타입의 CharacterData를 받을 수 있으므로 주의!
        if (data is T specificData) // 현재 T 타입에 맞는 데이터인지 확인
        {
            _gameData = specificData;
        }
        else
            Debug.LogError($"Attempted to set incompatible data type {data.GetType().Name} to CharacterManager<{typeof(T).Name}>.");
    }

    public void ChangeData(T data)
    {
        if (data is PlayerData otherPlayerData)
        {
            PlayerCopyFrom(otherPlayerData);
        }
        else if (data is MonsterData otherMonsterData)
        {
            EnemyCopyFrom(otherMonsterData);
        }
    }
    protected void CopyFrom(CharacterData _origin)
    {
        if (_origin == null) return;
        _gameData.Name = _origin.Name;
        _gameData.myAnimControllerPath = _origin.myAnimControllerPath;
        _gameData.Level = _origin.Level;
        _gameData.LevelCount = _origin.LevelCount;
        _gameData.Hp = _origin.Hp;
        _gameData.MaxHp = _origin.MaxHp;
        _gameData.Hill = _origin.Hill;
        _gameData.Exp = _origin.Exp;
        _gameData.AttackPower = _origin.AttackPower;
        _gameData.Money = _origin.Money;
        _gameData.BulletSpeed = _origin.BulletSpeed;
        _gameData.AttackSpeed = _origin.AttackSpeed;
        _gameData.Speed = _origin.Speed;
    }

    public void PlayerCopyFrom(PlayerData _data)
    {
        Managers.DataManager.Players.TryGetValue(String.Format($"Player"), out PlayerData originData);

        _data.jobType = originData.jobType;

        _data.LevelCountMax = originData.LevelCountMax;

        CopyFrom(originData);
    }

    public void EnemyCopyFrom(MonsterData _data)
    {
        Managers.DataManager.Enemys.TryGetValue(String.Format($"{_data.enemyType}"), out MonsterData originData);

        _data.enemyType = originData.enemyType;

        CopyFrom(originData);
    }

    #region Getter Setter
    public string Name
    {
        get { return _gameData?.Name ?? "N/A"; } 
        set { if (_gameData != null) _gameData.Name = value; }
    }

    public string MyAnimControllerPath 
    {
        get { return _gameData?.myAnimControllerPath; }
        set { if (_gameData != null) _gameData.myAnimControllerPath = value; }
    }

    public int Level
    {
        get { return _gameData?.Level ?? 0; }
        set { if (_gameData != null) _gameData.Level = value; }
    }

    public int LevelCount
    {
        get { return _gameData?.LevelCount ?? 0; }
        set { if (_gameData != null) _gameData.LevelCount = value; }
    }

    public int LevelCountMax
    {
        get { return LevelCountMax;  }
        set 
        { 
            LevelCountMax = Level * value;
            LevelCount = 0;
            Level += 1;
         }
    }

    public int Hp
    {
        get { return _gameData?.Hp ?? 0; }
        set { if (_gameData != null) _gameData.Hp = value; }
    }

    public int MaxHp
    {
        get { return _gameData?.MaxHp ?? 0; }
        set { if (_gameData != null) { _gameData.MaxHp = value; RefreshStatCollections(); } }
    }

    public int Hill 
    {
        get { return _gameData?.Hill ?? 0; }
        set { if (_gameData != null) { _gameData.Hill = value; RefreshStatCollections(); } }
    }

    public int Exp
    {
        get { return _gameData?.Exp ?? 0; }
        set { if (_gameData != null) { _gameData.Exp = value; RefreshStatCollections(); } }
    }

    public int AttackPower
    {
        get { return _gameData?.AttackPower ?? 0; }
        set { if (_gameData != null) { _gameData.AttackPower = value; RefreshStatCollections(); } }
    }

    public int Money
    {
        get { return _gameData?.Money ?? 0; }
        set { if (_gameData != null) { _gameData.Money = value; } }
    }

    public int BulletSpeed 
    {
        get { return _gameData?.BulletSpeed ?? 0; }
        set { if (_gameData != null) { _gameData.BulletSpeed = value; RefreshStatCollections(); } }
    }

    public int AttackSpeed 
    {
        get { return _gameData?.AttackSpeed ?? 0; }
        set { if (_gameData != null) { _gameData.AttackSpeed = value; RefreshStatCollections(); } }
    }

    public int Speed
    {
        get { return _gameData?.Speed ?? 0; }
        set { if (_gameData != null) { _gameData.Speed = value; RefreshStatCollections(); } }
    }

    private void RefreshStatCollections()
    {
    }
    #endregion

  
    //#region Save & Load	
    //public string _path = Application.persistentDataPath + "/SaveData.json";

    //public void SaveGame()
    //{
    //    string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
    //    File.WriteAllText(_path, jsonStr);
    //    Debug.Log($"Save Game Completed : {_path}");
    //}

    //public bool LoadGame()
    //{
    //    if (File.Exists(_path) == false)
    //        return false;

    //    string fileStr = File.ReadAllText(_path);
    //    GameData data = JsonUtility.FromJson<GameData>(fileStr);
    //    if (data != null)
    //    {
    //        Managers.Game.SaveData = data;
    //    }

    //    Debug.Log($"Save Game Loaded : {_path}");
    //    return true;
    //}
    //#endregion
}

#region Data Loader
[Serializable, XmlRoot("ArrayOfPlayerData")]
public class PlayerDataLoader : ILoader<string, PlayerData>
{
    [XmlElement("PlayerData")]
    public List<PlayerData> _characterDatas = new List<PlayerData>();

    public Dictionary<string, PlayerData> MakeDic()
    {
        Dictionary<string, PlayerData> dic = new Dictionary<string, PlayerData>();

        foreach (PlayerData data in _characterDatas)
            dic.Add(data.Name, data);

        return dic;
    }

    public bool Validate()
    {
        return true;
    }
}

[Serializable, XmlRoot("ArrayOfMonsterData")]
public class EnemyDataLoader : ILoader<string, MonsterData>
{
    [XmlElement("MonsterData")]
    public List<MonsterData> _characterDatas = new List<MonsterData>();

    public Dictionary<string, MonsterData> MakeDic()
    {
        Dictionary<string, MonsterData> dic = new Dictionary<string, MonsterData>();

        foreach (MonsterData data in _characterDatas)
            dic.Add(data.Name, data);

        return dic;
    }

    public bool Validate()
    {
        return true;
    }
}
#endregion
