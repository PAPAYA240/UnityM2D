using System.Collections.Generic;
using System;
using UnityEngine;
using static Defines;

public static class WeaponStrategyFactor
{
    private static readonly Dictionary<WeaponType, Func<WeaponData, IAttackStrategy>> StrategyCreators;

    //  static 생성자 : 클래스가 처음 로드될 때 한 번만 실행
    static WeaponStrategyFactor()
    {
        StrategyCreators = new Dictionary<WeaponType, Func<WeaponData, IAttackStrategy>>();

        StrategyCreators.Add(WeaponType.None_Weapon, (WeaponData) => new BasicAttack());

        StrategyCreators.Add(WeaponType.Basic_Weapon, (WeaponData) => new BasicAttack());

        StrategyCreators.Add(WeaponType.Advanced_Weapon, (WeaponData) => new AdvancedAttack());

        StrategyCreators.Add(WeaponType.Elite_Weapon, (WeaponData) => new EliteAttack());

        StrategyCreators.Add(WeaponType.Epic_Weapon, (WeaponData) => new EpicAttack());

        StrategyCreators.Add(WeaponType.Mythic_Weapon, (WeaponData) => new MythicAttack());
    }

    public static IAttackStrategy CreateStrategy(WeaponData _data)
    {
        if (_data == null)
        {
            Console.WriteLine("Failed  WeaponData");
            return null;
        }

        if (StrategyCreators.TryGetValue(_data.weaponType, out var creator))
            return creator.Invoke(_data);
        else
        {
            Console.WriteLine("Failed CreateStrategy : WeaponStrategyFactory()");
            return null;
        }
    }


}
