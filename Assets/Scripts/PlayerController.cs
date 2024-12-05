using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    Camera mainCamera;
    float minScreenWidth = -7.86f;
    float maxScreenWidth = 7.86f;

    private Material paddleMaterial;
    private Color originalEmissionColor;
    public Color hitEmissionColor;
    public float emissionIntensity;
    private bool isHit = false;



    private void Start()
    {
        paddleMaterial = GetComponent<Renderer>().material;
        originalEmissionColor = paddleMaterial.GetColor("_EmissionColor");
    }



    void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint
            (new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));

        transform.position = new Vector3(Mathf.Clamp(mouseWorldPosition.x, minScreenWidth, maxScreenWidth), transform.position.y, transform.position.z);
    }

    public void OnPaddleHit()
    {
        if (!isHit)
        {
            isHit = true;
            StartCoroutine(PaddleGlow());
        }
    }

    IEnumerator PaddleGlow()
    {
        float elapsedTime = 0f;
        // Duration of the glow effect
        float duration = 0.5f;

        // Handle emission over a duration to create a consistent glow effect.
        while (elapsedTime < duration)
        {
            float lerpFactor = Mathf.PingPong(elapsedTime * 2, 1);
            Color emissionColor = Color.Lerp(hitEmissionColor * emissionIntensity, originalEmissionColor, lerpFactor);
            paddleMaterial.SetColor("_EmissionColor", emissionColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        paddleMaterial.SetColor("_EmissionColor", originalEmissionColor);
        isHit = false;
    }
}
