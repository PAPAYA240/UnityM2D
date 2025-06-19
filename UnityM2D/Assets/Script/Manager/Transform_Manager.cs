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

}
