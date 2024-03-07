using UnityEngine;

public class ChaseState : IEnemyState
{
    private Enemy enemy;
    private EnemyStateMachine enemyStateMachine;
    public ChaseState(Enemy enemy, EnemyStateMachine ems)
    {
        this.enemy = enemy;
        enemyStateMachine = ems;
    }
    public void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState()
    {
        throw new System.NotImplementedException();
    }
}
