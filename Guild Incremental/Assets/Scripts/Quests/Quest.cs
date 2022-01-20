using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestCategory
{
    Main,
    Guild,
    Project,
}

public enum QuestType
{
    Kill,
    Gather,
    Boss,
}

[Serializable]
public class QuestObjective
{
    public QuestObjective(string id, string name, ResourceType type, int count, int current = 0)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.count = count;
        this.current = current;
    }
    public string id;
    public string name;
    public ResourceType type;
    public int count;
    public int current;
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
            MonsterData monsterData = guild.GetMonsterData(data.monsterId);
            if (monsterData != null)
            {
                objectives.Add(new QuestObjective(data.monsterId, monsterData.Name, ResourceType.Monster, questData.count));
            }
        }
        if (data.itemID.Length > 0)
        {
            ItemData itemData = guild.GetItemData(data.itemID);
            if (itemData != null)
            {
                objectives.Add(new QuestObjective(data.itemID, itemData.Name, itemData.resourceType, questData.count));
            }
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
    
    public List<QuestObjective> objectives = new List<QuestObjective>();

    public string projectSource = "";
    public string projectID = "";

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

        if (category == QuestCategory.Main)
        {
            adventurer.assignedQuest = this;
            adventurer.assignedQuestID = questID;
        }

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
        objectives.Add(new QuestObjective(monster.monsterID, monster.name, ResourceType.Monster, count));
        minLevel = loc.data.minLevel;
        maxLevel = loc.data.maxLevel;
    }

    public void SetObjective(Location loc, ItemData item, int count)
    {
        targetLocationID = loc.data.locationID;
        targetLocation = loc;
        objectives.Add(new QuestObjective(item.itemID, item.Name, item.resourceType, count));
    }

    public void AddReward(ResourceType type, int value)
    {
        rewards.Add(new Resource(type, value));
    }

    public bool UpdateStatus(MonsterData monster, int count = 1)
    {
        if (type != QuestType.Kill) return objectiveMet;

        for (int i = 0; i < objectives.Count; i++)
        {
           if (objectives[i].id == monster.monsterID)
            {
                objectives[i].current += count;
                if (objectives[i].current > objectives[i].count) objectives[i].current = objectives[i].count;
                break;
            }
        }

        UpdateObjectiveState();
        return objectiveMet;
    }

    public bool UpdateStatus(ItemData item, int count)
    {
        if (type != QuestType.Gather) return objectiveMet;

        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i].id == item.itemID)
            {
                objectives[i].current += count;
                if (objectives[i].current > objectives[i].count) objectives[i].current = objectives[i].count;
                break;
            }
        }

        UpdateObjectiveState();
        return objectiveMet;
    }

    public void UpdateObjectiveState()
    {
        objectiveMet = true;
        foreach (QuestObjective obj in objectives)
        {
            if (obj.current < obj.count) { objectiveMet = false; break; }
        }
    }

    public override string ToString()
    {
        string output = "";       

        if (data != null)
        {
            output += data.questDescription;
            return output;
        }

        if (category == QuestCategory.Project)
            return GetProjectString();

        int count = 1;
        switch (type)
        {
            case QuestType.Kill:
                output += targetLocation.data.Name + " - Slay ";

                count = 1;
                foreach (QuestObjective obj in objectives)
                {
                    MonsterData monsterData = guild.GetMonsterData(obj.id);
                    if (monsterData == null) continue;

                    if (count > 1) output += " and ";
                    if (claimed)
                    {
                        output += monsterData.Name + "s ";
                        output += obj.current.ToString() + "/" + obj.count.ToString();
                    }
                    else output += obj.count.ToString() + " " + monsterData.Name + "s";
                    count++;
                }

                break;
            case QuestType.Gather:
                if (targetLocation != null) output += targetLocation.data.Name + " - ";
                output += "Collect ";

                count = 1;
                foreach (QuestObjective obj in objectives)
                {
                    ItemData itemData = guild.GetItemData(obj.id);
                    if (itemData == null) continue;

                    if (count > 1) output += " and ";
                    if (claimed)
                    {
                        output += itemData.Name + "s ";
                        output += obj.current.ToString() + "/" + obj.count.ToString();
                    }
                    else output += obj.count.ToString() + " " + itemData.Name + "s";
                    count++;
                }

                break;
        }

        return output;
    }

    public string GetProjectString()
    {
        if (category != QuestCategory.Project)
            return "";

        string output = "";
        switch (projectSource)
        {
            case "Tavern":
                TavernRecipeData tavernRecipeData = guild.GetTavernRecipeData(projectID);
                if (tavernRecipeData != null) 
                {
                    output = "Recipe Research: " + tavernRecipeData.name;
                    output += "\n\nIngredients:";
                    /*foreach (var objective in objectives)
                    {
                        output += "\n" + objective.name + " " + objective.current.ToString() + "/" + objective.count.ToString();
                    }*/
                }

                break;

            default:
                Debug.LogError("Quest::GetProjectString unhandled project source [" + projectSource + "]");
                break;
        }

        return output;
    }
}
