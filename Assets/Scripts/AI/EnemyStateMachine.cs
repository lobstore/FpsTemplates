using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    public IEnemyState CurrentState { get; set; }
    public void Initialize(IEnemyState startingState)
    {
        CurrentState = startingState;
        startingState.EnterState();
    }

    public void ChangeState(IEnemyState newState)
    {
        CurrentState.ExitState();

        CurrentState = newState;
        newState.ExitState();
    }
}
