using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private Dictionary<Type, BaseState> availableStates;

    public BaseState CurrentState { get; private set; }

    public void SetStates(Dictionary<Type, BaseState> states)
    {
        availableStates = states;
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentState == null)
        {
            CurrentState = availableStates.Values.First();
        }

        Type nextState = CurrentState?.Tick();

        if (nextState != null && nextState != CurrentState?.GetType())
        {
            ChangeState(nextState);
        }
    }

    private void ChangeState(Type newState)
    {
        CurrentState?.OnBeforeStateChange();
        CurrentState = availableStates[newState];
        CurrentState?.OnStateChange();
    }
}
