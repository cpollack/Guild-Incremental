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

    public override Type Tick()
    {
        Type type = null;
        type = HandleIdleState();
        return type;

        /*switch (Guild.timeOfDay)
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
                adventurer.SetActionText("Sleeping...");
                break;
        }

        return type;*/
    }

    public override int GetSubState()
    {
        return (int)idleSubState;
    }

    public override void Load()
    {
        base.Load();
        idleSubState = (IdleSubState)adventurer.currentSubState;
    }

    public override void OnStateChange()
    {
        idleSubState = IdleSubState.None;
        adventurer.Resting = false;
    }

    private Type HandleIdleState()
    {
        adventurer.Resting = false;

        switch (idleSubState)
        {
            case IdleSubState.None:
                if (adventurer.currentQuest != null)
                {
                    if (adventurer.currentQuest.objectiveMet) idleSubState = IdleSubState.TurnIn;
                    else idleSubState = IdleSubState.Location;

                }
                else idleSubState = IdleSubState.Quest;
                break;

            case IdleSubState.TurnIn:
                adventurer.TurnInQuest();
                adventurer.SetActionText("Turning in a quest and collecting the rewards.");
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
                if (adventurer.currentQuest == null)
                {
                    if (adventurer.ChooseQuest())
                    {
                        adventurer.SetActionText("Visiting the quest board.");
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
                if (adventurer.targetLocation == null)
                {
                    if (adventurer.ChooseLocation())
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
                if (adventurer.currentQuest != null)
                {
                    if (adventurer.currentQuest.objectiveMet) idleSubState = IdleSubState.TurnIn;
                }

                if (idleSubState == IdleSubState.None)
                {
                    adventurer.SetActionText("Daydreaming about being a hero...");
                    ResetStartTime();
                    HideActionPercent();
                }

                break;

            case IdleSubState.TurnIn:
                adventurer.TurnInQuest();
                adventurer.SetActionText("Turning in a quest and collecting the rewards.");
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
                if (adventurer.currentQuest != null)
                {
                    if (adventurer.currentQuest.objectiveMet)
                        idleSubState = IdleSubState.TurnIn;
                }

                if (idleSubState == IdleSubState.None)
                {
                    adventurer.SetActionText("Loitering around the guild...");
                    ResetStartTime();
                    HideActionPercent();
                }

                break;

            case IdleSubState.TurnIn:
                adventurer.TurnInQuest();
                adventurer.SetActionText("Turning in a quest and collecting the rewards.");
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
        if (!adventurer.Resting)
        {
            ResetStartTime();
            adventurer.Resting = true;
            adventurer.HideActionPercent();
        }
        else
        {
            float elapsedTime = GetElapsedTime().GetHours();
            adventurer.Recover(elapsedTime);
            ResetStartTime();
        }
        
        return null;
    }
}
