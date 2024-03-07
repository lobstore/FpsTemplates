using UnityEngine;

public class PatrolState : IEnemyState
{
    private Enemy enemy;
    private EnemyStateMachine enemyStateMachine;
    private Vector3 destination;
    public PatrolState(Enemy enemy, EnemyStateMachine ems)
    {
        this.enemy = enemy;
        enemyStateMachine = ems;
    }
    public void EnterState()
    {
        GetRandomDestination();
        enemy.NavMeshAgent.SetDestination(destination);
    }

    public void ExitState()
    {
    }

    public void UpdateState()
    {
        if (enemy.NavMeshAgent.velocity.magnitude < 0.01f)
        {
            GetRandomDestination();
        }
    }

    public void GetRandomDestination()
    {
        destination = enemy.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 2f;
    }
}