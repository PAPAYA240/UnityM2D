using System.Collections;
using UnityEngine;

public class Bullet : Base
{
    BaseController attacker = null;
    GameObject targeter = null;
    public int _attackPower = 30;
    void Start()
    {
        FindObject();
    }

    public IEnumerator Fire(float waitduration, Vector3 targetLastPosition)
    {
        yield return new WaitForSeconds(waitduration);
        if (FindObject() == false)
        {
            Debug.LogWarning("Failed Load Player && Enemy : Bullet");
            yield break;
        }

       Vector3 startPos = transform.position;
       Vector3 endPos = targetLastPosition;

       float duration = attacker.data.AttackSpeed * 0.03f;

       float elapsedTime = 0f;
       while (elapsedTime < duration)
       {
           float t = elapsedTime / duration;
           transform.position = Vector3.Lerp(startPos, endPos, t);
           elapsedTime += Time.deltaTime;
           yield return null;
       }
        yield return new WaitForSeconds(0.25f);
        Managers.ObjectPoolManager.ReturnObject(this.gameObject);
    }
  
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == targeter)
        {
            BaseController bc = targeter.GetComponent<BaseController>();
            GameObject TargetObject = GameObject.Find("Player");
            if(bc != null)
                bc.TakeDamage(TargetObject.GetComponent<PlayerController>().GetAttackPower());

            this.transform.position = this.transform.parent.position;
            Managers.ObjectPoolManager.ReturnObject(this.gameObject);
        }
    }
    private bool FindObject()
    {
        Transform parentTransform = this.transform.parent;
        if (parentTransform == null)
        {
            Debug.LogWarning("부모 Transform이 없습니다.");
            return false;
        }

        BaseController potentialAttacker = null;
        BaseController controller = parentTransform.gameObject.GetComponent<BaseController>();
        if (controller != null)
        {
            potentialAttacker = controller.GetOwner();
        }
        else
        {
            Weapon weapon = parentTransform.gameObject.GetComponent<Weapon>();
            if (weapon != null)
            {
                potentialAttacker = weapon.GetOwner();
            }
        }

        attacker = potentialAttacker.GetOwner();
        targeter = attacker.TargetObject;

        return true;
    }
}
