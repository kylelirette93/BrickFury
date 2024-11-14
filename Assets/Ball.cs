using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Ball : MonoBehaviour
{
    float speed = 5f;
    Vector3 desiredDirection;

    private void Start()
    {
        desiredDirection = -transform.up;
    }


    void Update()
    {
        transform.position += desiredDirection * speed * Time.deltaTime;

        RaycastHit hit;
        Physics.Raycast(transform.position, desiredDirection, out hit, 0.1f);

        if (hit.collider != null)
        {
            desiredDirection = Vector3.Reflect(desiredDirection, hit.normal);
        }
    }
}
