using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAdventurePrep : AdventurerBaseState
{
    public StateAdventurePrep(Adventurer adventurer) : base (adventurer)
    {
        //
    }

    public override Type Tick()
    {
        adventurer.SetActionText("Preparing for an Adventure");
        float elapsedTime = GetElapsedTime().GetHours();
        float actionPerc = elapsedTime / stateLength.GetHours();
        adventurer.SetActionPercent(actionPerc);

        if (actionPerc >= 1)
        {
            return typeof(StateTravel);
        }

        return null;
    }

    public override void OnStateChange()
    {
        base.OnStateChange();
        //stateLength.hour = 1;
        stateLength.SetByMinutes(30);
    }
}
