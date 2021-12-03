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

        if (HasStateLengthBeenFulfilled())
        {
            switch (deathStatus)
            {
                case DeathStatus.Revive:
                    ResetStartTime();
                    Adventurer.SetActionText("Recuperating.");
                    stateLength.Set(0, 1);
                    deathStatus = DeathStatus.Recover;
                    break;

                case DeathStatus.Recover:
                    Adventurer.ResetLife();
                    return typeof(StateIdle);
            }            
        }

        

        return null;
    }

    public override void OnStateChange()
    {
        base.OnStateChange();
        Adventurer.currentLocation = null;
        Adventurer.SetActionText("Reviving.");
        stateLength.Set(0, 1);
        deathStatus = DeathStatus.Revive;
    }
}
