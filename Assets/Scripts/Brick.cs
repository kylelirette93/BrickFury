using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    int hitCount = 0;
    Material brickMaterial;
    float destroyTime = 0.2f;
    float changeMaterialTime = 0.05f;
    public ParticleSystem brickExplosion;
    AudioSource brickExplosionSound;
    Animator animator;

    private void Start()
    {
        brickMaterial = GetComponent<Renderer>().material;
        animator = GetComponent<Animator>();
        brickExplosionSound = GetComponent<AudioSource>();
    }

    public void HitBrick(Vector3 direction)
    {
        hitCount++;
        animator.SetTrigger("isHit");
        if (hitCount >= 3)
        {
            DestroyBrick();
        }
        else
        {
            brickMaterial.color = Color.white;
            Invoke("ChangeMaterialColor", changeMaterialTime);
        }   
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
            // Debug.Log("Color is: " + brickMaterial.color);
        }
        else if (hitCount == 3)
        {
            DestroyBrick();
        }
    }

   

    void DestroyBrick()
    {
        brickExplosionSound.Play();
        GameObject.Instantiate(brickExplosion, transform.position, Quaternion.identity);
        Destroy(gameObject, destroyTime);
    }

   
}

