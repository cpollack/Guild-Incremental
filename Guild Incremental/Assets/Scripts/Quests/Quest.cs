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
    public Quest(QuestCategory cat, QuestType type, Guild guild)
    {
        data = null;
        category = cat;
        this.type = type;
        this.guild = guild;
        startTime.day = guild.CurrentTime.day;
        startTime.hour = guild.CurrentTime.hour;
        claimed = false;
    }

    public Quest(QuestData questData, Guild guild)
    {
        data = questData;
        questID = data.questID;
        category = data.category;
        type = data.type;
        this.guild = guild;

        targetLocationID = data.locationID;
        targetLocation = guild.GetLocation(targetLocationID);
        if (data.monsterId.Length > 0)
        {
            targetMonsterID = data.monsterId;
            targetMonster = guild.GetMonsterData(targetMonsterID);
        }
        if (data.itemID.Length > 0)
        {
            targetItemID = data.itemID;
            targetItem = guild.GetItemData(targetItemID);
        }
        rewards = data.rewards;

        startTime.day = guild.CurrentTime.day;
        startTime.hour = guild.CurrentTime.hour;
        claimed = false;
    }

    [NonSerialized] public Guild guild;
    public string questID = "";
    [NonSerialized] public QuestData data = null;

    public QuestCategory category;
    public QuestType type;
    public GameTime startTime = new GameTime();

    public string targetLocationID = "";
    [NonSerialized] [SerializeReference] public Location targetLocation;
    public string targetMonsterID = "";
    [NonSerialized] [SerializeReference] public MonsterData targetMonster;
    public string targetItemID = "";
    [NonSerialized] [SerializeReference] public ItemData targetItem;

    public int killCount;
    public int itemCount;
    public int currentCount = 0;
    public int minLevel = 1;
    public int maxLevel = 1;

    public bool objectiveMet { get; private set; }
    public bool claimed { get; private set; }
    public string adventurerName = "";
    [NonSerialized] [SerializeReference] public Adventurer adventurer = null;
    public GameTime claimTime = new GameTime();

    public int rewardGold = 10;
    public int rewardRenown = 0;
    public List<Resource> rewards = new List<Resource>();

    public void Load()
    {
        if (questID.Length > 0 && data == null)
            data = guild.GetQuestData(questID);

        if (targetLocationID.Length > 0 && targetLocation == null)
            targetLocation = guild.GetLocation(targetLocationID);

        if (targetMonsterID.Length > 0 && targetMonster == null)
            targetMonster = guild.GetMonsterData(targetMonsterID);

        if (targetItemID.Length > 0 && targetItem == null)
            targetItem = guild.GetItemData(targetItemID);

        if (adventurerName.Length > 0 && adventurer == null)
        {
            adventurer = guild.GetAdventurer(adventurerName);
            adventurer.currentQuest = this;
        }
    }

    public bool Claim(Adventurer adventurer)
    {
        if (claimed) return false;

        this.adventurer = adventurer;
        adventurerName = adventurer.Name;
        objectiveMet = false;
        claimed = true;
        claimTime.day = guild.CurrentTime.day;
        claimTime.hour = guild.CurrentTime.hour;

        return true;
    }

    public void SetObjective(Location loc, MonsterData monster, int count)
    {
        targetLocationID = loc.data.locationID;
        targetLocation = loc;
        targetMonsterID = monster.monsterID;
        targetMonster = monster;
        killCount = count;
        minLevel = loc.data.minLevel;
        maxLevel = loc.data.maxLevel;
    }

    public void SetObjective(Location loc, ItemData item, int count)
    {
        targetLocationID = loc.data.Name;
        targetLocation = loc;
        targetItemID = item.itemID;
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
