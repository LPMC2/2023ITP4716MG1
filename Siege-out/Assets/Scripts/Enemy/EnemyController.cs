using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float lookRadius = 10f;
    [Header("Attack System")]
    [SerializeField] public AttackType attackType;
    [SerializeField] public ProjectileType projectileType;
    [SerializeField] private float damage;
    [SerializeField] private float attackCD;
  
    [SerializeField] private GameObject projectileObject;
    private float AttackTime;
    [SerializeField] private float ProjectileSpeed = 5f;
    [SerializeField] private float AttackAngleMultiplier = 1f;
    [SerializeField] private float maxTime;
    [SerializeField]
    private LayerMask obstacleMask;
    public enum AttackType
    {
        Melee,
        Ranged
    }
    public enum ProjectileType
    {
        StraightForward,
        InstantForce
    }
    public ProjectileType CurrentAttackType { get; set; }

    Transform target;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);
            if (distance <= agent.stoppingDistance)
            {
                //Attack the target
                FaceTarget();
                StartAttack();
            }

        }



        
        
       
    }
    void FaceTarget()
    {

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    private void StartAttack()
    {

        AttackTime += Time.deltaTime;

        if (AttackTime >= attackCD)
        {

            StartCoroutine(AttackCoroutine());
            AttackTime = 0;
        }
    }

    private IEnumerator AttackCoroutine()
    {
       
        if (attackType == AttackType.Melee)
        {
            IDamageable damageable = target.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
        else if (attackType == AttackType.Ranged)
        {
            if (projectileType == ProjectileType.StraightForward)
            {

                GameObject projectileInstance = Instantiate(projectileObject, transform.position, Quaternion.identity);
                Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.SetTarget(target.gameObject, gameObject);
                    projectileScript.InitializeProjectile(gameObject.GetComponent<EnemyController>(),ProjectileSpeed);
                    
                    projectileScript.SetDamage(damage);
                    projectileScript.SetObstacleMask(obstacleMask);
                    projectileScript.ShootStraight(maxTime);
                }
            }
            else if (projectileType == ProjectileType.InstantForce)
            {
                Rigidbody rb = Instantiate(projectileObject, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                Projectile projectileScript = rb.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.SetTarget(target.gameObject, gameObject);
                    projectileScript.InitializeProjectile(gameObject.GetComponent<EnemyController>(),ProjectileSpeed);
                    rb.transform.LookAt(target.position);
                    projectileScript.SetDamage(damage);
                    projectileScript.SetObstacleMask(obstacleMask);
                    // Calculate the distance to the target
                    float distance = Vector3.Distance(rb.position, target.position);

                    // Calculate the modified speed and angle
                    float modifiedSpeed = ProjectileSpeed * Mathf.Clamp(distance / 10f, 0.5f, 2f);
                    float modifiedAngle = AttackAngleMultiplier * Mathf.Clamp(distance / 10f, 0.5f, 2f);

                    // Apply the force to the rigidbody
                    rb.AddForce(transform.forward * modifiedSpeed, ForceMode.Impulse);
                    rb.AddForce(transform.up * modifiedAngle, ForceMode.Impulse);
                }
            }

            

        }
        // Wait for the attack animation to complete before exiting the coroutine
        yield return new WaitForSeconds(attackCD);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}