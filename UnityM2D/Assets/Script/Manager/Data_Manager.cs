using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public interface ILoader<Key, Item>
{
    Dictionary<Key, Item> MakeDic();
    bool Validate();
}

public class Data_Manager
{
   // public Dictionary<string, MonsterData> Stats { get; private set; }
    public Dictionary<string, PlayerData> Players { get; private set; }
    public Dictionary<string, MonsterData> Enemys { get; private set; }
    public Dictionary<string, PetData> Pets { get; private set; }

    public void Init()
    {
        Enemys = LoadXml<EnemyDataLoader, string, MonsterData>("EnemyData").MakeDic();
        Players = LoadXml<PlayerDataLoader, string, PlayerData>("PlayerData").MakeDic();
        Pets = LoadXml<PetDataLoader, string, PetData>("PetData").MakeDic();
    }

    private Item LoadSingleXml<Item>(string name)
    {
        XmlSerializer xs = new XmlSerializer(typeof(Item));
        TextAsset textAsset = Resources.Load<TextAsset>("Data/" + name);
        using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textAsset.text)))
            return (Item)xs.Deserialize(stream);
    }

    private Loader LoadXml<Loader, Key, Item>(string name) where Loader : ILoader<Key, Item>, new()
    {
        XmlSerializer xs = new XmlSerializer(typeof(Loader));
        TextAsset textAsset = Resources.Load<TextAsset>("Data/" + name);
        using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textAsset.text)))
            return (Loader)xs.Deserialize(stream);
    }
}

