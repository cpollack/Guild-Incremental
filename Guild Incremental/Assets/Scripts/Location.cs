using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Location
{
    public LocationData data;
    public float explored;

    public Location(LocationData locData)
    {
        data = locData;
    }

    public EncounterType RollEncounter()
    {
        if (data == null) return EncounterType.None;

        int rand = UnityEngine.Random.Range(0, 100);
        int offset = 0;
        foreach (EncounterRate encounterRate in data.encounterRates)
        {
            if (encounterRate.rate < rand + offset) return encounterRate.type;
            offset += encounterRate.rate;
        }

        return EncounterType.None;
    }

    public MonsterData RollMonster(Adventurer adventurer) 
    {
        if (data.monsters.Count == 0)
        {
            Debug.LogError("Location " + data.Name + " has no monsters.");
            return null;
        }

        List<MonsterData> pool = GetMonsterPool(adventurer);
        return pool[UnityEngine.Random.Range(0, pool.Count)];
    }

    public ItemData RollGather(Adventurer adventurer)
    {
        if (data.gatherables.Count == 0)
        {
            Debug.LogError("Location " + data.Name + " has no gatherable items.");
            return null;
        }

        //Get rate range
        int maxRate = 0;
        foreach (var gatherable in data.gatherables)
            maxRate += gatherable.weightedRate;

        int roll = UnityEngine.Random.Range(1, maxRate);

        //Get the randomly rolled item
        int offset = 0;
        foreach (var gatherable in data.gatherables)
        {
            if (gatherable.weightedRate <= roll + offset) return gatherable.item;
            offset += gatherable.weightedRate;
        }

        return null;
    }

    private List<MonsterData> GetMonsterPool(Adventurer adventurer)
    {
        List<MonsterData> pool = new List<MonsterData>();

        float perc = explored / data.maxExplore;
        foreach (MonsterData monster in data.monsters)
        {
            if (monster.explorationPercent > perc) continue;
            pool.Add(monster);
        }

        return pool;
    }

    public void GainProgress(float val)
    {
        explored += val;

        //check for resulting events
    }

    public Quest GenerateRandomQuest(Guild guild)
    {
        List<QuestType> availTypes = new List<QuestType>();
        if (data.monsters.Count > 0) availTypes.Add(QuestType.Kill);
        if (data.gatherables.Count > 0) availTypes.Add(QuestType.Gather);

        if (availTypes.Count == 0) return null;

        QuestType questType = availTypes[UnityEngine.Random.Range(0, availTypes.Count)];
        Quest quest = new Quest(QuestCategory.Guild, questType, guild);
        quest.targetLocationID = data.locationID;
        quest.targetLocation = this;

        int count = 0;
        int min, max, range, challengeValue, difficulty;
        float renownReward, meritReward;
        switch (questType)
        {
            case QuestType.Kill:
                MonsterData monster = data.monsters[UnityEngine.Random.Range(0, data.monsters.Count)];

                min = 5;
                max = 20;
                range = max - min;
                challengeValue = range / 3;                

                count = UnityEngine.Random.Range(min, max);
                quest.SetObjective(this, monster, count);
                difficulty = (count - min) / challengeValue;

                renownReward = 1 * (0.9f + (0.1f * (float)difficulty));
                renownReward = Math.Max(renownReward, 1);
                meritReward = 10 * (0.9f + (0.1f * (float)difficulty));
                meritReward = Math.Max(meritReward, 1);

                quest.AddReward(ResourceType.Renown, (int)renownReward);
                quest.AddReward(ResourceType.Merit, (int)meritReward);

                break;
            case QuestType.Gather:
                ItemData item = data.gatherables[UnityEngine.Random.Range(0, data.gatherables.Count)].item;

                min = 5;
                max = 20;
                range = max - min;
                challengeValue = range / 3;

                count = UnityEngine.Random.Range(min, max);
                quest.SetObjective(this, item, count);
                difficulty = (count - min) / challengeValue;

                renownReward = 1 * (0.9f + (0.1f * (float)difficulty));
                renownReward = Math.Max(renownReward, 1);
                meritReward = 10 * (0.9f + (0.1f * (float)difficulty));
                meritReward = Math.Max(meritReward, 1);

                quest.AddReward(ResourceType.Renown, (int)renownReward);
                quest.AddReward(ResourceType.Merit, (int)meritReward);

                break;

            default:
                Debug.LogError("Location:GenerateRandomQuest() - Unhandled quest type: " + questType);
                break;
        }

        return quest;
    }
}
