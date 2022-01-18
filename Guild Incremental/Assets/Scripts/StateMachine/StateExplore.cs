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
    private const float battleTimer = 0.0167f;

    public override Type Tick()
    {
        if (HasStateLengthBeenFulfilled())
        {
            if ((Guild.timeOfDay == TimeOfDay.Evening && !adventurer.IsHuntingBoss()) || Guild.timeOfDay == TimeOfDay.Night)
            {
                adventurer.targetLocationID = "";
                adventurer.targetLocation = null; //Return home
                return typeof(StateTravel);
            }

            Type newState = null;
            if (adventurer.IsHuntingBoss())
            {
                EncounterType encounterType;
                //Boss battles will currently always begin once evening hits
                if (Guild.timeOfDay == TimeOfDay.Evening || adventurer.IsInBossBattle()) encounterType = EncounterType.BossBattle;
                else encounterType = location.RollEncounter();
                newState = HandleEncounterByType(encounterType);
            }
            else newState = HandleEncounterByType(location.RollEncounter());

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
        location = adventurer.currentLocation;
        adventurer.SetActionText("Exploring " + location.data.Name);
        adventurer.HideActionPercent();
        stateLength.Set(0, encounterTimer);
    }

    public override void Load()
    {
        base.Load();
        location = adventurer.currentLocation;
    }

    private Type HandleEncounterByType(EncounterType encounterType)
    {
        switch (encounterType)
        {
            case EncounterType.None:
                adventurer.SetActionText("Exploring " + location.data.Name);
                location.GainProgress(1);
                break;

            case EncounterType.Battle:
                return HandleBattle();

            case EncounterType.BossBattle:
                return HandleBossBattle();

            case EncounterType.Resource:
                //Roll resource gain
                break;

        }

        return null;
    }

    private Type HandleBattle()
    {
        Battle battle = new Battle();
        battle.addAdventurer(adventurer);
        MonsterData monster = location.RollMonster(adventurer);
        battle.addMonster(monster);
        adventurer.SetActionText("Battling a " + monster.Name);

        if (battle.Process())
        {
            //Victory
            battle.UpdateMonsterKills();
            battle.AwardExp();
            //battle.AwardGold();
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

    private Type HandleBossBattle()
    {
        if (adventurer.bossBattle == null)
        {
            adventurer.bossBattle = new Battle();
            adventurer.bossBattle.addAdventurer(adventurer);
            adventurer.bossBattle.addMonster(adventurer.currentQuest.targetMonster);
        }

        adventurer.SetActionText("In an epic Boss Battle with a " + adventurer.currentQuest.targetMonster.Name);
        bool battleEnded = adventurer.bossBattle.DoRound();      
        if (battleEnded)
        {
            adventurer.SetActionText("The " + adventurer.currentQuest.targetMonster.Name + " has been slain!");
            adventurer.targetLocationID = "";
            adventurer.targetLocation = null; //Return home
            return typeof(StateTravel);
        }

        return null;
    }
}
