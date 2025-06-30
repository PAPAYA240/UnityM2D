using System;
using UnityEngine;
using UnityEngine.UI;
using static Defines;

public class UI_FixFolder : UI_Base
{
    #region 변수
    enum Buttons
    {
        Upgrade_Icon,
    }

    enum Texts
    {
        Weapon_Name,
        Cnt_Text,
        Upgrade,
        Cost_Text,
        thisName,
    }
    enum Images
    {
        Icon,
        Pet_Icon,
    }

    FixType myFixType = FixType.None_Fix;
    PlayerController Player = null;

    bool[] bOpenFix = new bool[(int)FixType.End_Fix];
    #endregion
    GameObject myPet = null;

    private void Start() => Init();
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        if (!InitBind())
            Debug.Log("Failed Bind : UI_FixFolder");

        return true;
    }

    void OnUpgradeButtonClick()
    {
        if (Player == null)
            return;

       switch(myFixType)
        {
            case FixType.Pet_Fix:
                if(myPet == null)
                {
                    myPet = Managers.Resource.Instantiate("Prefab/Pet/Pet");
                    myPet.AddComponent<Pet>();
                    break;
                }
                PetType dataType = myPet.GetComponent<Pet>().UpgradePet(Player);
                Change_PetInformation(dataType);

                break;

            case FixType.Bomber_Fix:
                StartCoroutine(Player.UseSkill(FixType.Bomber_Fix));
                break;

            case FixType.Ultimate_Fix:
                StartCoroutine(Player.UseSkill(FixType.Ultimate_Fix));
                break;

            default:
                break;
        }
    }

    private void Change_PetInformation(PetType _dataType)
    {
        Managers.DataManager.Pets.TryGetValue(String.Format($"{_dataType}"), out PetData originData);
        if (originData == null)
            return;

        GetText(Texts.Cost_Text).text = String.Format($"{originData.Money}");
        GetImage(Images.Pet_Icon).sprite = Resources.Load<Sprite>(originData.prefab);
        GetText(Texts.thisName).text = String.Format($"{originData.Name}");
    }

    public void SetInfo(FixType _type)
    {
        myFixType = _type;

        GameObject playerObj = GameObject.Find(strPlayerObject);
        if(playerObj != null)
            Player = playerObj.GetComponent<PlayerController>();
    }

    #region Initialize
    private bool InitBind()
    {
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        Button upgradeButton = GetButton(Buttons.Upgrade_Icon);
        if (upgradeButton == null)
            return false;
        BindEvent(upgradeButton.gameObject, OnUpgradeButtonClick);

        return true;
    }
    #endregion

}
