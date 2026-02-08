using System;
using UnityEngine;
using UnityEngine.UI;
using static Defines;

public class UI_FixFolder : UI_Base
{
    enum Buttons { Upgrade_Icon }
    enum Texts
    {
        Weapon_Name,
        Cnt_Text,
        Upgrade,
        Cost_Text,
        thisName,
    }
    enum Images { Icon, Object_Icon }

    private FixType _myFixType = FixType.None_Fix;
    private PlayerController _player = null;

    private GameObject _myPet = null;
    private GameObject _myAirplane = null;

    private int _healPrice = 500;

    private const int BOMBER_COST = 2000;
    private const int AIRPLANE_COST = 5000;
    private const int HEAL_INCREMENT_COST = 500;
    private const int HEAL_AMOUNT = 10;

    private void Awake() => Init();

    public override bool Init()
    {
        if (!base.Init())
            return false;

        if (!InitBind())
            return false;

        return true;
    }

    public void SetInfo(FixType type)
    {
        _myFixType = type;

        GameObject playerObj = GameObject.Find(strPlayerObject);
        if (playerObj != null)
            _player = playerObj.GetComponent<PlayerController>();
    }

    void OnUpgradeButtonClick()
    {
        if (_player == null) return;

        switch (_myFixType)
        {
            case FixType.Pet_Fix: ProcessPetUpgrade(); break;
            case FixType.Bomber_Fix: ProcessBomberUpgrade(); break;
            case FixType.Heal_Fix: ProcessHealUpgrade(); break;
            case FixType.Airplane_Fix: ProcessAirplaneUpgrade(); break;
        }
    }

    #region Upgrade Logic Processes
    private void ProcessPetUpgrade()
    {
        if (_myPet == null)
        {
            PetData data = Managers.PetLoader.GetPetDataByType(PetType.Slime);
            if (data.price > _player.data.Money)
                return;

            _myPet = Managers.Resource.Instantiate("Prefab/Pet/Pet");
            _myPet.AddComponent<Pet>();
        }

        Pet petScript = _myPet.GetComponent<Pet>();
        if (petScript != null)
        {
            PetType nextData = petScript.UpgradePet(_player);
            UpdatePetUI(nextData);
        }
    }

    private void ProcessBomberUpgrade()
    {
        if (_player.data.Money < BOMBER_COST) return;

        _player.data.Money -= BOMBER_COST;
        StartCoroutine(_player.UseSkill(FixType.Bomber_Fix, true));

        GetText(Texts.Cnt_Text).text = $"{_player.bomberDuration} DURATION";
    }

    private void ProcessHealUpgrade()
    {
        if (_player.data.Heal >= _player.data.MaxHeal)
        {
            _player.data.Heal = _player.data.MaxHeal;
            UpdateHealUI(true);
            return;
        }

        if (_healPrice > _player.data.Money) return;

        _player.data.Money -= _healPrice;
        _healPrice += HEAL_INCREMENT_COST;

        _player.UpgradeHeal(HEAL_AMOUNT);
        UpdateHealUI(false);
    }

    private void ProcessAirplaneUpgrade()
    {
        if (_player.data.Money < AIRPLANE_COST) return;

        _player.data.Money -= AIRPLANE_COST;

        if (_myAirplane == null)
        {
            _myAirplane = Managers.Resource.Instantiate("Prefab/Airplane/Airplane");
            _myAirplane.AddComponent<Airplane>();
        }

        Airplane airplaneScript = _myAirplane.GetComponent<Airplane>();
        if (airplaneScript != null)
        {
            AirplaneType nextData = airplaneScript.UpgradeAirplane(_player);
            UpdateAirplaneUI(nextData);
        }
    }
    #endregion

    #region UI Update Methods
    private void UpdatePetUI(PetType dataType)
    {
        Managers.DataManager.Pets.TryGetValue($"{dataType}", out PetData originData);
        if (originData == null) 
            return;

        PetData data = Managers.PetLoader.GetPetDataByType(dataType);
        if (data == null) 
            return;

        GetText(Texts.Cost_Text).text = $"{data.price}";
        GetImage(Images.Object_Icon).sprite = Resources.Load<Sprite>(data.petPrefab);
        GetText(Texts.thisName).text = data.petName;
    }

    private void UpdateHealUI(bool isMax)
    {
        if (isMax || _player.data.Heal >= _player.data.MaxHeal)
        {
            GetText(Texts.Cnt_Text).text = "MAX HEAL!";
            GetText(Texts.Cost_Text).text = "MAX";
        }
        else
        {
            GetText(Texts.Cnt_Text).text = $"{HEAL_AMOUNT} UPGRADE";
        }
    }

    private void UpdateAirplaneUI(AirplaneType dataType)
    {
        Managers.DataManager.Airplanes.TryGetValue($"{dataType}", out AirplaneData originData);
        if (originData == null) return;

        AirplaneData data = Managers.AirplaneLoader.GetAirplaneDataByType(dataType);
        if (data == null) return;

        GetText(Texts.Cost_Text).text = $"{data.price}";
        GetImage(Images.Object_Icon).sprite = Resources.Load<Sprite>(data.airplanePrefab);
        GetText(Texts.thisName).text = data.airplaneName;
    }
    #endregion

    private bool InitBind()
    {
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        Button upgradeBtn = GetButton(Buttons.Upgrade_Icon);
        if (upgradeBtn != null)
            BindEvent(upgradeBtn.gameObject, OnUpgradeButtonClick);
        else
            return false;

        return true;
    }
}