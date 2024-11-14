using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Ball : MonoBehaviour
{
    float speed = 5f;
    Vector3 desiredDirection;
    Vector3 diagonalLeft;
    Vector3 diagonalRight;

    private void Start()
    {
        desiredDirection = -transform.up;
        diagonalLeft = transform.up + transform.right;
        diagonalRight = transform.up - transform.right;

    }


    void Update()
    {
        transform.position += desiredDirection * speed * Time.deltaTime;

       
        RaycastHit hit;
        Physics.Raycast(transform.position, desiredDirection, out hit, 0.2f);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Paddle"))
            {
                float paddleCenterX = hit.collider.transform.position.x;
                float ballX = transform.position.x;

                if (ballX < paddleCenterX)  
                {
                    desiredDirection = Vector3.Reflect(diagonalRight, hit.normal);
                }
                else  
                {
                    desiredDirection = Vector3.Reflect(diagonalLeft, hit.normal);
                }

            }
            desiredDirection = Vector3.Reflect(desiredDirection, hit.normal);
        }
    }
}