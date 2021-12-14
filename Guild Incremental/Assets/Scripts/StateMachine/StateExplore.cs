using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateExplore : AdventurerBaseState
{
    public StateExplore(Adventurer adventurer) : base(adventurer)
    {
        //
    }

    private Location location;

    private const float encounterTimer = 0.25f;

    public override Type Tick()
    {     
        if (HasStateLengthBeenFulfilled())
        {
            if (Guild.timeOfDay == TimeOfDay.Evening || Guild.timeOfDay == TimeOfDay.Night)
            {
                Adventurer.targetLocationID = "";
                Adventurer.targetLocation = null; //Return home
                return typeof(StateTravel);
            }

            Type newState = HandleEncounterByType(location.RollEncounter());
            
            if (newState != null)
            {
                return newState;
            }

            ResetStartTime();
        }

        /*float elapsedHours = Guild.ElapsedTimeHours(stateStartTime);
        float actionPerc = elapsedHours / stateLengthHours;
        Adventurer.SetActionPercent(actionPerc);

        if (actionPerc >= 1)
        {
            if (Adventurer.targetLocation != null) return typeof(StateExplore);
            //else return typeof(StateReturnToGuild);
        }*/

        return null;
    }
     
    public override void OnStateChange()
    {
        base.OnStateChange();
        location = Adventurer.currentLocation;
        Adventurer.SetActionText("Exploring " + location.data.Name);
        Adventurer.HideActionPercent();
        stateLength.Set(0, encounterTimer);
    }

    public override void Load()
    {
        base.Load();
        location = Adventurer.currentLocation;
    }

    private Type HandleEncounterByType(EncounterType encounterType)
    {
        switch (encounterType)
        {
            case EncounterType.None:
                Adventurer.SetActionText("Exploring " + location.data.Name);
                location.GainProgress(1);
                break;

            case EncounterType.Battle:
                return HandleBattle();

            case EncounterType.Resource:
                //Roll resource gain
                break;
            
        }

        return null;
    }

    private Type HandleBattle()
    {
        Battle battle = new Battle();
        battle.addAdventurer(Adventurer);
        MonsterData monster = location.RollMonster(Adventurer);
        battle.addMonster(monster);
        Adventurer.SetActionText("Battling a " + monster.Name);

        if (battle.Process())
        {
            //Victory
            battle.UpdateMonsterKills();
            battle.AwardExp();
            battle.AwardGold();
            battle.RollDrops();
            location.GainProgress(1);
        }
        else
        {
            //Defeat, either died or ran

            //if the party did not wipe, they may be able to revive fallen team mates
            return typeof(StateDeath);
        }

        return null;
    }
}
