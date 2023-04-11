using UnityEngine;


public class ChaseState : BaseState
{
    private Transform target;

    public override void Enter()
    {
        target = enemy.chaseTarget.transform;
        Debug.Log("Entering ChaseState");
        enemy.Agent.destination = target.position;
        enemy.Agent.speed = 3.5f;
    }

    public override void Execute()
    {
        enemy.Agent.destination = target.position;
    }

    public override void Exit()
    {
        enemy.chaseTarget.GetComponent<ChaseTarget>().SetChased(false);
        enemy.chaseTarget = null; // Reset the chaseTarget variable to null
        stateMachine.ChangeState(new PatrolState());
        Debug.Log("Changed State 1");
    }

    public override void Perform()
    {
        // Add empty implementation for the Perform() method
    }
}