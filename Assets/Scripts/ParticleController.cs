using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    ParticleSystem particleSystem;
    float duration;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        duration += Time.deltaTime;
        if (duration > particleSystem.main.duration) 
        {
            Destroy(gameObject);
        }
    }
}
