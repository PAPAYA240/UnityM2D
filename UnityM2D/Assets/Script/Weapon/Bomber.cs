using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bomber : Base
{
    public void UseBomber(GameObject _attacker, GameObject _targeter, float _speed)
    {
        StartCoroutine(Shelling(_attacker, _targeter, _speed));
    }

    private IEnumerator Shelling(GameObject _attacker, GameObject _targeter, float _speed)
    {
        while (Vector3.Distance(transform.position, _targeter.transform.position) > 0.1f)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = _targeter.transform.position;

            float duration = _speed * 0.03f;

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
}
