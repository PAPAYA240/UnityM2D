using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bullet : Base
{
    PlayerController PlayerCon = null;
    EnemyController EnemyCon = null;

    GameObject BulletPrefab = null;

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

        while(Vector3.Distance(transform.position, EnemyCon.transform.position) > 0.1f)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = EnemyCon.transform.position;

            float duration = PlayerCon.data.AttackSpeed * 0.03f;

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
        if (collision.gameObject.CompareTag("Enemy"))
        {
           EnemyCon.TakeDamage(PlayerCon.data.AttackPower);

            Managers.ObjectPoolManager.ReturnObject(this.gameObject);
        }
    }
    
    private bool FindObject()
    {
        if (PlayerCon == null)
        {
            GameObject playerObj = GameObject.Find(Defines.strPlayerObject);
            if(playerObj != null)
                PlayerCon = playerObj.GetComponent<PlayerController>(); 
        }

        if (EnemyCon == null)
        {
            GameObject enemyObj = GameObject.Find(Defines.strEnemyObject);
            if (enemyObj != null)
                EnemyCon = enemyObj.GetComponent<EnemyController>();
        }

        return (PlayerCon != null && EnemyCon != null);
    }
}
