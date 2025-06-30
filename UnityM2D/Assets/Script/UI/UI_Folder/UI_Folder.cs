using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Defines;
using static UnityEngine.EventSystems.EventTrigger;

public class UI_Folder : UI_Popup
{
    enum TextType
    {
        CostText,
        SpeedText,
        AttackText,
    }
    enum ButtonType
    {
        Weapon_Button,
        Fix_Button,
        Boss_Button,
        Shop_Button,
    }
    enum GameObjects
    {
        WeaponContent,
        Weapon_Item,
        BossContent,
        Boss_Item,
        FixContent,
        Fix_Item,
        ShopContent,
        WeaponTab,
        FixTab,
        BossTab,
        ShopTab,
        ShopItem,
    }
    public enum PlayTab
    {
        None,
        Weapon,
        Fix,
        Boss,
        Shop
    }

    PlayTab TabType = PlayTab.None;
    PlayerController Player = null;

    private List<UI_WeaponFolder> weaponFolder = new List<UI_WeaponFolder>();
    private List<UI_BossFolder> bossFolder = new List<UI_BossFolder>();
    private List<UI_FixFolder> fixFolder = new List<UI_FixFolder>();
    private List<UI_AdsFolder> shopFolder = new List<UI_AdsFolder>();

    public EnemyController TargetEnemyController { get; private set; }
    private UI_CheckBossFolder _checkBossFolderUI;
    private EnemyType enemyType;


    void Start() => Init();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if(!InitBind())
            Debug.Log("Failed Bind : UI_Folder()");

        ChangeTab(PlayTab.Weapon);

        GameObject PlayerObject = GameObject.Find(strPlayerObject);
        Player = PlayerObject.GetComponent<PlayerController>();
        TargetEnemyController = GameObject.Find(strEnemyObject).GetComponent<EnemyController>();

        // 하위 폴더 모음
        Register_WeaponFolder();
        Register_BossFolder();
        Register_FixFolder();

        return true;
 }
    void Update()
    {
        GetText(TextType.AttackText).text = String.Format($"{Player.data.AttackPower}");
    }

    #region Weapon Folder
    private void Register_WeaponFolder()
    {
        GameObject parent = GetObject(GameObjects.Weapon_Item);

        List<GameObject> childobj = new List<GameObject>();
        childobj = Setting.FindChildList(parent, "Weapon_Type");

        for (int i = 0; i < childobj.Count; i++)
        {
            UI_WeaponFolder item = Setting.GetOrAddComponent<UI_WeaponFolder>(childobj[i].gameObject);

            item.SetInfo(Player, Defines.WeaponType.Basic_Weapon+ i, (i + 1) * 700);

            if (i > 0) weaponFolder[i - 1].NextLockObject = item.MyLockObject;

            weaponFolder.Add(item);
        }
    }
    #endregion

    #region Fix Folder
    private void Register_FixFolder()
    {
        GameObject parent = GetObject(GameObjects.Fix_Item);

        List<GameObject> childobj = new List<GameObject>();
        childobj = Setting.FindChildList(parent, "Fix_Type");

        for (int i = 0; i < childobj.Count; i++)
        {
            UI_FixFolder item = Setting.GetOrAddComponent<UI_FixFolder>(childobj[i].gameObject);

            item.SetInfo((FixType) i + 1);

            fixFolder.Add(item);
        }
    }
    #endregion

    #region Boss Folder
    private void Register_BossFolder()
   {
        // 보스 몹 시작 UI
        _checkBossFolderUI = Managers.UIManager.ShowUI<UI_CheckBossFolder>("UI_CheckBossFolder", this.gameObject.transform);
        _checkBossFolderUI.gameObject.SetActive(false);

        GameObject parent = GetObject(GameObjects.Boss_Item);

        List<GameObject> childobj = new List<GameObject>();
        childobj = Setting.FindChildList(parent, "Boss_Type");

        foreach (GameObject boss in childobj)
        {
            UI_BossFolder item = Setting.GetOrAddComponent<UI_BossFolder>(boss);
            if (item != null)
            {
                item.SetInfo(
                    (Defines.EnemyType)(boss.transform.GetSiblingIndex() + (int)Defines.EnemyType.Zombi_Boss),
                    _checkBossFolderUI,
                    TargetEnemyController 
                );
                bossFolder.Add(item); 
            }
        }
    }

    #endregion

    #region Ads Folder
    private void Register_AdsFolder()
    {
        GameObject parent = GetObject(GameObjects.ShopItem);

        List<GameObject> childobj = new List<GameObject>();
        childobj = Setting.FindChildList(parent, "Ads_Type");

        for (int i = 0; i < childobj.Count; i++)
        {
            UI_AdsFolder item = Setting.GetOrAddComponent<UI_AdsFolder>(childobj[i].gameObject);

            shopFolder.Add(item);
        }
    }
    #endregion

    public void ChangeTab(PlayTab _tab)
    {
        if (TabType == _tab)
            return;

        GetObject(GameObjects.WeaponTab).gameObject.SetActive(false);
        GetObject(GameObjects.FixTab).gameObject.SetActive(false);
        GetObject(GameObjects.BossTab).gameObject.SetActive(false);
        GetObject(GameObjects.ShopTab).gameObject.SetActive(false);

        TabType = _tab;
        switch (TabType)
        {
            case PlayTab.Weapon:
                GetObject(GameObjects.WeaponTab).gameObject.SetActive(true);
                break;
            case PlayTab.Fix:
                GetObject(GameObjects.FixTab).gameObject.SetActive(true);
            break;
            case PlayTab.Boss:
                GetObject(GameObjects.BossTab).gameObject.SetActive(true);
            break;
            case PlayTab.Shop:
                GetObject(GameObjects.ShopTab).gameObject.SetActive(true);
            break;
            default:
                break;
        }
    }

    #region Initialize
    private bool InitBind()
    {
        Managers.UIManager.ShowUI<UI_UltimateButton>("UI_UltimateButton", this.gameObject.transform);

        BindText(typeof(TextType));
        BindButton(typeof(ButtonType));
        BindObject(typeof(GameObjects));

        GameObject weaponButtonGO = GetButton(ButtonType.Weapon_Button).gameObject;
        GameObject FixButtonGO = GetButton(ButtonType.Fix_Button).gameObject;
        GameObject BossButtonGO = GetButton(ButtonType.Boss_Button).gameObject;
        GameObject shopButtonGO = GetButton(ButtonType.Shop_Button).gameObject;

        BindEvent(weaponButtonGO, () => ChangeTab(PlayTab.Weapon));
        BindEvent(FixButtonGO, () => ChangeTab(PlayTab.Fix));
        BindEvent(BossButtonGO, () => ChangeTab(PlayTab.Boss));
        BindEvent(shopButtonGO, () => ChangeTab(PlayTab.Shop));
        return true;
    }
    #endregion
}
