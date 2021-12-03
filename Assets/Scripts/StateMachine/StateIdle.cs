using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : AdventurerBaseState
{
    public StateIdle(Adventurer adventurer) : base(adventurer)
    {
        //
    }

    private enum IdleSubState
    {
        None,
        TurnIn,
        WaitTurnIn,
        Quest,
        WaitQuest,
        Location,
    }
    private IdleSubState idleSubState = IdleSubState.None;
    private bool resting = false;

    public override Type Tick()
    {
        Type type = null;
        switch (Guild.timeOfDay)
        {
            case TimeOfDay.Morning:
                type = HandleMorning();
                break;

            case TimeOfDay.Afternoon:
                type = HandleAfternoon();
                break;

            case TimeOfDay.Evening:
                type = HandleEvening();
                break;

            case TimeOfDay.Night:
                type = HandleNight();
                Adventurer.SetActionText("Sleeping...");
                break;
        }

        return type;
    }

    public override void OnStateChange()
    {
        idleSubState = IdleSubState.None;
        resting = false;
    }

    private Type HandleMorning()
    {
        resting = false;

        switch (idleSubState)
        {
            case IdleSubState.None:
                if (Adventurer.currentQuest != null)
                {
                    if (Adventurer.currentQuest.objectiveMet) idleSubState = IdleSubState.TurnIn;
                    else idleSubState = IdleSubState.Location;

                }
                else idleSubState = IdleSubState.Quest;
                break;

            case IdleSubState.TurnIn:
                Adventurer.TurnInQuest();
                Adventurer.SetActionText("Turning in a quest and collecting the rewards.");
                ResetStartTime();
                stateLength.hour = 0.25f;
                idleSubState = IdleSubState.WaitTurnIn;
                break;

            case IdleSubState.WaitTurnIn:
                if (HasStateLengthBeenFulfilled())
                {
                    idleSubState = IdleSubState.Quest;
                }
                break;

            case IdleSubState.Quest:
                if (Adventurer.currentQuest == null)
                {
                    if (Adventurer.ChooseQuest())
                    {
                        Adventurer.SetActionText("Visiting the quest board.");
                        ResetStartTime();
                        stateLength.hour = 0.25f;
                        idleSubState = IdleSubState.WaitQuest;
                    }
                    else idleSubState = IdleSubState.Location;
                }
                else idleSubState = IdleSubState.Location;
                break;

            case IdleSubState.WaitQuest:
                if (HasStateLengthBeenFulfilled())
                {
                    idleSubState = IdleSubState.Location;
                }
                break;

            case IdleSubState.Location:
                if (Adventurer.targetLocation == null)
                {
                    if (Adventurer.ChooseLocation())
                    {
                        return typeof(StateAdventurePrep);
                    }
                    else Debug.LogError("Adventurer failed to select a location.");
                }
                else return typeof(StateAdventurePrep);
                break;
        }
        return null;
    }

    private Type HandleAfternoon()
    {
        switch (idleSubState)
        {
            case IdleSubState.None:
                if (Adventurer.currentQuest != null)
                {
                    if (Adventurer.currentQuest.objectiveMet) idleSubState = IdleSubState.TurnIn;
                }

                if (idleSubState == IdleSubState.None)
                {
                    Adventurer.SetActionText("Daydreaming about being a hero...");
                    ResetStartTime();
                    HideActionPercent();
                }

                break;

            case IdleSubState.TurnIn:
                Adventurer.TurnInQuest();
                Adventurer.SetActionText("Turning in a quest and collecting the rewards.");
                ResetStartTime();
                stateLength.hour = 0.25f;
                idleSubState = IdleSubState.WaitTurnIn;
                break;

            case IdleSubState.WaitTurnIn:
                if (HasStateLengthBeenFulfilled())
                {
                    idleSubState = IdleSubState.None;
                }
                break;
        }

        return null;
    }

    private Type HandleEvening()
    {
        switch (idleSubState)
        {
            case IdleSubState.None:
                if (Adventurer.currentQuest != null)
                {
                    if (Adventurer.currentQuest.objectiveMet)
                        idleSubState = IdleSubState.TurnIn;
                }

                if (idleSubState == IdleSubState.None)
                {
                    Adventurer.SetActionText("Loitering around the guild...");
                    ResetStartTime();
                    HideActionPercent();
                }

                break;

            case IdleSubState.TurnIn:
                Adventurer.TurnInQuest();
                Adventurer.SetActionText("Turning in a quest and collecting the rewards.");
                stateLength.hour = 0.25f;
                idleSubState = IdleSubState.WaitTurnIn;
                break;

            case IdleSubState.WaitTurnIn:
                if (HasStateLengthBeenFulfilled())
                {
                    idleSubState = IdleSubState.None;
                }
                break;
        }

        return null;
    }

    private Type HandleNight()
    {
        if (!resting)
        {
            ResetStartTime();
            resting = true;
            Adventurer.HideActionPercent();
        }
        else
        {
            float elapsedTime = GetElapsedTime().GetHours();
            Adventurer.Recover(elapsedTime);
            ResetStartTime();
        }
        
        return null;
    }
}
