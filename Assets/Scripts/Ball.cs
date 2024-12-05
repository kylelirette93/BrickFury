using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
public class Ball : MonoBehaviour
{
    public float speed = 3f; 
    float radius = 0.25f; 
    string brickTag = "Brick"; 
    string wallTag = "Wall"; 
    string playerTag = "Player"; 
    float initialZPos; 
    float minVerticalSpeed = 0.6f;
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

    // Ball glow variables.
    private Material ballMaterial;
    private Color originalEmissionColor;
    public Color hitEmissionColor;
    public float emissionIntensity;
    private bool isHit = false;

    public AudioSource paddleHit;
    public AudioSource brickHit;

    bool canMove = false;

    // Variables for countdown at start.
    TextMeshProUGUI countDownText;

    // Highlight normal vector of surface being hit with particles.
    public GameObject hitParticleSystemPrefab;


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

        ballMaterial = GetComponent<Renderer>().material;
        originalEmissionColor = ballMaterial.GetColor("_EmissionColor");

        // Give the ball a player a second before ball comes in.
        if (GameManager.instance.currentState != GameManager.GameState.Win)
        {
            // Prevent any count down on the win screen.
            if (GameManager.instance.lives == 3)
            {
                StartCoroutine(CountDownRoutine());
                Invoke("EnableMovement", 4f);
            }
            else
            {
                Invoke("EnableMovement", 1f);
            }
        }
    }

    void EnableMovement()
    {
        canMove = true;
    }

    IEnumerator CountDownRoutine()
    {
        countDownText = GameObject.Find("Canvas").transform.Find("CountDownText").GetComponent<TextMeshProUGUI>();
        countDownText.gameObject.SetActive(true);
        int countDownTimer = 3;

        while (countDownTimer > 0)
        {
            countDownText.text = countDownTimer.ToString();
            yield return new WaitForSeconds(1);
            countDownTimer--;
        }

        // Adjust the position for "GO!" text
        RectTransform rectTransform = countDownText.GetComponent<RectTransform>();
        Vector3 originalPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector3(originalPosition.x - 50, originalPosition.y, originalPosition.z); // Adjust the x value as needed

        countDownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        // Reset the position after displaying "GO!"
        rectTransform.anchoredPosition = originalPosition;
        countDownText.gameObject.SetActive(false);
    }


    int hitCount = 0;
    void Update()
    {
        if (canMove)
        {
            // Create a movement vector, scaled by speed and time.
            Vector3 movement = desiredDirection * speed * Time.deltaTime;

            // Apply movement to the position of the ball.
            transform.position += movement;

            // Apply contraint to the ball's z position.
            transform.position = new Vector3(transform.position.x, transform.position.y, initialZPos);

            // Check if ball fell below the player.
            if (transform.position.y < -5 && GameManager.instance.currentState != GameManager.GameState.Reset)
            {
                Destroy(gameObject);
                GameManager.instance.ChangeState(GameManager.GameState.Reset);
            }


            RaycastHit hit;
            if (Physics.SphereCast(transform.position, radius, desiredDirection, out hit, movement.magnitude + radius))
            {
                // Compare this tag to nearby hit collider tags.
                if (hit.collider.CompareTag(wallTag) || hit.collider.CompareTag(brickTag) || hit.collider.CompareTag(playerTag))
                {
                    Vector3 collisionNormal = hit.normal;
                    Vector3 reflectedDirection = Vector3.Reflect(desiredDirection, collisionNormal).normalized;

                    if (Mathf.Abs(reflectedDirection.y) < minVerticalSpeed)
                    {
                        reflectedDirection.y = Mathf.Sign(reflectedDirection.y) * minVerticalSpeed;
                    }

                    if (hit.collider.CompareTag(wallTag))
                    {
                        brickHit.Play();
                    }
                    else if (hit.collider.CompareTag(brickTag))
                    {
                        hit.collider.GetComponent<Brick>().HitBrick(collisionNormal);
                        brickHit.pitch = 1f;
                        brickHit.Play();
                    }
                    else if (hit.collider.CompareTag(playerTag))
                    {
                        Transform paddle = hit.collider.transform;
                        float paddleWidth = paddle.localScale.x;
                        float relativePosition = hit.point.x - paddle.position.x;
                        float hitOffset = relativePosition / (paddleWidth / 2);
                        reflectedDirection.x += hitOffset;
                        reflectedDirection = reflectedDirection.normalized;
                        paddleHit.pitch = 0.8f;
                        paddleHit.Play();
                        BallHit();
                        playerController.OnPaddleHit();
                    }

                    desiredDirection = reflectedDirection;
                    transform.position = hit.point + hit.normal * radius;
                    hitCount++;

                    if (Time.time - lastShakeTime >= shakeCooldown)
                    {
                        animator.SetTrigger("isHit");
                        StartCoroutine(ShakeScreen(hitCount));
                        lastShakeTime = Time.time;
                    }

                    // Call the method to handle the collision and instantiate the particle system
                    HandleCollision(hit.point, collisionNormal);
                }
            }
            // Clamp the ball's position to the screen position.
            Vector3 clampedPosition = new Vector3(transform.position.x, transform.position.y, initialZPos);
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -8.5f, 8.5f);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, -6f, 5.75f);
            transform.position = clampedPosition;
        }
    }

    void HandleCollision(Vector3 hitPoint, Vector3 collisionNormal)
    {
        // Instantiate the particle system at the hit point
        GameObject hitParticles = Instantiate(hitParticleSystemPrefab, hitPoint, Quaternion.LookRotation(collisionNormal));

        // Optionally, destroy the particle system after a short duration
        Destroy(hitParticles, 2f); // Adjust the duration as needed
    }


    void BallHit()
    {
        if (!isHit)
        {
            isHit = true;
            StartCoroutine(BallGlow());
        }
    }

    IEnumerator BallGlow()
    {
        float elapsedTime = 0f;
        // Duration of the glow effect
        float duration = 0.3f;

        // Handle emission over a duration to create a consistent glow effect.
        while (elapsedTime < duration)
        {
            float lerpFactor = Mathf.PingPong(elapsedTime * 2, 1);
            Color emissionColor = Color.Lerp(hitEmissionColor * emissionIntensity, originalEmissionColor, lerpFactor);
            ballMaterial.SetColor("_EmissionColor", emissionColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ballMaterial.SetColor("_EmissionColor", originalEmissionColor);
        isHit = false;
    }
    IEnumerator ShakeScreen(float shakeMultiplier)
    {
        isShaking = true;
        float elapsed = 0.0f;
        // Calculate current magnitude based off of multiplier.
        float currentShakeMagnitude = shakeMagnitude * shakeMultiplier;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1, 1f) * currentShakeMagnitude;
            float y = Random.Range(-1, 1f) * currentShakeMagnitude;

            cameraTransform.localPosition = new Vector3(x, originalCameraPosition.y, originalCameraPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        cameraTransform.localPosition = originalCameraPosition;
        isShaking = false;
        // Reset hit count to control intensity.
        hitCount = 1;
    }
}