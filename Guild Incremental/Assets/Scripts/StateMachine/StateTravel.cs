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
        Adventurer.SetActionText((Adventurer.targetLocation != null ? "Traveling to " + Adventurer.targetLocation.data.Name : "Returning from " + Adventurer.currentLocation.data.Name));

        UpdateActionPercent();
        if (HasStateLengthBeenFulfilled())
        {
            if (Adventurer.targetLocation != null) return typeof(StateExplore);
            else return typeof(StateIdle);
        }

        return null;
    }

    public override void OnStateChange()
    {
        base.OnStateChange();

        //At a location, return home
        if (Adventurer.currentLocation != null)
        {
            stateLength.hour = Adventurer.currentLocation.data.distance * Adventurer.GetTravelSpeed();
        }
        else
        {
            stateLength.hour = Adventurer.targetLocation.data.distance * Adventurer.GetTravelSpeed();
        }

        if (stateLength.hour < MinimumTravelTime) stateLength.hour = MinimumTravelTime;
    }

    public override void OnBeforeStateChange()
    {
        Adventurer.currentLocationID = Adventurer.targetLocationID;
        Adventurer.currentLocation = Adventurer.targetLocation;        
        Adventurer.targetLocationID = "";
        Adventurer.targetLocation = null;
    }
}
