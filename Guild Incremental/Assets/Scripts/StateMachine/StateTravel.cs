using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTravel : AdventurerBaseState
{
    private static float MinimumTravelTime = 0.5f; //Half an hour

    public StateTravel(Adventurer adventurer) : base(adventurer)
    {
        //
    }

    public override Type Tick()
    {        
        adventurer.SetActionText((adventurer.targetLocation != null ? "Traveling to " + adventurer.targetLocation.data.Name : "Returning from " + adventurer.currentLocation.data.Name));

        UpdateActionPercent();
        if (StateTimerMet())
        {
            if (adventurer.targetLocation != null) return typeof(StateExplore);
            else return typeof(StateIdle);
        }

        return null;
    }

    public override void OnStateChange()
    {
        base.OnStateChange();

        //At a location, return home
        if (adventurer.currentLocation != null)
        {
            stateLength.hour = adventurer.currentLocation.data.distance * adventurer.GetTravelSpeed();
        }
        else
        {
            stateLength.hour = adventurer.targetLocation.data.distance * adventurer.GetTravelSpeed();
        }

        if (stateLength.hour < MinimumTravelTime) stateLength.hour = MinimumTravelTime;
    }

    public override void OnBeforeStateChange()
    {
        adventurer.currentLocationID = adventurer.targetLocationID;
        adventurer.currentLocation = adventurer.targetLocation;        
        adventurer.targetLocationID = "";
        adventurer.targetLocation = null;
    }
}
