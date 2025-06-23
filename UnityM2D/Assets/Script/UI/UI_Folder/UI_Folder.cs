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
        ShopContent,
        WeaponTab,
        FixTab,
        BossTab,
        ShopTab,
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

    private List<UI_WeaponFolder> WeaponFolder = new List<UI_WeaponFolder>();
    private List<UI_BossFolder> BossFolders = new List<UI_BossFolder>();

    public EnemyController TargetEnemyController { get; private set; }
    private UI_CheckBossFolder _checkBossFolderUI;
    private EnemyType enemyType;


    void Start()
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

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
        ChangeTab(PlayTab.Weapon);

        GameObject PlayerObject = GameObject.Find(strPlayerObject);
        Player = PlayerObject.GetComponent<PlayerController>();
        TargetEnemyController = GameObject.Find(strEnemyObject).GetComponent<EnemyController>();

        // 하위 폴더 모음
        Register_WeaponFolder();
        Register_BossFolder();

        return true;
 }
    void Update()
    {
        GetText(TextType.AttackText).text = String.Format($"{Player.State.AttackPower}");
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

            if (i > 0) WeaponFolder[i - 1].NextLockObject = item.MyLockObject;

            WeaponFolder.Add(item);
        }
    }
    #endregion

    #region Boss Folder
    private void Register_BossFolder()
   {
        // 보스 몹 시작 UI
        _checkBossFolderUI = Managers.UIManager.ShowUI<UI_CheckBossFolder>("UI_CheckBossFolder", this.gameObject.transform);
        _checkBossFolderUI.gameObject.SetActive(false);

        // 취소 버튼
        Button noButton = Setting.FindChild<Button>(_checkBossFolderUI.gameObject, "No_Button", true);
        Button yesButton = Setting.FindChild<Button>(_checkBossFolderUI.gameObject, "Yes_Button", true);
        if (noButton != null)
            BindEvent(noButton.gameObject, CancelButton);
        if(yesButton != null)
            BindEvent(yesButton.gameObject, AcceptanceButten);

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
                BossFolders.Add(item); 
            }
        }
    }

    private void CancelButton()
    {
        if (_checkBossFolderUI != null)
            _checkBossFolderUI.gameObject.SetActive(false);
    }

    public void AcceptanceButten()
    {
        if (_checkBossFolderUI != null)
            _checkBossFolderUI.gameObject.SetActive(false);

        TargetEnemyController.NextEnemyType = _checkBossFolderUI.enemyType;
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
}
