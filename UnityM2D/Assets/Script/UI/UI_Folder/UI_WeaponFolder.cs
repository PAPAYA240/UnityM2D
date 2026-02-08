using System;
using UnityEngine;
using static Defines;

public class UI_WeaponFolder : UI_Base
{
    enum Buttons { Upgrade_Button }
    enum Texts
    {
        Cost_Text,   
        Attack_Text,
        Weapon_Name,
        Upgrade,
    }
    enum GameObjects { UI_Lock }

    private PlayerController _player = null;
    private WeaponType _weaponType = WeaponType.Basic_Weapon;

    private bool _isLocked = false;
    private int _currentPrice = 0;

    public GameObject MyLockObject { get; private set; }
    public GameObject NextLockObject { private get; set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton(Buttons.Upgrade_Button).gameObject.BindEvent(OnUpgradeButtonClick, Defines.Input.Click);
        MyLockObject = GetObject(GameObjects.UI_Lock);

        return true;
    }

    public void SetInfo(PlayerController player, WeaponType weaponType)
    {
        if (_init == false) Init();

        _player = player;
        _weaponType = weaponType;

        RefreshUI();
    }

    void OnUpgradeButtonClick()
    {
        if (_isLocked) return;

        WeaponData weaponData = Managers.WeaponLoader.GetWeaponData(_weaponType);
        if (weaponData == null || _player == null) return;

        if (_player.data.Money < _currentPrice)
            return;

        _player.data.Money -= _currentPrice;
        _player.data.AttackPower += weaponData.addedAttack;

        RefreshUI();
    }

    void RefreshUI()
    {
        WeaponData weaponData = Managers.WeaponLoader.GetWeaponData(_weaponType);
        if (weaponData == null) return;

        int nextAttack = _player.data.AttackPower + weaponData.addedAttack;
        int nextPrice = (_currentPrice == 0 ? weaponData.addedPrice : _currentPrice + weaponData.addedPrice); // 로직에 따라 초기값 조정 필요

        if (_player.data.AttackPower >= weaponData.openWeaponLimit)
        {
            SetCompletedState(weaponData.openWeaponLimit);
        }
        else
        {
            _currentPrice = nextPrice;
            GetText(Texts.Cost_Text).text = $"{_currentPrice}";
            GetText(Texts.Attack_Text).text = $"{_player.data.AttackPower} >> {nextAttack}";
        }
    }

    private void SetCompletedState(int maxLimit)
    {
        GetText(Texts.Cost_Text).text = "NEXT";
        GetText(Texts.Attack_Text).text = $"{maxLimit} ATTACK CLEAR!";

        _isLocked = true;

        if (NextLockObject != null)
            NextLockObject.SetActive(false);

        if (_player != null)
        {
            WeaponType nextWeapon = _weaponType + 1;
            _player.EquipWeapon(nextWeapon);
        }
    }
}