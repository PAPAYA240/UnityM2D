using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using UnityEngine;
using System.ComponentModel;

public class PlayerInfo
{
    [XmlAttribute]
    public int ID;
    [XmlAttribute]
    public int nameID;
    [XmlAttribute]
    public string illustPath;
    [XmlAttribute]
    public string battleIconPath;
    [XmlAttribute]
    public string spine;
    [XmlAttribute]
    public string aniIdle;
    [XmlAttribute]
    public string aniIdleSkin;
    [XmlAttribute]
    public string aniWorking;
    [XmlAttribute]
    public string aniWorkingSkin;
    [XmlAttribute]
    public string aniAttack;
    [XmlAttribute]
    public string aniAttackSkin;
    [XmlAttribute]
    public string aniWalk;
    [XmlAttribute]
    public string aniWalkSkin;
    [XmlAttribute]
    public string aniSweat;
    [XmlAttribute]
    public string aniSweatSkin;
    [XmlAttribute]
    public int maxhp;
    [XmlAttribute]
    public int atk;
    [XmlArray]
    public List<int> attackTexts = new List<int>();
    [XmlAttribute]
    public string promotion;
}

[Serializable, XmlRoot("ArrayOfPlayerData")]
public class PlayerDataLoader : ILoader<int, PlayerInfo>
{
    [XmlElement("PlayerInfo")]
    public List<PlayerInfo> _characterDatas = new List<PlayerInfo>();

    public Dictionary<int, PlayerInfo> MakeDic()
    {
        Dictionary<int, PlayerInfo> dic = new Dictionary<int, PlayerInfo>();

        foreach (PlayerInfo data in _characterDatas)
            dic.Add(data.ID, data);

        return dic;
    }


    public bool Validate()
    {
        return true;
    }
}