using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDeath : AdventurerBaseState
{
    public StateDeath(Adventurer adventurer) : base(adventurer)
    {
        //
    }

    private enum DeathStatus
    {
        Revive,
        Recover,
    }

    private DeathStatus deathStatus;

    public override Type Tick()
    {
        UpdateActionPercent();

        if (StateTimerMet())
        {
            switch (deathStatus)
            {
                case DeathStatus.Revive:
                    ResetStartTime();
                    adventurer.SetActionText("Recuperating.");
                    stateLength.Set(0, 1);
                    deathStatus = DeathStatus.Recover;
                    break;

                case DeathStatus.Recover:
                    adventurer.ResetLife();
                    return typeof(StateIdle);
            }            
        }

        

        return null;
    }

    public override int GetSubState()
    {
        return (int)deathStatus;
    }

    public override void Load()
    {
        base.Load();
        deathStatus = (DeathStatus)adventurer.currentSubState;
    }

    public override void OnStateChange()
    {
        base.OnStateChange();
        adventurer.currentLocation = null;
        adventurer.SetActionText("Reviving.");
        stateLength.Set(0, 1);
        deathStatus = DeathStatus.Revive;
    }
}
