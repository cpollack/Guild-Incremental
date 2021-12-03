using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public Building(Guild guild, BuildingData buildingData, BuildingUI buildingUI)
    {
        this.guild = guild;
        data = buildingData;
        this.buildingUI = buildingUI;

        Started = false;
        Paused = false;
        Completed = false;
        UpdateButton();
        UpdateBuildData();
    }

    public BuildingData data;
    private Guild guild;

    public BuildingUI buildingUI;

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
            buildingUI.textTime.text = remTime.ToString() + " Remaining";

            float elapsed = elapsedTime.GetHours();
            float required = (data.buildTimeDays * 24) + data.buildTimeHours;
            float perc = Mathf.Min(elapsed / required, 1f);
            buildingUI.progressBar.SetPercent(perc);
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
        if (guild.gold < data.costGold) return;

        guild.gold -= data.costGold;
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
                buildingUI.textButton.text = data.requiredRenown.ToString("0") + " Renown";
                break;
            case BuildMode.Build:
                buildingUI.textButton.text = "Build";
                break;
            case BuildMode.Pause:
                buildingUI.textButton.text = "Resume";
                break;
            case BuildMode.Continue:
                buildingUI.textButton.text = "Pause";
                break;
        }
    }

    private void UpdateBuildData()
    {
        buildingUI.textName.text = data.buildingTitle;
        GameTime buildTime = new GameTime(data.buildTimeDays, data.buildTimeHours);
        buildingUI.textTime.text = buildTime.ToString();
        buildingUI.textFlavor.text = data.description;
        buildingUI.textCost.text = (data.costGold > 0 ? data.costGold.ToString() +  " Gold" : "");
    }

    private void OnComplete()
    {
        Completed = true;
        guild.CompleteBuilding(data.buildingID);
        buildingUI.OnComplete();
        if (data.completeLog.Length > 0) guild.AddLogEntry(data.completeLog);
    }
}
