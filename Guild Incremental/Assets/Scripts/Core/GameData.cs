using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public string Name;
    public Rank rank;
    public GameTime currentTime;
    public int renown;
    public int gold;

    public List<Adventurer> adventurers;
    public List<Battle> activeBattles;
    public List<StoryEntry> storyEntries;
    public List<LogEntry> logEntries;

    //Buildings
    public List<string> completedBuildings;
    public List<Building> currentBuildProjects;
    public int maxConstructionJobs;

    //Quests
    public List<string> completedQuests;
    public List<Quest> quests;
    public int maxGuildQuests;
    public int totalGuildQuestsIssued;

    //Items
    public Dictionary<string, int> itemStatistics;

    //public List<RecipeEntry>

    public GameData()
    {
        Name = "Guild";
        rank = Rank.F;
        currentTime = new GameTime(1, 5);
        renown = 0;
        gold = 0;

        adventurers = new List<Adventurer>();
        activeBattles = new List<Battle>();

        storyEntries = new List<StoryEntry>();
        logEntries = new List<LogEntry>();

        completedBuildings = new List<string>();
        currentBuildProjects = new List<Building>();
        maxConstructionJobs = 1;

        quests = new List<Quest>();
        completedQuests = new List<string>();
        maxGuildQuests = 3;
        totalGuildQuestsIssued = 0;

        itemStatistics = new Dictionary<string, int>();
    }

    public void AfterLoad()
    {
        if (currentTime == null) currentTime = new GameTime(1, 5);

        if (adventurers == null) adventurers = new List<Adventurer>();
        if (activeBattles == null) activeBattles = new List<Battle>();
        if (logEntries == null) logEntries = new List<LogEntry>();
        if (storyEntries == null) storyEntries = new List<StoryEntry>();

        if (completedBuildings == null) completedBuildings = new List<string>();
        if (currentBuildProjects == null) currentBuildProjects = new List<Building>();

        if (completedQuests == null) completedQuests = new List<string>();
        if (quests == null) quests = new List<Quest>();

        if (itemStatistics == null) itemStatistics = new Dictionary<string, int>();
    }
}
