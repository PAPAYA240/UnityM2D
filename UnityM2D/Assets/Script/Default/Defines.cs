
public class Defines
{
    // NAME
    public const string strEnemyObject = "@Enemy";
    public const string strPlayerObject = "Player";
    public const string strPlayerArea = "@PlayerArea";
    public const string strEnemyArea = "@EnemyArea";
    public const string strManagers = "@Managers";

    // UI NAME
    public const string CreateHpBar = "UI_HP";
    public const string strStartUI = "UI/Start_UI";

    // PATH : Enemy
    public const string strEnemyPath = "Prefab/Character/Enemy";
    public const string strEnemyAreaPath = "Prefab/Character/EnemyArea";
    public const string strEnemySpawnAreaPath = "Prefab/Character/EnemySpawnArea";
    public const string strAirplaneSpawnAreaPath = "Prefab/Character/AirplaneArea";

    // PATH : Player
    public const string strPlayerPath = "Prefab/Character/Player";
    public const string strPlayerAreaPath = "Prefab/Character/PlayerArea";

    // PATH : Pet
    public const string strPetSpanwer = "Prefab/Pet/PetSpawner";

    // PATH :Bullet
    public const string strBomberPath = "Prefab/Wepaon/Bomber";
    public const string strBulletPath = "WeaponPrefab/Bullet";

    // PATH : Object
    public const string strCoinPath = "Prefab/Coin";
    public const string strHealPath = "Prefab/HealPack";

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

    public enum AirplaneType
    {
        Basic_Airplane,
        Advance_Airplane,
        Elite_Airplane,
        Epic_Airplane,
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
  
    public enum PetType
    {
        Slime,
        PinkPet,
        GrapePet,
        BearJellyPet,
        PuddingPet,
        BlackPet,
        CatFootPet,
        EarthwormPet,
        SharkPet,
        SushiPet,
        BottlePet,
        EarthPet,
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

    public enum FixType
    {
        None_Fix,
        Pet_Fix,
        Bomber_Fix,
        Heal_Fix,
        Airplane_Fix,
        Ultimate_Fix,
        End_Fix,
    }

    public enum StatType
    {
        MaxHp ,
        Exp,
        Attackbility,
    }

    public enum UIType
    {
        None,
        Title,
        Exp,
    }
}
