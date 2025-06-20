using System;
using System.Collections;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static Defines;

public class Weapon : MonoBehaviour
{
    private WeaponData MyData;
    private IAttackStrategy CurrentAttackStrategy;
    private BaseController Owner;
    GameObject bulletprefab = null;

    private GameObject WeaponSocket;

    private float Speed = 0.4f;
    private float AttackDist = 0.6f;

    private float Interp = 0.03f;
    private float fireInterval = 0.1f;
    bool bMove = false;

    
    public void Init(WeaponData _data, BaseController _owner, GameObject _weaponSoket)
    {
        if(_data == null || _owner == null)
        {
            Console.WriteLine("Failed Init : Weapon()");
            return;
        }

        MyData = _data;
        Owner = _owner;
        name = MyData.weaponName;

        CurrentAttackStrategy = WeaponStrategyFactor.CreateStrategy(MyData);
        if(CurrentAttackStrategy == null)
            Console.WriteLine("Failed Load CurrentAttackStrategy : Weapon()");

        // 총알 저장
        if( IsGun())
        {
            bulletprefab = Managers.Resource.Instantiate("WeaponPrefab/Bullet", this.transform);
            int bulletCount = 10;
            Managers.ObjectPoolManager.CreatePool<Bullet>(bulletprefab, bulletCount);

        }

        WeaponSocket = _weaponSoket;
        transform.position = WeaponSocket.transform.position;
    }

    public IEnumerator DeadWeapon()
    {
        Vector3 upPos = this.transform.position + new Vector3(0, 1f, 0);

        while (Vector3.Distance(this.transform.position, upPos) > 0.1f)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = upPos;

            float duration = 5 * 0.03f;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.2f);

        Vector3 DownPosition = this.transform.position + new Vector3(0, -3f, 0);

        while (Vector3.Distance(this.transform.position, DownPosition) > 0.1f)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = DownPosition;

            float duration = 5 * 0.03f;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        Destroy(this);

        yield return null;
    }
    // 기본 움직임
    private void DefaultMoving()
    {
        float rightX = WeaponSocket.transform.position.x + Interp;
        float leftX = WeaponSocket.transform.position.x - Interp;
        if (rightX < this.transform.position.x)
        {
            transform.position = new Vector3(rightX, transform.position.y, transform.position.z);
            bMove = true;
        }
        else if (leftX > this.transform.position.x)
        {
            transform.position = new Vector3(leftX, transform.position.y, transform.position.z);
            bMove = false;
        }

        if (bMove == false)
            this.transform.position = Managers.TransformManager.Go_Forward(this.transform.position, Speed);
        else
            this.transform.position = Managers.TransformManager.Go_Back(this.transform.position, Speed);
    }

    private void AutoFire()
    {
        if (IsGun() == false)
            return;

        GameObject bullet = Managers.ObjectPoolManager.GetObjectKey(bulletprefab, transform.position, transform.rotation);
        
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        
        bulletScript.Fire();
    }

    private bool IsGun()
    {
        return Defines.WeaponType.Advanced_Weapon < MyData.weaponType;
    }

    public IEnumerator PerformAttack(GameObject attacker, GameObject targeter)
    {
        if (CurrentAttackStrategy != null && MyData != null)
        {
            yield return StartCoroutine(CurrentAttackStrategy.ExecuteAttack(attacker, targeter));
        }
        else
        {
            Debug.LogWarning("Cannot perform attack: No valid strategy or weapon data.");
            yield break;
        }
    }

    public IEnumerator ReactionWeapon()
    {
        // == 공격하기 == 
        Vector3 originPos = this.transform.position;
        Vector3 attackTargetPosition = originPos + new Vector3(AttackDist, 0, 0);
        float attackDuration = MyData.reactionTime.x; 
        float attackElapsedTime = 0f;
        while(attackElapsedTime < attackDuration)
        {
            float t = attackElapsedTime / attackDuration;
            this.transform.position = Vector3.Lerp(originPos, attackTargetPosition, t);
            attackElapsedTime += Time.deltaTime;

            yield return null;
        }
        AutoFire();
        this.transform.position = attackTargetPosition;


        // == 돌아가기 == 
        Vector3 returnStartPos = this.transform.position;
       float returnDuration = MyData.reactionTime.y;

        float returnElapsedTime = 0f;
        while(returnElapsedTime < returnDuration)
        {
            float t = returnElapsedTime / returnDuration;
            transform.position = Vector3.Lerp(returnStartPos, WeaponSocket.transform.position, t); 
            returnElapsedTime += Time.deltaTime;
            yield return null;
        }
        this.transform.position = WeaponSocket.transform.position;
    }

}
