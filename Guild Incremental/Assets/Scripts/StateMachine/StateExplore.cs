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

    private const float encounterTimer = 0.25f; //15 minutes
    private const float battleTimer = 0.0167f; //~1 minute per round
    private const int maxExploreHours = 8;

    public override Type Tick()
    {
        Type newState = null;
        //Boss Battle!
        if (adventurer.IsInBossBattle())
        {
            if (HasSecondaryLengthBeenFulfilled(battleTimer))
                newState = HandleEncounterByType(EncounterType.BossBattle);
            if (newState != null)
                ResetSecondaryTime();
            return newState;
        }

        //Return to the Guild after 8 hours of exploring
        if (HasStateLengthBeenFulfilled() && !adventurer.IsHuntingBoss())
        {            
            adventurer.targetLocationID = "";
            adventurer.targetLocation = null;
            return typeof(StateTravel);
        }

        //Run encounters
        if (HasSecondaryLengthBeenFulfilled(encounterTimer))
        {
            if (adventurer.IsHuntingBoss())
            {
                EncounterType encounterType;
                //Boss battles occure once adventurer reaches the end of the zone
                if (GetElapsedTime().GetHours() >= location.data.depthInHours)
                    encounterType = EncounterType.BossBattle;
                else encounterType = location.RollEncounter();
                newState = HandleEncounterByType(encounterType);
            }
            else newState = HandleEncounterByType(location.RollEncounter());

            ResetSecondaryTime();
            return newState;
        }

        return null;
    }

    public override void OnStateChange()
    {
        base.OnStateChange();
        location = adventurer.currentLocation;
        adventurer.SetActionText("Exploring " + location.data.Name);
        adventurer.HideActionPercent();
        stateLength.Set(0, maxExploreHours);
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
            foreach (var objective in adventurer.currentQuest.objectives)
            {
                MonsterData monsterData = Guild.GetMonsterData(objective.id); 
                if (monsterData != null) adventurer.bossBattle.addMonster(monsterData);
            }                      
        }

        adventurer.SetActionText("In an epic Boss Battle with a " + adventurer.bossBattle.GetMonsterName());
        bool battleEnded = adventurer.bossBattle.DoRound();      

        if (battleEnded)
        {
            adventurer.SetActionText("The " + adventurer.bossBattle.GetMonsterName() + " has been slain!");
            adventurer.targetLocationID = "";
            adventurer.targetLocation = null; //Return home
            return typeof(StateTravel);
        }

        return null;
    }
}
