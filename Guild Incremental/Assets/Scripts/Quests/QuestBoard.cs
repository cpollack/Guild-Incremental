using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : GuildHall
{
    public QuestBoardPanel questBoardPanel;

    public bool staffed = false;

    private Dictionary<string, QuestData> allQuests = new Dictionary<string, QuestData>();

    new private void Awake()
    {
        base.Awake();
        questBoardPanel.SetLockState(Unlocked);

        LoadResources();
    }

    private void LoadResources()
    {
        allQuests = new Dictionary<string, QuestData>();

        UnityEngine.Object[] resources = Resources.LoadAll("Quests", typeof(QuestData));
        foreach (UnityEngine.Object resource in resources)
        {
            QuestData questData = (QuestData)resource;
            allQuests.Add(questData.questID, questData);
        }
    }

    private void Update()
    {
        if (!Unlocked) return;
        UpdateQuestPool();
    }

    public override void Load()
    {
        foreach (string questID in guild.CompletedQuests)
            allQuests.Remove(questID);

        foreach (string buildID in guild.CompletedBuildings)
            CompleteBuild(buildID);

        foreach (Quest quest in guild.Quests)
            questBoardPanel.AddQuest(quest);
    }

    public override void ResetGame()
    {
        LoadResources();
        questBoardPanel.RemoveAllQuests();
    }

    public void CompleteQuest(Quest quest)
    {
        questBoardPanel.RemoveQuest(quest);
        guild.Quests.Remove(quest);        
        UpdateQuestPool();
        GenerateMainQuests();
    }

    public void UpdateQuestPool()
    {
        if (staffed)
        {
            while (GetGuildQuestCount() < guild.MaxGuildQuests)
            {
                GenerateGuildQuest();
            }
        }        
    }

    public void GenerateMainQuests()
    { 
        foreach (KeyValuePair<string, QuestData> kvp in allQuests)
        {
            QuestData data = kvp.Value;
            //Quest already active
            bool skip = false;
            foreach (Quest quest in guild.Quests)
            {
                if (quest.data != null && quest.data.questID == kvp.Key)
                {
                    skip = true;
                    break;
                }
            }
            if (skip) continue;

            //Quest already completed
            if (guild.CompletedQuests.Contains(kvp.Key))
                continue;

            //Quest unlock conditions met
            foreach (Resource resource in data.unlockRequirements)
            {
                if (!resource.HasResource(guild))
                {
                    skip = true;
                    break;
                }
            }
            if (skip) continue;

            AddQuest(data);
        }
    }

    public void IssueQuest()
    {
        if (GetGuildQuestCount() >= guild.MaxGuildQuests) return;
        GenerateGuildQuest();
    }

    private int GetGuildQuestCount()
    {
        int count = 0;
        foreach (Quest quest in guild.Quests)
        {
            if (quest.category == QuestCategory.Guild) count++;
        }
        return count;
    }

    public void GenerateGuildQuest()
    {
        //Later this should select locations based on user defined rules

        Location questLoc = guild.locations[0];
        Quest newQuest = questLoc.GenerateRandomQuest(guild);
        if (newQuest != null)
        {
            guild.Quests.Add(newQuest);
            questBoardPanel.AddQuest(newQuest);

            guild.TotalGuildQuestsIssued++;
            if (guild.TotalGuildQuestsIssued == 1)
            {
                guild.AddStoryEntry("A Bright Eyed Newbie", "As you pin the first quest to the board, a young and bright eyed man walks in." +
                    "\n\n\"Slaying rats in the sewers, eh? I'm your man for the job!\"" +
                    "\n\nHe doesn't look like much with his hand-me-down worn leathers, and rusty sword. Still, he will have to do.");
                guild.AddLogEntry("Issued first guild quest. You have attracted the attention of a new adventurer!");
                guild.SpawnFirstAdventurer();
            }
        }
        else Debug.LogError("QuestBoard:GenerateGuildQuest failed to generate a quest for " + questLoc.data.Name);
    }

    public List<Quest> GetQuests()
    {
        return guild.Quests;
    }

    public int GetMaxGuildQuests() { return guild.MaxGuildQuests; }

    public List<Quest> GetAvailableQuests()
    {
        List<Quest> availableQuests = new List<Quest>();
        foreach (Quest quest in guild.Quests)
        {
            if (!quest.claimed) availableQuests.Add(quest);
        }
        return availableQuests;
    }

    public override void CompleteBuild(string buildID)
    {
        if (buildID == "QuestBoard")
        {
            Unlocked = true;
            questBoardPanel.SetLockState(Unlocked);
        }
    }

    public void AddQuest(QuestData data)
    {
        Quest quest = new Quest(data, guild);
        guild.Quests.Add(quest);
        questBoardPanel.AddQuest(quest);

        if (data.unlockStory.Length > 0) guild.AddStoryEntry(data.unlockStoryHeader, data.unlockStory);
        if (data.unlockLog.Length > 0) guild.AddLogEntry(data.unlockLog);
    }
}
