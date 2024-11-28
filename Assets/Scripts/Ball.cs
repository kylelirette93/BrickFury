using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 3f;
    float radius = 0.25f;
    float gravity = 1f;
    string brickTag = "Brick";
    string wallTag = "Wall";
    string playerTag = "Player";
    float initialZPos;
    float minVerticalSpeed = 0.5f;

    Vector3 desiredDirection;
    Vector3 lastHitPoint;

    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.1f;
    private Transform cameraTransform;
    private Vector3 originalCameraPosition;

    private bool isShaking = false;
    private float shakeCooldown = 0.2f;
    private float lastShakeTime = 0f;

    Animator animator;
    PlayerController playerController;



    private void Start()
    {
        // Initial movement direction.
        desiredDirection = new Vector3(1, -1, 0);

        // Initial z position.
        initialZPos = transform.position.z;

        // Get the camera transform.
        cameraTransform = Camera.main.transform;
        originalCameraPosition = cameraTransform.localPosition;
        
        // Get the animator component.
        animator = GetComponent<Animator>();

        // Get the player controller component.
        playerController = GameObject.FindWithTag(playerTag).GetComponent<PlayerController>();
    }

    

    void Update()
    {
        // Create a movement vector, scaled by speed and time.
        Vector3 movement = desiredDirection * speed * Time.deltaTime;
        // Apply movement to the position of the ball.
        transform.position += movement;

        // Apply contraint to the ball's z position.
        transform.position = new Vector3(transform.position.x, transform.position.y, initialZPos);


        RaycastHit hit;
        if (Physics.SphereCast(transform.position, radius, desiredDirection, out hit, movement.magnitude + radius))
        {
            if (hit.collider.CompareTag(wallTag))
            {
                Vector3 collisionNormal = hit.normal;
                // Apply reflection to the desired direction and normalize it to maintain speed.
                desiredDirection = Vector3.Reflect(desiredDirection, collisionNormal).normalized;

                
                if (Mathf.Abs(desiredDirection.y) < minVerticalSpeed)
                {
                    // Return a unit vector depending on y direction, to avoid getting stuck.
                    desiredDirection.y = Mathf.Sign(desiredDirection.y) * minVerticalSpeed;
                }

                transform.position = hit.point + hit.normal * radius;

                if (Time.time - lastShakeTime >= shakeCooldown)
                {
                    animator.SetTrigger("isHit");
                    StartCoroutine(ShakeScreen());
                    lastShakeTime = Time.time;
                }
            }
            else if (hit.collider.CompareTag(brickTag))
            {
                Vector3 collisionNormal = hit.normal;
                desiredDirection = Vector3.Reflect(desiredDirection, collisionNormal).normalized;

                if (Mathf.Abs(desiredDirection.y) < minVerticalSpeed)
                {
                    desiredDirection.y = Mathf.Sign(desiredDirection.y) * minVerticalSpeed;
                }

                hit.collider.GetComponent<Brick>().HitBrick();
                transform.position = hit.point + hit.normal * radius;

                if (Time.time - lastShakeTime >= shakeCooldown)
                {
                    animator.SetTrigger("isHit");
                    StartCoroutine(ShakeScreen());
                    lastShakeTime = Time.time;
                }
            }
            else if (hit.collider.CompareTag(playerTag))
            {
                Transform paddle = hit.collider.transform;

                // Get the paddle's width.
                float paddleWidth = paddle.localScale.x;

                // Get distance from the hit point to the paddle's center.
                float relativePosition = hit.point.x - paddle.position.x;
    
                // Get the normalized hit offset, by dividing by half the paddle width.
                float hitOffset = relativePosition / (paddleWidth / 2);

                // Reflect the ball.
                Vector3 collisionNormal = Vector3.up;
                desiredDirection = Vector3.Reflect(desiredDirection, collisionNormal);

                desiredDirection.x += hitOffset;

                if (Mathf.Abs(desiredDirection.y) < minVerticalSpeed)
                {
                    desiredDirection.y = Mathf.Sign(desiredDirection.y) * minVerticalSpeed;
                }

                // Maintain speed of the ball by normalizing the direction
                desiredDirection = desiredDirection.normalized;

                lastHitPoint = hit.point;
                transform.position = hit.point + hit.normal * radius;
                if (Time.time - lastShakeTime >= shakeCooldown)
                {
                    playerController.OnPaddleHit();
                    animator.SetTrigger("isHit");
                    StartCoroutine(ShakeScreen());
                    lastShakeTime = Time.time;
                }
            }
        }
    }

    IEnumerator ShakeScreen()
    {
        isShaking = true;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1, 1f) * shakeMagnitude;
            float y = Random.Range(-1, 1f) * shakeMagnitude;

            cameraTransform.localPosition = new Vector3(x, originalCameraPosition.y, originalCameraPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        cameraTransform.localPosition = originalCameraPosition;
        isShaking = false;
    }
}