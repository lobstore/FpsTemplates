using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyStateMachine StateMachine;
    public IdleState IdleState { get; set; }
    public PatrolState PatrolState { get; set; }
    public ChaseState ChaseState { get; set; }

    public NavMeshAgent NavMeshAgent { get; set; }
    private void Start()
    {


        StateMachine = new EnemyStateMachine();

        IdleState = new IdleState(this, StateMachine);
        PatrolState = new PatrolState(this, StateMachine);
        ChaseState = new ChaseState(this, StateMachine);

        StateMachine.Initialize(PatrolState);
    }

    private void Update()
    {
        StateMachine.CurrentState.UpdateState();

    }

    public void Attack()
    {

    }
}
