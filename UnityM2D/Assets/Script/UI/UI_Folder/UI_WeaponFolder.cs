using System;
using Spine;
using Unity.VisualScripting;
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

        GameObject PlayerObject = GameObject.Find("Player");
        Player = PlayerObject.GetComponent<PlayerController>();

        // === Weapon 세팅 ===
        CurrentWeaponFigure = 100;
        // 가격
        GetText(Texts.Cost_Text).text = String.Format($"{CurrentWeaponFigure * 10}");
        // 다음 공격력
        GetText(Texts.Attack_Text).text = String.Format($"{Player.State.AttackPower} >> {CurrentWeaponFigure}");

        return true;    
    }

    public void SetInfo(WeaponType _statType, int _openFigure)
    {
        Init();

        weaponType = _statType;
        OpenWeaponLimit = _openFigure;
    }
    
    void OnUpgradeButtonClick()
    {
        if (IsLock())
            return;

        int costText = CurrentWeaponFigure * 10;
        if (Player.State.Money < costText)
        {
            Console.WriteLine("돈이 부족합니다.");
            Console.WriteLine($"Mony : { Player.State.Money }");
            return;
        }

        Player.State.Money -= costText;
        Player.State.AttackPower = CurrentWeaponFigure;
        CurrentWeaponFigure = CurrentWeaponFigure + 100;

        // 가격
        GetText(Texts.Cost_Text).text = String.Format($"{CurrentWeaponFigure * 10}");
        if (OpenWeaponLimit > CurrentWeaponFigure)
            GetText(Texts.Attack_Text).text = String.Format($"{Player.State.AttackPower} >> {CurrentWeaponFigure}");
        else
        { 
            GetText(Texts.Attack_Text).text = String.Format($"{OpenWeaponLimit} ATTACK!");
        }

        IsLock();
    }

    private bool IsLock()
    {
        if (bLock == true)
            return false;

        if (Player == null)
            return false;

        if (OpenWeaponLimit <= Player.State.AttackPower)
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
