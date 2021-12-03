using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerBaseState : BaseState
{
    protected Adventurer Adventurer;
    protected Guild Guild;

    protected GameTime startTime = new GameTime();
    protected GameTime stateLength = new GameTime();

    public AdventurerBaseState(Adventurer adventurer) : base(adventurer.gameObject)
    {
        this.Adventurer = adventurer;
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
        startTime.day = Guild.currentTime.day;
        startTime.hour = Guild.currentTime.hour;
    }

    protected void UpdateActionPercent()
    {
        float elapsedHours = GetElapsedTime().GetHours();
        float actionPerc = elapsedHours / stateLength.GetHours();
        Adventurer.SetActionPercent(actionPerc);
    }

    protected void HideActionPercent()
    {
        Adventurer.SetActionPercent(-1);
    }
}
