using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticleBehaviour : MonoBehaviour
{
    private HealthBehaviour healthBehaviour;
    public int maxHits = 1; // The maximum number of hits allowed per collider
    public bool isActive = false; // Whether the particle system is active or not
    private float currentCooldown = 0f;
    private bool hasDamagedThisFrame = false;
    private Dictionary<Collider, int> hitCounts = new Dictionary<Collider, int>();
    private GunController gunController;

    void Start()
    {
        // Get the GunController component from the parent object
        gunController = GetComponentInParent<GunController>();

        // Set the hitCooldown based on the shootingTime of the GunController
        if (!gunController)
        {
            hitCooldown = gunController.GetShootingTime();
        }
    }

    // The amount of time to wait before allowing another hit on the same collider
    private float hitCooldown;

    void OnTriggerStay(Collider other)
    {
        Debug.Log("GotHit!");
        if (isActive && other.CompareTag("Enemy") && !hasDamagedThisFrame && currentCooldown <= 0f)
        {
            // Check if this collider has already been hit the maximum number of times
            if (hitCounts.ContainsKey(other) && hitCounts[other] >= maxHits)
            {
                return;
            }

            // Damage the enemy or apply any other desired effect
            healthBehaviour = other.gameObject.GetComponent<HealthBehaviour>();
            healthBehaviour.TakeDamage(gunController.GetDamage());
            hasDamagedThisFrame = true;

            // Add this collider to the hitCounts dictionary if it's not already there
            if (!hitCounts.ContainsKey(other))
            {
                hitCounts.Add(other, 1);
            }
            // Otherwise, increment the hit count for this collider
            else
            {
                hitCounts[other]++;
            }

            // Start the hit cooldown timer
            currentCooldown = hitCooldown;
        }
    }

    void LateUpdate()
    {
        hasDamagedThisFrame = false;

        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
        }

        // Clear the hitCounts dictionary for any colliders that haven't been hit recently
        List<Collider> keysToRemove = new List<Collider>();
        foreach (KeyValuePair<Collider, int> pair in hitCounts)
        {
            if (currentCooldown <= 0f || pair.Value >= maxHits)
            {
                keysToRemove.Add(pair.Key);
            }
        }
        foreach (Collider key in keysToRemove)
        {
            hitCounts.Remove(key);
        }
    }
}