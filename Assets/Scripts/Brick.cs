using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    int hitCount = 0;

    public void HitBrick()
    {
        hitCount++;

        if (hitCount == 1)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else if (hitCount == 2)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if (hitCount == 3)
        {
            Destroy(gameObject);
        }
    }
}

