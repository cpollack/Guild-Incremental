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

    public List<string> completedBuildings;
    public List<Building> currentBuildProjects;
    public int maxConstructionJobs;

    public List<string> completedQuests;

    public GameData()
    {
        currentTime = new GameTime(1, 5);
        renown = 0;
        gold = 1000;

        completedBuildings = new List<string>();
        currentBuildProjects = new List<Building>();
        maxConstructionJobs = 1;

        completedQuests = new List<string>();
    }

    public void AfterLoad()
    {
        if (currentTime == null) currentTime = new GameTime(1, 5);
        if (completedBuildings == null) completedBuildings = new List<string>();
        if (currentBuildProjects == null) currentBuildProjects = new List<Building>();
        if (completedQuests == null) completedQuests = new List<string>();
    }
}
