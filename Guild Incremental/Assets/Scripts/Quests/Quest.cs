using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestCategory
{
    Main,
    Guild,
}

public enum QuestType
{
    Kill,
    Gather,
    Boss,
}

[Serializable]
public class Quest
{
    public Quest(QuestCategory cat, QuestType type)
    {
        data = null;
        category = cat;
        this.type = type;
        guild = GameObject.Find("Guild").GetComponent<Guild>();
        startTime.day = guild.CurrentTime.day;
        startTime.hour = guild.CurrentTime.hour;
        claimed = false;
    }

    public Quest(QuestData questData)
    {
        data = questData;
        category = data.category;
        type = data.type;
        guild = GameObject.Find("Guild").GetComponent<Guild>();
        
        targetLocation = guild.GetLocation(data.locationID);
        rewards = data.rewards;

        startTime.day = guild.CurrentTime.day;
        startTime.hour = guild.CurrentTime.hour;
        claimed = false;
    }

    public Guild guild;
    public QuestData data = null;
    public QuestCategory category;
    public QuestType type;
    public GameTime startTime = new GameTime();

    [SerializeReference] public Location targetLocation;
    [SerializeReference] public MonsterData targetMonster;
    [SerializeReference] public ItemData targetItem;

    public int killCount;
    public int itemCount;
    public int currentCount = 0;
    public int minLevel = 1;
    public int maxLevel = 1;

    public bool objectiveMet { get; private set; }
    public bool claimed { get; private set; }
    [SerializeReference] public Adventurer adventurer = null;
    private GameTime claimTime = new GameTime();

    public int rewardGold = 10;
    public int rewardRenown = 0;
    public List<Resource> rewards = new List<Resource>();

    public bool Claim(Adventurer adventurer)
    {
        if (claimed) return false;

        this.adventurer = adventurer;
        objectiveMet = false;
        claimed = true;
        claimTime.day = guild.CurrentTime.day;
        claimTime.hour = guild.CurrentTime.hour;

        return true;
    }

    public void SetObjective(Location loc, MonsterData monster, int count)
    {
        targetLocation = loc;
        targetMonster = monster;
        killCount = count;
        minLevel = loc.data.minLevel;
        maxLevel = loc.data.maxLevel;
    }

    public void SetObjective(Location loc, ItemData item, int count)
    {
        targetLocation = loc;
        targetItem = item;
        itemCount = count;
    }

    public void AddReward(ResourceType type, int value)
    {
        rewards.Add(new Resource(type, value));
    }

    public bool UpdateStatus(MonsterData monster, int count = 1)
    {
        if (type != QuestType.Kill) return objectiveMet;
        if (monster != targetMonster) return objectiveMet;
        currentCount += count;
        if (currentCount >= killCount) objectiveMet = true;
        return objectiveMet;
    }

    public bool UpdateStatus(ItemData item, int count)
    {
        if (type != QuestType.Gather) return objectiveMet;
        if (item != targetItem) return objectiveMet;
        currentCount = count;
        if (currentCount >= itemCount) objectiveMet = true;
        return objectiveMet;
    }

    public override string ToString()
    {
        string output = "";
        //output += "[" + category + "] ";
        output += targetLocation.data.Name + " - ";

        if (data != null)
        {
            output += data.questDescription;
            return output;
        }

        switch (type)
        {
            case QuestType.Kill:
                output += "Slay ";
                if (claimed)
                {
                    output += targetMonster.Name + "s ";
                    output += Math.Min(currentCount, killCount).ToString() + "/" + killCount.ToString();
                }
                else output += killCount.ToString() + " " + targetMonster.Name + "s";

                break;
            case QuestType.Gather:
                output += "Collect ";
                if (claimed)
                {
                    output += targetItem.Name + "s ";
                    output += Math.Min(currentCount, itemCount).ToString() + "/" + itemCount.ToString();
                }
                else output += itemCount.ToString() + " " + targetItem.Name + "s";

                break;
        }
        //if (claimed) output += " (claimed)";

        //output += "\nReward:" + (rewardGold > 0 ? " " + rewardGold.ToString() + " Gold" : "") + (rewardRenown > 0 ? " " + rewardRenown.ToString() + " Renown" : "");

        return output;
    }
}
