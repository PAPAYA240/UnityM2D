using System.Collections.Generic;
using System;
using UnityEngine;
using static Defines;

public static class WeaponStrategyFactor
{
    private static readonly Dictionary<WeaponType, Func<WeaponData, IAttackStrategy>> StrategyCreators;

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
            return null;
        }

        if (StrategyCreators.TryGetValue(_data.weaponType, out var creator))
        {
            return creator.Invoke(_data);
        }
        else
        {
            return null;
        }
    }
}
