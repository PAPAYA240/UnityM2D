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
        Object_Icon,
    }

    FixType myFixType = FixType.None_Fix;
    PlayerController Player = null;

    bool[] bOpenFix = new bool[(int)FixType.End_Fix];
    #endregion
    GameObject myPet = null;
    GameObject myAirplane = null;

    private void Awake() => Init();
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
                }
                PetType petNextData = myPet.GetComponent<Pet>().UpgradePet(Player);
                Change_PetInformation(petNextData);
                break;

            case FixType.Bomber_Fix:
                StartCoroutine(Player.UseSkill(FixType.Bomber_Fix));
                break;

            case FixType.Heal_Fix:
                if (Player.data.Heal >= Player.data.MaxHeal)
                {
                    Player.data.Heal = Player.data.MaxHeal;
                    return;
                }
                Change_HealInformation();
                break;
            case FixType.Airplane_Fix:
                if(myAirplane == null)
                {
                    myAirplane = Managers.Resource.Instantiate("Prefab/Airplane/Airplane");
                    myAirplane.AddComponent<Airplane>();
                }
                AirplaneType airplaneNextData = myAirplane.GetComponent<Airplane>().UpgradeAirplane(Player);
                Change_AirplaneInformation(airplaneNextData);
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

        PetData data = Managers.PetLoader.GetPetDataByType(_dataType);
        if (data == null)
            return;

        GetText(Texts.Cost_Text).text = String.Format($"{data.price}");
        GetImage(Images.Object_Icon).sprite = Resources.Load<Sprite>(data.petPrefab);
        GetText(Texts.thisName).text = String.Format($"{data.petName}");
    }

    int _Healprice = 500;
    private void Change_HealInformation()
    {
        // 돈 빼고 시세 올리고
        if (_Healprice > Player.data.Money)
            return;
        Player.data.Money -= _Healprice;
        _Healprice += 500;

        // 힐 추가해주고
        int addHeal = 10;
        Player.UpgradeHeal(addHeal);
        GetText(Texts.Cnt_Text).text = String.Format($"{Player.data.Heal} >> {Player.data.Heal + addHeal}");

       if (Player.data.Heal >= Player.data.MaxHeal)
        {
            Player.data.Heal = Player.data.MaxHeal;
            GetText(Texts.Cnt_Text).text = String.Format($"{Player.data.Heal} >> MAX HEAL!");
            GetText(Texts.Cost_Text).text = String.Format("MAX");
        }
    }

    private void Change_AirplaneInformation(AirplaneType _dataType)
    {
        Managers.DataManager.Airplanes.TryGetValue(String.Format($"{_dataType}"), out AirplaneData originData);
        if (originData == null)
            return;

        AirplaneData data = Managers.AirplaneLoader.GetAirplaneDataByType(_dataType);
        if (data == null)
            return;

        GetText(Texts.Cost_Text).text = String.Format($"{data.price}");
        GetImage(Images.Object_Icon).sprite = Resources.Load<Sprite>(data.airplanePrefab);
        GetText(Texts.thisName).text = String.Format($"{data.airplaneName}");
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
