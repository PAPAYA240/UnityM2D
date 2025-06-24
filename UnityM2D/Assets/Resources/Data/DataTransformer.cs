using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Defines;

public class DataTransformer : EditorWindow
{
    [MenuItem("Tools/RemoveSaveData")]
    public static void RemoveSaveData()
    {
        string path = Application.persistentDataPath + "/SaveData.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("SaveFile Deleted");
        }
        else
        {
            Debug.Log("No SaveFile Detected");
        }
    }

    [MenuItem("Tools/ParseExcel")]
    public static void ParseExcel()
    {
        ParsePlayerData();
        ParseEnemyData();
    }

    static void ParsePlayerData()
    {
        List<PlayerData> playerDatas = new List<PlayerData>();

        #region ExcelData
        string[] lines = Resources.Load<TextAsset>($"Data/Excel/PlayerData").text.Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            PlayerData playerData = new PlayerData()
            {
                Name = row[0],
                myAnimControllerPath = row[1],
                Level = int.Parse(row[2]),
                LevelCount = int.Parse(row[3]),

                Hp = int.Parse(row[4]),
                MaxHp = int.Parse(row[5]),
                Hill = int.Parse(row[6]),
                Exp = int.Parse(row[7]),
                AttackPower = int.Parse(row[8]),
                Money = int.Parse(row[9]),

                BulletSpeed = int.Parse(row[10]),
                AttackSpeed = int.Parse(row[11]),
                Speed = int.Parse(row[12]),
                jobType = (JobType)Enum.Parse(typeof(JobType), row[13], ignoreCase: true),
            };
            playerDatas.Add(playerData);
        }
        #endregion

        string xmlString = ToXML(playerDatas);
        File.WriteAllText($"{Application.dataPath}/Resources/Data/PlayerData.xml", xmlString);
        AssetDatabase.Refresh();
    }

    static void ParseEnemyData()
    {
        List<MonsterData> EnemysDatas = new List<MonsterData>();

        #region ExcelData
        string[] lines = Resources.Load<TextAsset>($"Data/Excel/EnemyData").text.Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            MonsterData EnemyData = new MonsterData()
            {
                Name = row[0],
                myAnimControllerPath = row[1],
                Level = int.Parse(row[2]),
                LevelCount = int.Parse(row[3]),

                Hp = int.Parse(row[4]),
                MaxHp = int.Parse(row[5]),
                Hill = int.Parse(row[6]),
                Exp = int.Parse(row[7]),
                AttackPower = int.Parse(row[8]),
                Money = int.Parse(row[9]),

                BulletSpeed = int.Parse(row[10]),
                AttackSpeed = int.Parse(row[11]),
                Speed = int.Parse(row[12]),
                enemyType = (EnemyType)Enum.Parse(typeof(EnemyType), row[13], ignoreCase: true),
            };
            EnemysDatas.Add(EnemyData);
        }
        #endregion

        string xmlString = ToXML(EnemysDatas);
        File.WriteAllText($"{Application.dataPath}/Resources/Data/EnemyData.xml", xmlString);
        AssetDatabase.Refresh();
    }

    #region XML Parse
    public sealed class ExtentedStringWriter : StringWriter
    {
        private readonly Encoding stringWriterEncoding;

        public ExtentedStringWriter(StringBuilder builder, Encoding desiredEncoding) : base(builder)
        {
            this.stringWriterEncoding = desiredEncoding;
        }

        public override Encoding Encoding
        {
            get
            {
                return this.stringWriterEncoding;
            }
        }
    }

    public static string ToXML<T>(T obj)
    {
        using (ExtentedStringWriter stringWriter = new ExtentedStringWriter(new StringBuilder(), Encoding.UTF8))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }
    }
#endregion
}
