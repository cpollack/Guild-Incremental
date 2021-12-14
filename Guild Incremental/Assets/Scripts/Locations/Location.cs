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

        int count = 0;
        switch (questType)
        {
            case QuestType.Kill:
                MonsterData monster = data.monsters[UnityEngine.Random.Range(0, data.monsters.Count)];

                int min = 5;
                int max = 20;
                int range = max - min;
                int challengeValue = range / 3;                

                count = UnityEngine.Random.Range(5, 20);
                quest.SetObjective(this, monster, count);
                int difficulty = (count - min) / challengeValue;

                float renownReward = 1 * (0.9f + (0.1f * (float)difficulty));
                renownReward = Math.Max(renownReward, 1);
                float goldReward = 10 * (0.9f + (0.1f * (float)difficulty));

                quest.AddReward(ResourceType.Renown, (int)renownReward);
                quest.AddReward(ResourceType.Gold, (int)goldReward);

                break;
            case QuestType.Gather:
                ItemData item = data.gatherables[UnityEngine.Random.Range(0, data.gatherables.Count)];
                count = UnityEngine.Random.Range(5, 20);
                quest.SetObjective(this, item, count);
                break;

            default:
                Debug.LogError("Location:GenerateRandomQuest() - Unhandled quest type: " + questType);
                break;
        }

        return quest;
    }
}
