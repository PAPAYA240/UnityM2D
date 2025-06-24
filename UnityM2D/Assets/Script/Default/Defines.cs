using System.ComponentModel.Design.Serialization;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class Defines
{
    public const string strEnemyObject = "@Enemy";
    public const string strPlayerObject = "Player";
    public const string strPlayerArea = "@PlayerArea";
    public const string strEnemyArea = "@EnemyArea";
    public const string strManagers = "@Managers";


    public enum Input
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
    }

    public enum Scene
    {
        None,
        InGame,
    }
    public enum BattleType
    {
        Player,
        Enemy,
    }

    public enum AnimState
    {
        None,
        Idle,
        Walk,
        Run,
        Jump,
        Attack,
        Dead,
    }

    public enum JobType
    {
        None,
        Knight,
    }

    public enum EnemyType
    {
        None,
        Zombi,
        Skeleton,

        // ======= Boss Type ========== 
        // TODO : Boss Type은 여기서부터 UI 순서대로 나열.
        Zombi_Boss,
        Skeleton_Boss,
    }

    public enum UIType
    {
        None,
        Title,
        Exp,
    }

    public enum WeaponType
    {
        None_Weapon,
        Basic_Weapon,
        Advanced_Weapon,
        Elite_Weapon,
        Epic_Weapon,
        Mythic_Weapon,
    }
    public enum StatType
    {
        MaxHp ,
        Exp,
        Attackbility,
    }
}
