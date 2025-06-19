using System.Collections;
using UnityEngine;


public interface IAttackStrategy
 {
    IEnumerator ExecuteAttack(GameObject _attacker, GameObject _target);
 }

public class BasicAttack : IAttackStrategy
{
     // 근접 공격
     public IEnumerator ExecuteAttack(GameObject _attacker, GameObject _target)
     {
         BaseController Attacker = _attacker.GetComponent<BaseController>();
         BaseController Targeter = _target.GetComponent<BaseController>();

         if (Attacker != null && Targeter != null) 
         {
             yield return new WaitForSeconds(0.3f);

             yield return Attacker.StartCoroutine(Attacker.AttacktoMove(_target));

        }
    }
 }

 public class AdvancedAttack : IAttackStrategy
 {
     public IEnumerator ExecuteAttack(GameObject _attacker, GameObject _target)
     {
        BaseController Attacker = _attacker.GetComponent<BaseController>();
        BaseController Targeter = _target.GetComponent<BaseController>();

        if (Attacker != null && Targeter != null)
        {
            yield return new WaitForSeconds(0.3f);

            yield return Attacker.StartCoroutine(Attacker.AttacktoMove(_target));

        }
    }
 }

 public class EliteAttack : IAttackStrategy
 {
     public IEnumerator ExecuteAttack(GameObject _attacker, GameObject _target)
     {
        BaseController Attacker = _attacker.GetComponent<BaseController>();
        BaseController Targeter = _target.GetComponent<BaseController>();

        if (Attacker != null && Targeter != null)
        {
            yield return new WaitForSeconds(0.3f);
            Attacker.ReactionWeapon();

            BaseController Target = Targeter.GetComponent<BaseController>();
            if (Target != null)
                Target.TakeDamage(Attacker.State.AttackPower);
        }
        yield break;
    }
}

 public class EpicAttack : IAttackStrategy
 {
     public IEnumerator ExecuteAttack(GameObject _attacker, GameObject _target)
     {
        yield break;
    }
}

 public class MythicAttack : IAttackStrategy
 {
     public IEnumerator ExecuteAttack(GameObject _attacker, GameObject _target)
     {
        yield break;
    }
}


