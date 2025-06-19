using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Defines;

public class UI_Folder : UI_Popup
{
    enum TextType
    {
        CostText,
        SpeedText,
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

    List<UI_WeaponFolder> WeaponFolder = new List<UI_WeaponFolder>();

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

        Register_WeaponFolder();

        return true;
 }
    void Update()
    {
    }

    void Register_WeaponFolder()
    {
        GameObject parent = GetObject(GameObjects.Weapon_Item);

        List<GameObject> childobj = new List<GameObject>();
        childobj = FindChild(parent, "Weapon_Type");

        for (int i = 0; i < childobj.Count; i++)
        {
            UI_WeaponFolder item = Setting.GetOrAddComponent<UI_WeaponFolder>(childobj[i].gameObject);


            item.SetInfo((Defines.WeaponType) Defines.WeaponType.Basic_Weapon+ i, (i + 1) * 700);

            if (i > 0) WeaponFolder[i - 1].NextLockObject = item.MyLockObject;

            WeaponFolder.Add(item);
        }
    }

   
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
