using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Transform_Manager : MonoBehaviour
{
    public Vector3 MoveToTarget(Vector3 _currPos, Vector3 _target, float _speed)
    {
         return Vector3.Lerp(_currPos, _target, Time.deltaTime * _speed);
    }

    public Vector3 Go_Forward(Vector3 pos, float _speed = 1)
    {
        pos.x += (_speed * Time.deltaTime);
        return pos;
    }
    public Vector3 Go_Back(Vector3 pos, float _speed = 1)
    {
        pos.x -= (_speed * Time.deltaTime);
        return pos;
    }

    // 1초 동안 갈 수 있는 새로운 위치를 계산한다.
    public Vector3 CalculateNextPosition(Vector3 currentPos, Vector3 targetPos, float speed)
    {
        float step = speed * Time.deltaTime;
        return Vector3.MoveTowards(currentPos, targetPos, step);
    }

    /* ref를 사용할 수 없을까?
    ref나 out 키워드는 메서드가 즉시 실행될 때 
    변수의 메모리 주소를 넘겨주어 원본 값을 수정할 수 있게 합니다. 
    하지만 코루틴은 yield return을 만나면 실행을 잠시 멈추고 제어권을 
    
    반환합니다. 다음 프레임에 다시 실행될 때, 원래 ref로 넘겨받았던 변수의 
    메모리 상태가 유지될 것이라는 보장이 없기 때문에 (가비지 컬렉션이나 
    다른 스레드에 의해 변경될 수 있으므로) C# 컴파일러가 이를 허용하지 않습니다.

    public IEnumerator LerpMoveToTarget(Vector3 targetPos, float speed)
    {
        Vector3 startPosition = transform.position; 
        float duration = speed * 0.03f; 

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // t = Mathf.SmoothStep(0f, 1f, t); 

            transform.position = Vector3.Lerp(startPosition, targetPos, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPos;
        Debug.Log("이동 완료!");
    }*/
}
