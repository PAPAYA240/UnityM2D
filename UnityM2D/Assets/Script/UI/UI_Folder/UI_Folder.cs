using System;
using System.Collections.Generic;
using UnityEngine;
using static Defines;

public class UI_Folder : UI_Popup
{
    #region Enums
    enum TextType
    {
        CostText,
        HealText,
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
#endregion

    private PlayTab _currentTab = PlayTab.None;
    private PlayerController _player = null;
    public EnemyController TargetEnemyController { get; private set; }

    private List<UI_WeaponFolder> _weaponFolder = new List<UI_WeaponFolder>();
    private List<UI_BossFolder> _bossFolder = new List<UI_BossFolder>();
    private List<UI_FixFolder> _fixFolder = new List<UI_FixFolder>();
    private List<UI_AdsFolder> _shopFolder = new List<UI_AdsFolder>();

    private UI_CheckBossFolder _checkBossFolderUI;


    void Start() => Init();

    public override bool Init()
    {
        if (!base.Init())
            return false;

        if (!InitBind())
            return false;

        GameObject playerObj = GameObject.Find(strPlayerObject);
        if (playerObj != null)
            _player = playerObj.GetComponent<PlayerController>();

        GameObject enemyObj = GameObject.Find(strEnemyObject);
        if (enemyObj != null)
            TargetEnemyController = enemyObj.GetComponent<EnemyController>();

        if (_player == null || TargetEnemyController == null)
            return false;

        RegisterWeaponFolder();
        RegisterBossFolder();
        RegisterFixFolder();
        RegisterAdsFolder();

        ChangeTab(PlayTab.Weapon);

        return true;
    }
    void Update()
    {
        if (_player == null) 
            return;

        // TODO: 최적화를 위해 데이터가 변경될 때만 갱신하는 이벤트 방식으로 변경 권장
        GetText(TextType.CostText).SetText(_player?.data.Money.ToString());
        GetText(TextType.HealText).SetText(_player?.data.Heal.ToString());
        GetText(TextType.AttackText).text = String.Format($"{_player.data.AttackPower}");
    }
    public void ChangeTab(PlayTab tab)
    {
        if (_currentTab == tab)
            return;

        _currentTab = tab;

        GetObject(GameObjects.WeaponTab).SetActive(false);
        GetObject(GameObjects.FixTab).SetActive(false);
        GetObject(GameObjects.BossTab).SetActive(false);
        GetObject(GameObjects.ShopTab).SetActive(false);

        switch (_currentTab)
        {
            case PlayTab.Weapon: GetObject(GameObjects.WeaponTab).SetActive(true); break;
            case PlayTab.Fix: GetObject(GameObjects.FixTab).SetActive(true); break;
            case PlayTab.Boss: GetObject(GameObjects.BossTab).SetActive(true); break;
            case PlayTab.Shop: GetObject(GameObjects.ShopTab).SetActive(true); break;
        }
    }

    #region Weapon Folder
    private void RegisterWeaponFolder()
    {
        GameObject parent = GetObject(GameObjects.Weapon_Item);
        List<GameObject> childobj = Setting.FindChildList(parent, "Weapon_Type");

        for (int i = 0; i < childobj.Count; i++)
        {
            UI_WeaponFolder item = Setting.GetOrAddComponent<UI_WeaponFolder>(childobj[i].gameObject);
            item.SetInfo(_player, WeaponType.Basic_Weapon + i);

            if (i > 0) 
                _weaponFolder[i - 1].NextLockObject = item.MyLockObject;

            _weaponFolder.Add(item);
        }
    }
    #endregion

    #region Fix Folder
    private void RegisterFixFolder()
    {
        GameObject parent = GetObject(GameObjects.Fix_Item);
        List<GameObject>  childobj = Setting.FindChildList(parent, "Fix_Type");

        for (int i = 0; i < childobj.Count; i++)
        {
            UI_FixFolder item = Setting.GetOrAddComponent<UI_FixFolder>(childobj[i].gameObject);
            item.SetInfo((FixType) i + 1);
            _fixFolder.Add(item);
        }
    }
    #endregion

    #region Boss Folder
    private void RegisterBossFolder()
   {
        _checkBossFolderUI = Managers.UIManager.ShowUI<UI_CheckBossFolder>("UI_CheckBossFolder", this.gameObject.transform);
        _checkBossFolderUI.gameObject.SetActive(false);

        GameObject parent = GetObject(GameObjects.Boss_Item);
        List<GameObject> childobj = Setting.FindChildList(parent, "Boss_Type");

        foreach (GameObject bossObj in childobj)
        {
            UI_BossFolder item = Setting.GetOrAddComponent<UI_BossFolder>(bossObj);
            if (item != null)
            {
                item.SetInfo(
                    (Defines.EnemyType)(bossObj.transform.GetSiblingIndex() + (int)Defines.EnemyType.Skeleton_Boss),
                    _checkBossFolderUI,
                    TargetEnemyController 
                );
                _bossFolder.Add(item); 
            }
        }
    }
    #endregion

    #region Ads Folder
    private void RegisterAdsFolder()
    {
        GameObject parent = GetObject(GameObjects.ShopItem);
        List<GameObject> childobj = Setting.FindChildList(parent, "Ads_Type");

        for (int i = 0; i < childobj.Count; i++)
        {
            UI_AdsFolder item = Setting.GetOrAddComponent<UI_AdsFolder>(childobj[i].gameObject);
            item.SetInfo((UI_AdsFolder.RewardType)i);
            _shopFolder.Add(item);
        }
    }
    #endregion


    #region Initialize
    private bool InitBind()
    {
        Managers.UIManager.ShowUI<UI_UltimateButton>("UI_UltimateButton", transform);

        BindText(typeof(TextType));
        BindButton(typeof(ButtonType));
        BindObject(typeof(GameObjects));

        BindEvent(GetButton(ButtonType.Weapon_Button).gameObject, () => ChangeTab(PlayTab.Weapon));
        BindEvent(GetButton(ButtonType.Fix_Button).gameObject, () => ChangeTab(PlayTab.Fix));
        BindEvent(GetButton(ButtonType.Boss_Button).gameObject, () => ChangeTab(PlayTab.Boss));
        BindEvent(GetButton(ButtonType.Shop_Button).gameObject, () => ChangeTab(PlayTab.Shop));

        return true;
    }
    #endregion
}
