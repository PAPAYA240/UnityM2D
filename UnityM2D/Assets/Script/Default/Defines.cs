using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class Defines
{
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
        Jombi,
        MagicJombi,
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
        MaxHp,
        Exp,
        Attackbility,
    }

}
