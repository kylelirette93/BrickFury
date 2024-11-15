using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float paddleSpeed = 10f;
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 horizontalMovement = new Vector3(horizontalInput, 0, 0);
        transform.position += horizontalMovement * paddleSpeed * Time.deltaTime;
    }
}
