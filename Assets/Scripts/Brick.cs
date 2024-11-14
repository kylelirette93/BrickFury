using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    int hitCount = 0;
    bool hasHit = false;

    private void Start()
    {
        // Set the color of the brick to white.
        GetComponent<Renderer>().material.color = Color.green;
    }



    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.2f))
        {
            if (hit.collider != null && !hasHit)
            {

                hitCount++;
                hasHit = true;

                if (hitCount == 1)
                {
                    GetComponent<Renderer>().material.color = Color.yellow;
                    Invoke("DelayedReset", 0.2f);
                }
                else if (hitCount == 2)
                {
                    GetComponent<Renderer>().material.color = Color.red;
                    Invoke("DelayedReset", 0.2f);
                }
                else if (hitCount >= 3)
                {
                    Destroy(gameObject);
                }
            }
            
        }

    }

    void DelayedReset()
    {
        hasHit = false;
    }
        
      
}
