using UnityEngine;
using UnityEngine.UIElements;

public class BackGroundScroll : MonoBehaviour
{
    float Speed = 3;

    float leftPosX = 0f;
    float rightPosX = 0f;
    private float spriteWidth;
    private float screenWidth;

    void Start()
    {
        float screenHeight = Camera.main.orthographicSize * 2f;
        screenWidth = screenHeight * Camera.main.aspect;

        float length = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        leftPosX = -length + (screenWidth / 2);
        rightPosX = length - (screenWidth / 2);


    }

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
