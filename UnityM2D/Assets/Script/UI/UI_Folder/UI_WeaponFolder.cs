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
    WeaponType weaponType = WeaponType.Basic_Weapon;

    private int OpenWeaponLimit = 700;
    private int CurrentWeaponFigure = 0;
    private bool bLock = false;

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

        CurrentWeaponFigure = 100;

        return true;    
    }

    public void SetInfo(PlayerController _player, WeaponType _statType, int _openFigure)
    {
        Init();

        Player = _player;
        weaponType = _statType;
        OpenWeaponLimit = _openFigure;

        int _cnt = (int)weaponType - 1;
        if(_cnt != 0)
        { 
            CurrentWeaponFigure = (_cnt * 700) + 100;
            ChangeText(_cnt * 700);
        }
    }

    void OnUpgradeButtonClick()
    {
        if (IsLock())
            return;

        int costText = CurrentWeaponFigure * 10;
        if (Player.data.Money < costText)
        {
            Console.WriteLine("돈이 부족합니다.");
            Console.WriteLine($"Mony : { Player.data.Money }");
            return;
        }

        Player.data.Money -= costText;
        Player.data.AttackPower = CurrentWeaponFigure;
        CurrentWeaponFigure = CurrentWeaponFigure + 100;

        ChangeText(Player.data.AttackPower);
        IsLock();
    }

    void ChangeText(int _currentPower)
    {
        // 가격
        GetText(Texts.Cost_Text).text = String.Format($"{CurrentWeaponFigure * 10}");
        if (OpenWeaponLimit > CurrentWeaponFigure)
            GetText(Texts.Attack_Text).text = String.Format($"{_currentPower} >> {CurrentWeaponFigure}");
        else
        {
            GetText(Texts.Attack_Text).text = String.Format($"{OpenWeaponLimit} ATTACK!");
        }
    }

    private bool IsLock()
    {
        if (bLock == true)
            return true;

        if (Player == null)
            return false;

        if (OpenWeaponLimit <= Player.data.AttackPower)
        {
            if (NextLockObject != null)
                NextLockObject.SetActive(false);

            // 무기 변경
            WeaponType nextWeapon = weaponType + 1;
            Player.EquipWeapon(nextWeapon);

            GetText(Texts.Upgrade).text = String.Format("CLEAR!");

            bLock = true;
            return true; 
        }

        return false;
    }
}
