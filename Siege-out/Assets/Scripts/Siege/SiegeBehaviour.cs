using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace PathCreation.Examples
{
    
    public class SiegeBehaviour : MonoBehaviour
    {
        private PathFollower pathFollower;
        // Define the variables
        [Header("Attack Mode")]
        [SerializeField] private Mode mode;
        [Header("Main Settings")]
        [SerializeField] private GameObject SiegeModel;
        [SerializeField] private float attackSpeed;
        [SerializeField] private float attackRange;
        [SerializeField] private float Damage;
        [Header("Particle")]
        [SerializeField] private GameObject ParitcleObject;
        [SerializeField] private ParticleSystem Particle;


        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private GameObject Target;
        private float attackTimer;
        private float moveTimer;
        private enum Mode
        {
            AttackWall,
            AttackSpawner
        }
        public void setDamage(float newDamage)
        {
            Damage = newDamage;
        }
        public float getDamage()
        {
            return Damage;
        }
        private void Start()
        {
            pathFollower = GetComponent<PathFollower>();
            switch (mode)
            {
                case Mode.AttackSpawner:
                    Target = GameObject.Find("Monster Spawner");
                    break;
                case Mode.AttackWall:
                    Target = GameObject.FindWithTag("TargetWall");
                    break;
            }
        }

        private void Update()
        {
            if (Target != null)
            {
                if (Vector3.Distance(transform.position, Target.transform.position) <= attackRange)
                {

                    if (pathFollower != null)
                    {
                        pathFollower.enabled = false;
                    }
                    Attack();
                }
                else
                {
                    if(moveTimer == 0)
                    {
                        SiegeModel.GetComponent<Animator>().Play("Siege-Move");
                    }
                    moveTimer += Time.deltaTime;

                    if(moveTimer >= 0.99f) 
                    {
                        SiegeModel.GetComponent<Animator>().Play("Siege-Idle");
                        moveTimer = 0;
                    }
                    
                }
            }
            else
            {
                SiegeModel.GetComponent<Animator>().Play("Siege-Idle");
                if (ParitcleObject != null && Particle != null)
                {
                    ParitcleObject.SetActive(false);
                }
            }
        }

        private bool hasRun = false;
        private bool hasStart = false;
        private void Attack()
        {
            attackTimer += Time.deltaTime;
            if (!hasStart)
            {
                switch (mode)
                {
                    case Mode.AttackSpawner:
                        SiegeModel.GetComponent<Animator>().Play("Siege-Idle");
                        if (ParitcleObject != null && Particle != null)
                        {
                            ParitcleObject.SetActive(true);
                            Particle.Play();
                        }
                        break;
                    case Mode.AttackWall:
                        SiegeModel.GetComponent<Animator>().Play("Siege-Attack");
                        break;
                }


                hasStart = true;
            }
            if (attackTimer >= attackSpeed / 3 && !hasRun)
            {
                switch (mode)
                {
                    case Mode.AttackSpawner:
                        
                        HealthBehaviour healthBehaviourSpawner = Target.GetComponent<HealthBehaviour>();
                        healthBehaviourSpawner.TakeDamage(Damage);
                        break;
                    case Mode.AttackWall:
                        HealthBehaviour healthBehaviour = Target.GetComponent<HealthBehaviour>();
                        healthBehaviour.TakeDamage(Damage);
                        
                        break;
                }

                
                hasRun = true;
            }
            if (attackTimer >= attackSpeed)
            {
                hasStart = false;
                hasRun = false;
                attackTimer = 0;
            }
        }
    }
  
}