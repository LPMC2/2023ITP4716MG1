using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    private ParticleSystem particleSystems;

    void Start()
    {
        particleSystems = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // Set the rotation of the particle system to always face x=90
        Quaternion rotation = Quaternion.Euler(90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        particleSystems.transform.rotation = rotation;
    }
}