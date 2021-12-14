using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public GameTime currentTime;
    public int renown;
    public int gold;

    public List<Adventurer> adventurers;

    public List<string> completedBuildings;
    public List<Building> currentBuildProjects;
    public int maxConstructionJobs;

    public List<string> completedQuests;
    public List<Quest> quests;
    public int maxGuildQuests;
    public int totalGuildQuestsIssued;

    public GameData()
    {
        currentTime = new GameTime(1, 5);
        renown = 0;
        gold = 1000;

        adventurers = new List<Adventurer>();

        completedBuildings = new List<string>();
        currentBuildProjects = new List<Building>();
        maxConstructionJobs = 1;

        quests = new List<Quest>();
        completedQuests = new List<string>();
        maxGuildQuests = 3;
        totalGuildQuestsIssued = 0;
    }

    public void AfterLoad()
    {
        if (currentTime == null) currentTime = new GameTime(1, 5);

        if (adventurers == null) adventurers = new List<Adventurer>();

        if (completedBuildings == null) completedBuildings = new List<string>();
        if (currentBuildProjects == null) currentBuildProjects = new List<Building>();

        if (completedQuests == null) completedQuests = new List<string>();
        if (quests == null) quests = new List<Quest>();
    }
}
