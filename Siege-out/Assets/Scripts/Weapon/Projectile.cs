using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject ParticleEffect;
    private float damage;
    private GameObject target;
    private LayerMask obstacleMask;
    private GameObject enemy;
    private float speed;
    private Rigidbody rb;
    
    private float AOE;

    private void FixedUpdate()
    {

        // Apply force only when in StraightForward state
        if (projectileType == ProjectileType.StraightForward)
        {
           
        }
    }
    public void SetSpeed(float Speed)
    {
        speed = Speed;
    }
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    public void SetAOE(bool isAoe, float Aoe)
    {
        if(isAoe == true)
        {
            AOE = Aoe;
        }
    }
    public void SetTarget(GameObject target, GameObject enemy)
    {
        this.target = target;
        this.enemy = enemy;
    }

    public void SetObstacleMask(LayerMask obstacleMask)
    {
        this.obstacleMask = obstacleMask;
    }

    public void InitializeProjectile(EnemyController enemyController, float proSpeed)
    {
        EnemyController ec = enemy.GetComponent<EnemyController>();
        EnemyController.ProjectileType enemyAttackType = ec.CurrentAttackType;
        projectileType = (ProjectileType)enemyAttackType; // explicit cast
        speed = proSpeed;
    }

    public void ShootStraight(float maxTime)
    {
        if (projectileType == ProjectileType.StraightForward)
        {
            StartCoroutine(MoveStraight(maxTime));
        }
    }

    public void ShootWithInstantForce(float maxTime)
    {
        
        if (projectileType == ProjectileType.InstantForce)
        {
            rb = GetComponent<Rigidbody>();
            Transform targetTransform = target.transform;
            Vector3 direction = (targetTransform.position - transform.position).normalized;
            rb.AddForce(direction * speed, ForceMode.Impulse);
            Destroy(gameObject, maxTime);
        }
    }

    private IEnumerator MoveStraight(float maxTime)
    {
        Transform targetTransform = target.transform;
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        rb = GetComponent<Rigidbody>();

        float timeElapsed = 0f;
        rb.useGravity = false;
        while (timeElapsed < maxTime)
        {
            rb.AddForce(direction * speed * Time.deltaTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject, maxTime);
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            Debug.Log("Wall Hit");
            if (ParticleEffect && AOE > 0)
            {
                if (ParticleEffect != null)
                {
                    GameObject particleObject = Instantiate(ParticleEffect, transform.position, Quaternion.identity);

                    // Detach the particle effect from the projectile so it can continue playing
                    particleObject.transform.parent = null;
                    Destroy(particleObject, 5);
                }
            }
            Collider[] colliders = Physics.OverlapSphere(transform.position, AOE);

            foreach (Collider collider in colliders)
            {
                
                // Check if the collider has a component that implements the IDamageable interface
                if (collider.gameObject == target)
                {
                    IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();

                    if (damageable != null)
                    {
                        // Calculate the distance between the hit point and the target's position
                        float distance = Vector3.Distance(transform.position, collider.gameObject.transform.position);

                        if (distance <= AOE)
                        {
                            // If the target is within the area of effect, deal damage to it
                            damageable.TakeDamage(damage);
                        }
                    }
                }
            }
            Destroy(gameObject);
        }
        else if (other.gameObject == target)
        {
            GameObject particleObject = Instantiate(ParticleEffect, transform.position, Quaternion.identity);

            // Detach the particle effect from the projectile so it can continue playing
            particleObject.transform.parent = null;
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {

                    damageable.TakeDamage(damage);

                Destroy(gameObject);
            }
        }
    }

    public enum ProjectileType
    {
        StraightForward,
        InstantForce
    }

    private ProjectileType projectileType;
    public ProjectileType Type => projectileType;
}