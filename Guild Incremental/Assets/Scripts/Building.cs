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

[Serializable]
public class Building
{
    public Building(Guild guild, BuildingData buildingData, BuildingPanel buildingUI)
    {
        this.guild = guild;
        data = buildingData;
        buildingID = data.buildingID;
        this.buildingPanel = buildingUI;
        this.buildingPanel.building = this;

        buildMode = BuildMode.None;
        Started = false;
        Paused = false;
        Completed = false;
        UpdateButton();
        SetBuildData();
    }

    public string buildingID;
    [NonSerialized] public BuildingData data;
    [NonSerialized] public Guild guild;
    [NonSerialized] public BuildingPanel buildingPanel;

    public bool Started;
    public bool Paused;
    public bool Completed;    

    public GameTime startTime = new GameTime();
    public GameTime pauseTime = new GameTime();

    public enum BuildMode
    {
        None,
        Build,
        Pause,
        Continue,
    }
    public BuildMode buildMode;

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

    public void OnLoad()
    {
        UpdateButton();
        SetBuildData();
        if (Started)
        {
            GameTime elapsedTime;
            if (Paused) elapsedTime = pauseTime.GetDifference(startTime);
            else elapsedTime = guild.GetElapsedTime(startTime);
            GameTime remTime = new GameTime(data.buildTimeDays, data.buildTimeHours);
            remTime = remTime.GetDifference(elapsedTime);
            buildingPanel.textTime.text = remTime.ToString() + " Remaining";

            float elapsed = elapsedTime.GetHours();
            float required = (data.buildTimeDays * 24) + data.buildTimeHours;
            float perc = Mathf.Min(elapsed / required, 1f);
            buildingPanel.progressBar.SetPercent(perc);
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
                startTime.Set(guild.CurrentTime);
                startTime.SubtractHours(dif.GetHours());
                pauseTime.Set(0, 0);

                Paused = false;
                UpdateButton();
                break;
            case BuildMode.Continue:
                Paused = true;
                pauseTime.Set(guild.CurrentTime);
                UpdateButton();
                break;
        }
    }

    private void StartConstruction()
    {
        if (guild.Renown < data.requiredRenown) return;
        if (guild.Gold < GetCost(ResourceType.Gold)) return;

        guild.Gold -= GetCost(ResourceType.Gold);
        Started = true;
        startTime.day = guild.CurrentTime.day;
        startTime.hour = guild.CurrentTime.hour;
        UpdateButton();
    }

    private void UpdateButton()
    {
        if (!Started)
        {
            if (guild.Renown < data.requiredRenown)
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
