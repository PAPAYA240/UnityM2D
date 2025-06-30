using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bomber : Base
{
    float timeElapsed = 0f;
    private const float g = 9.81f;
    private float _v0x = 5f;
    private float _v0y = 3;
    int bomberAttack = 5;

    Coroutine startAttackCoroutine = null;
    public void UseBomber(GameObject _attacker, GameObject _targeter, float _speed)
    {
        startAttackCoroutine = StartCoroutine(Shelling(_attacker, _targeter, _speed));
    }

    private IEnumerator Shelling(GameObject _attacker, GameObject _targeter, float _speed)
    {
        Vector3 initialPosition = _attacker.transform.position;

        while (Vector3.Distance(transform.position, _targeter.transform.position) > 0.1f)
        {
            // 이동
            timeElapsed += Time.deltaTime;
            float x = initialPosition.x + _v0x * timeElapsed;
            float y = initialPosition.y + _v0y * timeElapsed - 0.5f * g * timeElapsed * timeElapsed;
            transform.position = new Vector3(x, y, transform.position.z);

            // 회전
            float currentVx = _v0x; 
            float currentVy = _v0y - g * timeElapsed; 
            Vector2 currentVelocity = new Vector2(currentVx, currentVy);

            float angleRad = Mathf.Atan2(currentVelocity.y, currentVelocity.x);
            float angleDegrees = angleRad * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angleDegrees);
            yield return null;
        }

        Reset();
        startAttackCoroutine = null;
        yield break;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            BaseController targetController = collision.gameObject.GetComponent<BaseController>();
            if (targetController != null)
                targetController.TakeDamage(bomberAttack);
            Reset();
        }
    }

    private void Reset()
    {
        Managers.ObjectPoolManager.ReturnObject(this.gameObject);
        StopCoroutine(startAttackCoroutine);
        timeElapsed = 0;
    }
}
