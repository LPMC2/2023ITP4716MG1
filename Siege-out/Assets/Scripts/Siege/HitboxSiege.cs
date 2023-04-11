using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxSiege : MonoBehaviour
{
    [SerializeField] private float hitboxDuration;
    [SerializeField] private float hitboxDamage;
    private bool destroyOnWallHit;

    public void Initialize(float HitboxDamage, bool destroyOnWallHit)
    {
        hitboxDamage = HitboxDamage;
        this.destroyOnWallHit = destroyOnWallHit;
        Destroy(gameObject, hitboxDuration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("TargetWall"))
        {
            HealthBehaviour healthBehaviour = other.GetComponent<HealthBehaviour>();
            healthBehaviour.TakeDamage(hitboxDamage);

            if (destroyOnWallHit && other.CompareTag("TargetWall"))
            {
                Destroy(gameObject);
            }
        }
    }
}