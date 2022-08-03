using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRest : AdventurerBaseState
{
    public StateRest(Adventurer adventurer) : base(adventurer)
    {
        //
    }

    private const float restTimer = 0.25f;

    public override Type Tick()
    {
        UpdateActionPercent();

        adventurer.SetActionText("Taking a much needed rest.");

        if (StateTimerMet())
        {
            float elapsedTime = GetElapsedTime().GetHours();
            adventurer.Recover(elapsedTime);
            ResetStartTime();
        }

        if (adventurer.IsMaxLife()) 
            return typeof(StateIdle);

        return null;
    }

    public override void Load()
    {
        base.Load();
    }

    public override void OnStateChange()
    {
        base.OnStateChange();
        adventurer.Resting = true;
        stateLength.SetByHours(restTimer);
    }
}
