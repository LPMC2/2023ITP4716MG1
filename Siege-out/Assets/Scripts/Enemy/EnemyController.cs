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
    [SerializeField] private float preAttackCD;
    [SerializeField] private float attackCD;
  
    [SerializeField] private GameObject projectileObject;
    private float AttackTime;
    [SerializeField] private float ProjectileSpeed = 5f;
    [SerializeField] private float AttackAngleMultiplier = 1f;
    [SerializeField] private float maxTime;
    [SerializeField] private Vector3 ProjectileOffset;
    [SerializeField] private bool isAreaDamage = false;
    [SerializeField] private float AOERadius = 1f;
    [SerializeField]
    private LayerMask obstacleMask;
    private Animator enemyAnimator;
    private GameObject nearestDestructible;
    [Header("Animation & Sound")]
    [SerializeField] private GameObject AnimateObject;
    [SerializeField] private AudioClip AttackSound;
    [SerializeField] private AudioClip DeathSound;
    private AudioSource audioSource;
    private float closestDistance;
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
    public void changeAttackType(int type)
    {
        switch (type) {
            case 1:
        
                attackType = AttackType.Melee;
                break;
            case 2:
                attackType = AttackType.Ranged;
                break;
        }
    }
    Transform target;
    NavMeshAgent agent;
    private GameObject Siege1;
    private GameObject Siege2;
    HealthBehaviour healthBehaviour;
    // Start is called before the first frame update
    void Start()
    {
        healthBehaviour = GetComponent<HealthBehaviour>();
        Siege1 = GameObject.Find("Siege-Mode1");
        Siege2 = GameObject.Find("Siege-Mode2");
        audioSource = GetComponent<AudioSource>();
        if(Siege1 != null && Siege1.activeSelf == true)
        {
            target = Siege1.transform;
        } else if (Siege1 != null && Siege2.activeSelf == true)
        {
            target = Siege2.transform;
        } else
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        if (AnimateObject != null)
        {
            enemyAnimator = AnimateObject.GetComponent<Animator>();
        }
        InvokeRepeating("DetectDestructables", 0f, 1f);
    }
    public void setAggro(GameObject originTarget)
    {
        target = originTarget.transform;
    }
    public void setAttackData(float switchDamage, float switchAttackCD, float switchPreAttackCD)
    {
        damage = switchDamage;
        attackCD = switchAttackCD;
        preAttackCD = switchPreAttackCD;
    }
    public float getDamage()
    {
        return damage;
    }
    public float getAttackCD()
    {
        return attackCD;
    }
    public float getPreAttackCD()
    {
        return preAttackCD;
    }
    public Transform getTarget()
    {
        return target;
    }
    public void playDeathSound()
    {
        if (DeathSound != null && audioSource != null)
        {
            audioSource.clip = DeathSound;
            audioSource.Play();
        }
    }
    private void DetectDestructables()
    {
        Destructable[] destructibles = FindObjectsOfType<Destructable>();
        closestDistance = agent.stoppingDistance;
        foreach (Destructable destructable in destructibles)
        {
            float enemydistance = Vector3.Distance(transform.position, destructable.transform.position);

            if (enemydistance <= closestDistance)
            {
                closestDistance = enemydistance;
                nearestDestructible = destructable.gameObject;

            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            target = PlayerManager.instance.player.transform;
        }
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);
            if (distance <= agent.stoppingDistance ||(nearestDestructible != null && (Vector3.Distance(transform.position, nearestDestructible.transform.position) <= closestDistance)))
            {

                //Attack the target
                FaceTarget();
                StartAttack();
            } else if (distance > agent.stoppingDistance)
            {
                if (AnimateObject != null)
                {
                    if (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                    {
                        StartCoroutine(PlayWalkAnimation());
                    }
                }
            }
        }
        else
        {
            if (enemyAnimator != null)
            {
                enemyAnimator.Play("Idle");
            }
        }
    }
    private IEnumerator PlayWalkAnimation()
    {
        while (true)
        {
            if (enemyAnimator != null)
            {
                // Play the "Walk" animation clip
                float distance = Vector3.Distance(target.position, transform.position);
                if (distance > agent.stoppingDistance)
                {
                    
                    if (healthBehaviour.GetHealth() > 0)
                    {
                        enemyAnimator.Play("Walk");
                    }
                }
            }

            // Wait until the animation clip ends
            yield return new WaitForSeconds(enemyAnimator.GetCurrentAnimatorStateInfo(0).length);
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
                if (DeathSound != null && audioSource != null)
                {
                    audioSource.clip = AttackSound;
                    audioSource.Play();
                }
            if (enemyAnimator != null)
            {
                // Play the "Walk" animation clip
             
                float speedMultiplier = 1.0f / attackCD;
                enemyAnimator.SetFloat("SpeedMultiplier", speedMultiplier);
                enemyAnimator.Play("AttackMelee");
            }
            yield return new WaitForSeconds(preAttackCD);
            IDamageable damageable = target.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            if (nearestDestructible != null)
            {
                Destructable destructable = nearestDestructible.GetComponent<Destructable>();
                if (destructable != null)
                {
                    destructable.takeDamage(damage);
                }
            }

        }
        else if (attackType == AttackType.Ranged)
        {
                if (DeathSound != null && audioSource != null)
                {
                    audioSource.clip = AttackSound;
                    audioSource.Play();
                }
            if (enemyAnimator != null)
            {
                // Play the "Walk" animation clip
                
                float speedMultiplier = 1.0f / attackCD;
                enemyAnimator.SetFloat("SpeedMultiplier", speedMultiplier);
                
                enemyAnimator.Play("AttackRanged");
            }
            yield return new WaitForSeconds(preAttackCD);
            if (projectileType == ProjectileType.StraightForward)
            {

                GameObject projectileInstance = Instantiate(projectileObject, transform.position + transform.TransformDirection(ProjectileOffset), Quaternion.identity);
                Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.SetTarget(target.gameObject, gameObject);
                    projectileInstance.transform.LookAt(target.position);
                    projectileScript.InitializeProjectile(gameObject.GetComponent<EnemyController>(),ProjectileSpeed);
                    projectileScript.SetSpeed(ProjectileSpeed);
                    projectileScript.setDestructable(nearestDestructible);
                    projectileScript.SetDamage(damage);
                    projectileScript.SetAOE(isAreaDamage, AOERadius);
                    projectileScript.SetObstacleMask(obstacleMask);
                    projectileScript.ShootStraight(maxTime);
                }
            }
            else if (projectileType == ProjectileType.InstantForce)
            {
                Rigidbody rb = Instantiate(projectileObject, transform.position + transform.TransformDirection(ProjectileOffset), Quaternion.identity).GetComponent<Rigidbody>();
                Projectile projectileScript = rb.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.SetTarget(target.gameObject, gameObject);
                    projectileScript.InitializeProjectile(gameObject.GetComponent<EnemyController>(), ProjectileSpeed);
                    projectileScript.setDestructable(nearestDestructible);
                    projectileScript.SetDamage(damage);
                    projectileScript.SetAOE(isAreaDamage, AOERadius);
                    projectileScript.SetObstacleMask(obstacleMask);

                    // Calculate the direction and distance to the target
                    Vector3 direction = (target.position - transform.position).normalized;
                    float distance = Vector3.Distance(rb.position, target.position);

                    // Calculate the height difference between the projectile and the target
                    float heightDifference = target.position.y - rb.position.y;

                    // Calculate the modified speed and angle based on the distance and height difference
                    float modifiedSpeed = ProjectileSpeed * Mathf.Clamp(distance / 10f, 0.5f, 2f);
                    float modifiedAngle = AttackAngleMultiplier * Mathf.Clamp(distance / 10f, 0.5f, 2f) + Mathf.Atan(heightDifference / distance) * Mathf.Rad2Deg;

                    // Set the speed and rotation of the projectile
                    projectileScript.SetSpeed(ProjectileSpeed);
                    rb.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                    rb.transform.Rotate(modifiedAngle, 0, 0);

                    // Apply the force to the rigidbody
                    Vector3 force = direction * modifiedSpeed + Vector3.up * Mathf.Tan(modifiedAngle * Mathf.Deg2Rad) * modifiedSpeed;
                    rb.AddForce(force, ForceMode.Impulse);
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