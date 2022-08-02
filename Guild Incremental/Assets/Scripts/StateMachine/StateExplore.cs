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
    //private const float battleTimer = 0.0167f; //~1 minute per round
    private const float battleTimer = 0.0417f; //~2.5 minute per round
    private const int maxExploreHours = 8;

    public override Type Tick()
    {
        Type newState = null;
        
        if (adventurer.InBattle())
        {
            UpdateSecondaryActionPercent(battleTimer);
            if (SecondaryStateTimerMet(battleTimer))
            {
                ResetSecondaryTime();
                newState = HandleBattle();
            }            
            return newState;
        }

        //Return to the Guild after 8 hours of exploring
        if (StateTimerMet() && !adventurer.IsHuntingBoss())
            return ReturnHome();

        //Run encounters
        UpdateSecondaryActionPercent(encounterTimer);
        if (SecondaryStateTimerMet(encounterTimer))
        {
            if (adventurer.IsHuntingBoss())
            {
                EncounterType encounterType;
                //Boss battles occur once adventurer reaches the end of the zone
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
                //return HandleInstantBattle();
                return HandleBattle();

            case EncounterType.BossBattle:
                //return HandleBossBattle();
                return HandleBattle();

            case EncounterType.Resource:
                return HandleGather();

        }

        return null;
    }

    private Type HandleBattle()
    {  
        //Only the pary leader handles/updates the battle!
        //We still need the battle ended state change though
        //How should party death be handled?
        if (!adventurer.isLeader) return null;

        if (adventurer.battle == null)
        {
            Battle battle = new Battle();
            battle.addAdventurer(adventurer);
            //add team members

            MonsterData monsterData = location.RollMonster(adventurer);
            battle.addMonster(monsterData);

            //This was for boss battles. How should boss spawn be decided now?
            /*foreach (var objective in adventurer.currentQuest.objectives)
            {
                MonsterData monsterData = Guild.GetMonsterData(objective.id);
                if (monsterData != null) battle.addMonster(monsterData);
            }*/

            Guild.StartBattle(battle);
            return null;
        }

        if (adventurer.battle.BattleEnd())
        {
            if (!adventurer.battle.DidWin())
            {
                //check if ran out of rounds
                adventurer.SetActionText("[DEFEAT] Slain by " + adventurer.battle.GetMonsterName() + ".");
                Guild.EndBattle(adventurer.battle);
                return typeof(StateDeath);
            }

            adventurer.SetActionText("[VICTORY] The " + adventurer.battle.GetMonsterName() + " has been slain!");

            //Victory
            adventurer.battle.UpdateMonsterKills();
            adventurer.battle.AwardExp();
            adventurer.battle.RollDrops();
            Guild.EndBattle(adventurer.battle);
            location.GainProgress(1);

            if (adventurer.currentQuest != null)
            {
                if (adventurer.currentQuest.objectiveMet) return ReturnHome();
            }
            return null;
        }

        //adventurer.SetActionText("In an epic Boss Battle with a " + adventurer.bossBattle.GetMonsterName());
        adventurer.battle.DoRound();
        return null;
    }

    private Type HandleInstantBattle()
    {
        Battle battle = new Battle();
        battle.addAdventurer(adventurer);
        MonsterData monster = location.RollMonster(adventurer);
        battle.addMonster(monster);
        adventurer.SetActionText("Battling a " + monster.Name);

        if (!battle.Process())
        { 
            //Defeat, either died or ran
            //if the party did not wipe, they may be able to revive fallen team mates
            return typeof(StateDeath);
        }

        //Victory
        battle.UpdateMonsterKills();
        battle.AwardExp();
        battle.RollDrops();
        location.GainProgress(1);

        if (adventurer.currentQuest != null)
        {
            if (adventurer.currentQuest.objectiveMet) return ReturnHome();
        }

        return null;
    }

    private Type HandleGather()
    {
        ItemData item = location.RollGather(adventurer);
        if (item != null)
        {
            adventurer.SetActionText("Gathering a " + item.Name);
            adventurer.GainItem(item);
        }
        
        if (adventurer.currentQuest != null)
        {
            if (adventurer.currentQuest.objectiveMet) return ReturnHome();
        }

        return null;
    }

    private Type ReturnHome()
    {
        adventurer.targetLocationID = "";
        adventurer.targetLocation = null;
        return typeof(StateTravel);
    }
}
