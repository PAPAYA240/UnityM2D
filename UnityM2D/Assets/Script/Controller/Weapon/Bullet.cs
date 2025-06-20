using UnityEngine;

public class Bullet : MonoBehaviour
{
    PlayerController PlayerCon = null;
    EnemyController EnemyCon = null;

    GameObject BulletPrefab = null;

    float elapsedTime = 0;
    void Start()
    {
        FindObject();
    }

    public void Fire()
    {
        if(FindObject() == false)
        {
            Debug.LogWarning("Failed Load Player && Enemy : Bullet");
            return;
        }

        if (Vector3.Distance(transform.position, EnemyCon.transform.position) > 0.1f)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = EnemyCon.transform.position;

            float duration = PlayerCon.State.AttackSpeed * 0.03f;

            if (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                elapsedTime += Time.deltaTime;
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            elapsedTime = 0;
            Managers.ObjectPoolManager.ReturnObject(this.gameObject);

        }
    }
    
    private bool FindObject()
    {
        if (PlayerCon == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if(playerObj != null)
                PlayerCon = playerObj.GetComponent<PlayerController>(); 
        }

        if (EnemyCon == null)
        {
            GameObject enemyObj = GameObject.Find("Enemy");
            if (enemyObj != null)
                EnemyCon = enemyObj.GetComponent<EnemyController>();
        }

        return (PlayerCon != null && EnemyCon != null);
    }
}
