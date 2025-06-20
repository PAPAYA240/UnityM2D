using System.Data;
using System;
using UnityEngine;
using UnityEngine.Playables;
using static Defines;
using System.IO;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Data/CharacterData")]
public class CharactorData : ScriptableObject
{
    public string Name;
    public JobType Job;
    public IAttackStrategy attackStrategy;

    public int Level;
    public float LevelCount;

    public GameObject weaponPrefab;
    public int Hp = 100;
    public int MaxHp = 100;
    public int Hill;
    public int Exp;
    public int AttackPower = 10;

    public int Money = 1000000;

    public int BulletSpeed = 10;
    public int AttackSpeed = 7;
    public int Speed = 5;
}


public class GameManagerEx
{
    private CharactorData _gameData;

    public GameManagerEx(CharactorData data)
    {
        _gameData = data;
    }

    public CharactorData CurrentCharacterData { get { return _gameData; } }

    public string Name
        {
            get { return _gameData.Name; }
            set { _gameData.Name = value; }
        }
        public JobType JobTitle
        {
            get { return _gameData.Job; }
            set { _gameData.Job = value;  }
        }

        public int Hp
        {
            get { return _gameData.Hp; }
            set { _gameData.Hp = Mathf.Clamp(value, 0, MaxHp); }
        }

        public int MaxHp
        {
            get { return _gameData.MaxHp; }
            set { _gameData.MaxHp = value; RefreshStatCollections(); }
        }
     
        public int HillHp
        {
            get { return _gameData.Hill; }
            set { _gameData.Hill = value; RefreshStatCollections(); }
        }
        public int ExpHp
        {
            get { return _gameData.Exp; }
            set { _gameData.Exp = value; RefreshStatCollections(); }
        }
        public int Money
        {
            get { return _gameData.Money; }
            set { _gameData.Money = value; RefreshStatCollections(); }
        }
        public int AttackSpeed
        {
            get { return _gameData.AttackSpeed; }
            set { _gameData.AttackSpeed = value; RefreshStatCollections(); }
        }
        public int Speed
        {
            get { return _gameData.Speed; }
            set { _gameData.Speed = value; RefreshStatCollections(); }
        }
        public int BulletSpeed
        {
            get { return _gameData.BulletSpeed; }
            set { _gameData.BulletSpeed = value; RefreshStatCollections(); }
        }
        public int AttackPower
        {
            get { return _gameData.AttackPower; }
            set { _gameData.AttackPower = value; RefreshStatCollections(); }
        }
        public int Level
        {
            get { return _gameData.Level; }
            set { _gameData.Level = value; RefreshStatCollections(); }
        }
        public int LevelCount
        {
            get { return _gameData.Level; }
            set { _gameData.LevelCount = value; RefreshStatCollections(); }
        }
        public float HpPercent { get { return Hp * 100 / (float)MaxHp; } }

       //public int Attack
       //{
       //   return 0;
       //}

        //public float BossCoolTimePercent
        //{
        //    get
        //    {
        //        float percent = Managers.Data.Start.cooltimePercent;
        //
        //        float incPercent = 0.0f;
        //        foreach (StatData statData in Managers.Data.Stats.Values)
        //            incPercent += GetStat(statData.type) * statData.cooltimePercent;
        //
        //        return percent + incPercent;
        //    }
        //}

        public int GetStat(StatType type)
        {
            switch (type)
            {
                case Defines.StatType.MaxHp:
                    return MaxHp;
                case StatType.Attackbility:
                    return AttackPower;
                case StatType.Exp:
                    return ExpHp;
            }
            return 0;
        }

        //public PlayerState GetPlayerState(Job type)
        //{
        //    return _gameData.Players[(int)type];
        //}

        public void RefreshStatCollections()
        {
            
        }
    }
