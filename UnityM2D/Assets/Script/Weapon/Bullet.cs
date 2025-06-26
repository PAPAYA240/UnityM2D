using Mono.Cecil;
using System.Collections;
using UnityEngine;

public class Bullet : Base
{
    BaseController attacker = null;
    GameObject targeter = null;

    void Start()
    {
        FindObject();
    }

    public IEnumerator Fire()
    {
        if(FindObject() == false)
        {
            Debug.LogWarning("Failed Load Player && Enemy : Bullet");
            yield break;
        }

        while(Vector3.Distance(transform.position, targeter.transform.position) > 0.1f)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = targeter.transform.position;

            float duration = attacker.data.AttackSpeed * 0.03f;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == targeter)
        {
            BaseController baseTargeter = targeter.GetComponent<BaseController>();
            if(baseTargeter != null)
                baseTargeter.TakeDamage(attacker.data.AttackPower);

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
        targeter = attacker.GetTargetObject();

        return true;
    }
}
