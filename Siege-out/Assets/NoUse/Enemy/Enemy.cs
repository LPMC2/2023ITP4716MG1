using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }
    [SerializeField] private string currentState;
    public Path path;
    public float chasingRange = 10f;
    public GameObject chaseTarget; // Add this variable
    public void setCurrentState(string name)
    {
        currentState = name;
    }
    // Start is called before the first frame update
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialise();

    }
    public void CheckForPatrolState()
    {
        if (chaseTarget == null || Vector3.Distance(transform.position, chaseTarget.transform.position) > chasingRange)
        {
            stateMachine.ChangeState(new PatrolState());
        }
    }
    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, chasingRange);
        foreach (Collider collider in colliders)
        {
            ChaseTarget chaseTargetScript = collider.GetComponent<ChaseTarget>();
            if (chaseTargetScript != null && !chaseTargetScript.IsBeingChased())
            {
                chaseTargetScript.SetChased(true);
                chaseTarget = collider.gameObject; // Set the chaseTarget variable
                agent.SetDestination(chaseTarget.transform.position); // Set the agent's destination to the chaseTarget's position
                break;
            }
        }
        if (Vector3.Distance(transform.position, chaseTarget.transform.position) > chasingRange)
        {
            CheckForPatrolState();
            return;
        }
    }
}