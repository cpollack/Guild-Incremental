using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestBoard : GuildHall
{
    public List<Quest> quests = new List<Quest>();
    public QuestBoardPanel questBoardPanel;

    public int maxGuildQuests = 3;
    public bool staffed = false;

    public int totalGuildQuestsIssued = 0;

    private Dictionary<string, QuestData> allQuests = new Dictionary<string, QuestData>();

    new private void Awake()
    {
        base.Awake();
        questBoardPanel.SetLockState(Unlocked);

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

    public void CompleteQuest(Quest quest)
    {
        questBoardPanel.RemoveQuest(quest);
        quests.Remove(quest);        
        UpdateQuestPool();
        GenerateMainQuests();
    }

    public void UpdateQuestPool()
    {
        if (staffed)
        {
            while (GetGuildQuestCount() < maxGuildQuests)
            {
                GenerateGuildQuest();
            }
        }        
    }

    public void GenerateMainQuests()
    { 
        foreach (KeyValuePair<string, QuestData> kvp in allQuests)
        {
            QuestData data = (QuestData)kvp.Value;
            //Quest already active
            bool skip = false;
            foreach (Quest quest in quests)
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
        if (GetGuildQuestCount() >= maxGuildQuests) return;
        GenerateGuildQuest();
    }

    private int GetGuildQuestCount()
    {
        int count = 0;
        foreach (Quest quest in quests)
        {
            if (quest.category == QuestCategory.Guild) count++;
        }
        return count;
    }

    public void GenerateGuildQuest()
    {
        //Later this should select locations based on user defined rules

        Location questLoc = guild.locations[0];
        Quest newQuest = questLoc.GenerateRandomQuest();
        if (newQuest != null)
        {
            quests.Add(newQuest);
            questBoardPanel.AddQuest(newQuest);
            
            totalGuildQuestsIssued++;
            if (totalGuildQuestsIssued == 1)
            {
                guild.TriggerPopup("As you pin the first quest to the board, a young and bright eyed man walks in." +
                    "\n\n\"Slaying rats in the sewers, eh? I'm your man for the job!\"" +
                    "\n\nHe doesn't look like much with his hand-me-down worn leathers, and rusty sword. Still, he will have to do.");
                guild.AddLogEntry("Issued first guild quest. You have attracted the attention of a new adventurer!");
                guild.SpawnFirstAdventurer();
            }
        }
        else Debug.LogError("QuestBoard:GenerateGuildQuest failed to generate a quest for " + questLoc.data.Name);
    }

    public List<Quest> GetAvailableQuests()
    {
        List<Quest> availableQuests = new List<Quest>();
        foreach (Quest quest in quests)
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
        Quest quest = new Quest(data);
        quests.Add(quest);
        questBoardPanel.AddQuest(quest);

        if (data.unlockPopup.Length > 0) guild.TriggerPopup(data.unlockPopup);
        if (data.unlockLog.Length > 0) guild.AddLogEntry(data.unlockLog);
    }
}
