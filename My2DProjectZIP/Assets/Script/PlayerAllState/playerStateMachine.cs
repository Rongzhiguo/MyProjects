using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStateMachine
{
    public playerState currentState { get; private set; }

    public void Initialize(playerState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(playerState _NewState)
    {
        currentState.Exit();
        currentState = _NewState;
        currentState.Enter();
    }
}
