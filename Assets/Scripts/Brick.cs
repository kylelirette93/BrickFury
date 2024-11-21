using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    int hitCount = 0;
    Material brickMaterial;

    private void Start()
    {
        brickMaterial = GetComponent<Renderer>().material;
    }

    public void HitBrick()
    {
        hitCount++;
        brickMaterial.color = Color.white;
        Invoke("ChangeMaterialColor", 0.05f);      
    }

    void ChangeMaterialColor()
    {
        if (hitCount == 1)
        {
            brickMaterial.color = Color.yellow;
        }
        else if (hitCount == 2)
        {
            brickMaterial.color = Color.red;
            Debug.Log("Color is: " + brickMaterial.color);
        }
        else if (hitCount == 3)
        {
            Destroy(gameObject);
        }
    }
}

