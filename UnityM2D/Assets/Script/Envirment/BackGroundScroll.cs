using UnityEngine;
using UnityEngine.UIElements;

public class BackGroundScroll : MonoBehaviour
{
    float Speed = 5;

    float leftPosX = 0f;
    float rightPosX = 0f;
    void Start()
    {
        float length = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        leftPosX = -length;
        rightPosX = length;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(Speed, 0, 0) * Time.deltaTime;

        if(transform.position.x < leftPosX)
        {
            Vector3 selfPos = transform.position;
            selfPos.Set(-leftPosX, selfPos.y, selfPos.z);
            transform.position = selfPos;  
        }
    }
}
