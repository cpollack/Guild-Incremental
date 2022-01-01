using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerBaseState : BaseState
{
    protected Adventurer adventurer;
    protected Guild Guild;

    protected GameTime startTime = new GameTime();
    protected GameTime stateLength = new GameTime();

    public AdventurerBaseState(Adventurer adventurer) : base(null)
    {
        this.adventurer = adventurer;
        this.Guild = adventurer.guild;
    }

    public override Type Tick()
    {
        return null;
    }

    public override void OnStateChange()
    {
        ResetStartTime();
    }

    protected bool HasStateLengthBeenFulfilled()
    {
        if (stateLength.GetHours() == 0) return false;
        if (Guild.GetElapsedTime(startTime) >= stateLength) return true;
        return false;
    }

    protected GameTime GetElapsedTime()
    {
        return Guild.GetElapsedTime(startTime);
    }

    protected void ResetStartTime()
    {
        startTime.day = Guild.CurrentTime.day;
        startTime.hour = Guild.CurrentTime.hour;
    }

    protected void UpdateActionPercent()
    {
        float elapsedHours = GetElapsedTime().GetHours();
        float actionPerc = elapsedHours / stateLength.GetHours();
        adventurer.SetActionPercent(actionPerc);
    }

    protected void HideActionPercent()
    {
        adventurer.SetActionPercent(-1);
    }

    public override void Save()
    {
        adventurer.stateStartTime = startTime;
        adventurer.stateLength = stateLength;
    }

    public override void Load()
    {
        startTime = adventurer.stateStartTime;
        if (startTime == null) startTime = new GameTime();
        stateLength = adventurer.stateLength;
        if (stateLength == null) stateLength = new GameTime();
    }
}
