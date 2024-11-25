using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    Camera mainCamera;
    float minScreenWidth = -7.65f;
    float maxScreenWidth = 7.65f;


    
    void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint
            (new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));

        transform.position = new Vector3(Mathf.Clamp(mouseWorldPosition.x, minScreenWidth, maxScreenWidth), transform.position.y, transform.position.z);
        
    }
}
