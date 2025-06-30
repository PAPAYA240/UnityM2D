using UnityEngine;
using static Defines;

public class Player_Ultimate : MonoBehaviour
{
    GameObject playerObject = null;
    void Start()
    {
        playerObject = GameObject.Find(strPlayerObject);
        if (playerObject != null)
        {
            Vector3 pos = playerObject.transform.position;
            pos.y = transform.position.y - 0.25f;
            transform.position = pos;
           }
    }

    void Update()
    {
        
    }
}
