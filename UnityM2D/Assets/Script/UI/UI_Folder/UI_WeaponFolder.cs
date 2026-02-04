using System;
using UnityEngine;
using static Defines;

public class UI_WeaponFolder : UI_Base
{
    enum Buttons
    {
        Upgrade_Button,
    }

    enum Texts
    {
        Cost_Text, // Upgrade 비용
        Attack_Text, // 다음 올릴 Attack
        Weapon_Name, // 무기 이름
        Upgrade, // 만료 시 Clear로 바꿀 예정
    }

    enum GameObjects
    {
        UI_Lock,
    }

    PlayerController Player = null;
    WeaponType _weaponType = WeaponType.Basic_Weapon;

    private bool bLock = false;
    int reinforcement_bonus = 3;

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

        WeaponData weaponData = Managers.WeaponLoader.GetWeaponData(_weaponType);
        if(weaponData)
            GetText(Texts.Attack_Text).text = String.Format($"{10} >> {10 + weaponData.addedAttack}");

        _currentPrice = weaponData.addedPrice;
        GetText(Texts.Cost_Text).text = String.Format($"{_currentPrice}"); // next Price
        return true;    
    }

    public void SetInfo(PlayerController _player, WeaponType _statType, int _openFigure)
    {
        Init();

        Player = _player;
        _weaponType = _statType;
    }

    void OnUpgradeButtonClick()
    {
        if (IsLock())
            return;

        WeaponData weaponData = Managers.WeaponLoader.GetWeaponData(_weaponType);
        if (!weaponData)
            return;
        int costText = _currentPrice/* + weaponData.addedPrice*/;

        if (Player.data.Money < costText)
        {
            Console.WriteLine("무기 살 돈이 부족합니다.");
            return;
        }

        // 구매
        Player.data.Money -= costText;
        Player.data.AttackPower = Player.data.AttackPower + weaponData.addedAttack;
        Debug.Log($"구매 : {Player.data.AttackPower}");
        ChangeText();
        IsLock();
    }
    private int _currentPrice = 0;
    void ChangeText()
    {
        // 가격
        WeaponData weaponData = Managers.WeaponLoader.GetWeaponData(_weaponType);
        if (!weaponData)
            return;

        int nextAttackDamage = Player.data.AttackPower + weaponData.addedAttack;
        int nextPrice = _currentPrice + weaponData.addedPrice;
        if (weaponData.openWeaponLimit > Player.data.AttackPower)
        {
            GetText(Texts.Cost_Text).text = String.Format($"{nextPrice}"); // next Price
            GetText(Texts.Attack_Text).text = String.Format($"{Player.data.AttackPower} >> {nextAttackDamage}"); // nextAttack
            _currentPrice = nextPrice;
        }
        else
        {
            GetText(Texts.Cost_Text).text = String.Format("NEXT");
            GetText(Texts.Attack_Text).text = String.Format($"{weaponData.openWeaponLimit} ATTACK CLEAR!");
            IsLock();
        }
    }

    private bool IsLock()
    {
        if (bLock == true)
            return true;

        if (Player == null)
            return false;

        WeaponData weaponData = Managers.WeaponLoader.GetWeaponData(_weaponType);
        if (weaponData.openWeaponLimit <= Player.data.AttackPower)
        {
            if (NextLockObject != null)
                NextLockObject.SetActive(false);

            // 무기 변경
            WeaponType nextWeapon = _weaponType + 1;
            Player.EquipWeapon(nextWeapon);

            bLock = true;
            return true; 
        }

        return false;
    }
}
