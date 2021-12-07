using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BuildingResource
{
    public ResourceType resourceType;
    public int value;

    public BuildingResource(ResourceType type, int val)
    {
        resourceType = type;
        value = val;
    }
}

public class Building
{
    public Building(Guild guild, BuildingData buildingData, BuildingPanel buildingUI)
    {
        this.guild = guild;
        data = buildingData;
        this.buildingPanel = buildingUI;
        this.buildingPanel.building = this;

        Started = false;
        Paused = false;
        Completed = false;
        UpdateButton();
        SetBuildData();
    }

    public BuildingData data;
    public Guild guild;

    public BuildingPanel buildingPanel;

    public bool Started { get; private set; }
    public bool Paused { get; private set; }
    public bool Completed { get; private set; }
    

    public GameTime startTime = new GameTime();
    public GameTime pauseTime = new GameTime();

    private enum BuildMode
    {
        None,
        Build,
        Pause,
        Continue,
    }
    private BuildMode buildMode = BuildMode.None;

    // Update is called once per frame
    public void Update()
    {
        if (Started && !Paused && !Completed)
        {
            GameTime elapsedTime = guild.GetElapsedTime(startTime);
            GameTime remTime = new GameTime(data.buildTimeDays, data.buildTimeHours);
            remTime = remTime.GetDifference(elapsedTime);
            buildingPanel.textTime.text = remTime.ToString() + " Remaining";

            float elapsed = elapsedTime.GetHours();
            float required = (data.buildTimeDays * 24) + data.buildTimeHours;
            float perc = Mathf.Min(elapsed / required, 1f);
            buildingPanel.progressBar.SetPercent(perc);
            if (perc == 1)
            {
                OnComplete();
            }
        }
    }

    public void OnClick()
    {
        switch (buildMode) {
            case BuildMode.Build:
                StartConstruction();
                break;
            case BuildMode.Pause:
                GameTime dif = new GameTime();
                dif.Set(pauseTime.GetDifference(startTime));
               //dif.SubtractHours(pauseTime.GetDifference(startTime).GetHours());
                startTime.Set(guild.currentTime);
                startTime.SubtractHours(dif.GetHours());
                pauseTime.Set(0, 0);

                Paused = false;
                UpdateButton();
                break;
            case BuildMode.Continue:
                Paused = true;
                pauseTime.Set(guild.currentTime);
                UpdateButton();
                break;
        }
    }

    private void StartConstruction()
    {
        if (guild.renown < data.requiredRenown) return;
        if (guild.gold < GetCost(ResourceType.Bank)) return;

        guild.gold -= GetCost(ResourceType.Bank);
        Started = true;
        startTime.day = guild.currentTime.day;
        startTime.hour = guild.currentTime.hour;
        UpdateButton();
    }

    private void UpdateButton()
    {
        if (!Started)
        {
            if (guild.renown < data.requiredRenown)
                buildMode = BuildMode.None;
            else
                buildMode = BuildMode.Build;
        }
        else
        {
            if (Paused)
                buildMode = BuildMode.Pause;
            else
                buildMode = BuildMode.Continue;
        }

        switch (buildMode) {
            case BuildMode.None:
                buildingPanel.textButton.text = data.requiredRenown.ToString("0") + " Renown";
                break;
            case BuildMode.Build:
                buildingPanel.textButton.text = "Build";
                break;
            case BuildMode.Pause:
                buildingPanel.textButton.text = "Resume";
                break;
            case BuildMode.Continue:
                buildingPanel.textButton.text = "Pause";
                break;
        }
    }

    private void SetBuildData()
    {
        buildingPanel.textName.text = data.buildingTitle;
        GameTime buildTime = new GameTime(data.buildTimeDays, data.buildTimeHours);
        buildingPanel.textTime.text = buildTime.ToString();
        buildingPanel.textFlavor.text = data.description;

        foreach (BuildingResource resource in data.cost)
            buildingPanel.AddCost(resource);
        //buildingUI.textCost.text = (data.costGold > 0 ? data.costGold.ToString() +  " Gold" : "");
    }

    private void OnComplete()
    {
        Completed = true;
        guild.CompleteBuilding(data.buildingID);
        buildingPanel.OnComplete();
        if (data.completeLog.Length > 0) guild.AddLogEntry(data.completeLog);
    }

    public bool IsActive()
    {
        return Started && !Paused;
    }

    private int GetCost(ResourceType resource)
    {
        foreach (BuildingResource buildingResource in data.cost)
            if (buildingResource.resourceType == resource)
                return buildingResource.value;
        return 0;
    }
}
