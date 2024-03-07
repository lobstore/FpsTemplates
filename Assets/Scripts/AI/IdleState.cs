public class IdleState : IEnemyState
{
    private Enemy enemy;
    private EnemyStateMachine enemyStateMachine;
    public IdleState(Enemy enemy, EnemyStateMachine ems)
    {
        this.enemy = enemy;
        enemyStateMachine = ems;
    }
    public void EnterState()
    {
        
    }

    public void ExitState()
    {
        
    }

    public void UpdateState()
    {
        
    }
}
