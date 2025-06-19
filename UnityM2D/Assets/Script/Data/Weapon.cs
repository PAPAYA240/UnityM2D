using System;
using System.Collections;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static Defines;

public class Weapon : MonoBehaviour
{
    private WeaponData MyData;
    private IAttackStrategy CurrentAttackStrategy;
    private BaseController Owner;

    private GameObject WeaponSocket;
    private SpriteRenderer MySpriteRenderer;

    private float Speed = 0.4f;
    private float AttackDist = 0.6f;

    private float Interp = 0.03f;

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

        // 스프라이트
        MySpriteRenderer = GetComponent<SpriteRenderer>();

        CurrentAttackStrategy = WeaponStrategyFactor.CreateStrategy(MyData);
        if(CurrentAttackStrategy == null)
            Console.WriteLine("Failed Load CurrentAttackStrategy : Weapon()");

        WeaponSocket = _weaponSoket;
        transform.position = WeaponSocket.transform.position;
    }

    private void Update()
    {
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

    public void Use(GameObject _target)
    {
        CurrentAttackStrategy.ExecuteAttack(Owner.gameObject, _target.gameObject);
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
