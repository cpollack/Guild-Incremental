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

    new private void Start()
    {
        base.Start();
        questBoardPanel.SetLockState(Unlocked);
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
}
